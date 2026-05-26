using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace TitanFx.PropertyChanged;

internal static class ImplementationGenerator
{
    private const string _tINotifyPropertyChanged =
        "global::System.ComponentModel.INotifyPropertyChanged";
    private const string _tINotifyPropertyChanging =
        "global::System.ComponentModel.INotifyPropertyChanging";
    private const string _tPropertyChangedEventHandler =
        "global::System.ComponentModel.PropertyChangedEventHandler";
    private const string _tPropertyChangedEventArgs =
        "global::System.ComponentModel.PropertyChangedEventArgs";
    private const string _tPropertyChangingEventHandler =
        "global::System.ComponentModel.PropertyChangingEventHandler";
    private const string _tPropertyChangingEventArgs =
        "global::System.ComponentModel.PropertyChangingEventArgs";
    private const string _tCallerMemberName =
        "global::System.Runtime.CompilerServices.CallerMemberNameAttribute";
    private const string _tString = "global::System.String";
    private const string _tObject = "global::System.Object";
    private const string _tReadOnlySpan = "global::System.ReadOnlySpan";
    private const string _tException = "global::System.Exception";
    private const string _tExceptionDispatchInfo =
        "global::System.Runtime.ExceptionServices.ExceptionDispatchInfo";
    private const string _tEqualityComparer = "global::System.Collections.Generic.EqualityComparer";

    internal static void GenerateSource(SourceProductionContext context, TypeCapture type)
    {
        context.AddSource(ToHintName(type), ToSourceCode(type));
    }

    private static string ToSourceCode(TypeCapture type)
    {
        var source = new StringWriter();
        var sourceGen = new IndentedTextWriter(source);

        sourceGen.WriteLine("#nullable enable");

        if (type.Namespace is { } ns)
        {
            sourceGen.WriteLine($"namespace {ns}");
            sourceGen.WriteLine("{");
            sourceGen.Indent++;
        }

        foreach (var container in type.ContainingTypes)
        {
            WriteTypeHeader(sourceGen, container);
            sourceGen.WriteLine();
            sourceGen.WriteLine("{");
            sourceGen.Indent++;
        }

        WriteTypeHeader(sourceGen, type.Header);
        var implNotifyPropertyChanged =
            type.NotifyPropertyChanged || type.Properties.Any(p => p.NotifyPropertyChanged);
        var implNotifyPropertyChanging =
            type.NotifyPropertyChanging || type.Properties.Any(p => p.NotifyPropertyChanging);

        if (implNotifyPropertyChanged)
        {
            sourceGen.Write($" : {_tINotifyPropertyChanged}");
            if (implNotifyPropertyChanging)
                sourceGen.Write($", {_tINotifyPropertyChanging}");
        }
        else if (implNotifyPropertyChanging)
            sourceGen.Write($" : {_tINotifyPropertyChanging}");
        sourceGen.WriteLine();
        sourceGen.WriteLine("{");
        sourceGen.Indent++;

        if (implNotifyPropertyChanged)
            sourceGen.WriteLine($"public event {_tPropertyChangedEventHandler}? PropertyChanged;");
        if (implNotifyPropertyChanging)
            sourceGen.WriteLine(
                $"public event {_tPropertyChangingEventHandler}? PropertyChanging;"
            );

        if (implNotifyPropertyChanged)
            WriteRaiseEventMethod(sourceGen, "PropertyChanged", _tPropertyChangedEventArgs);
        if (implNotifyPropertyChanging)
            WriteRaiseEventMethod(sourceGen, "PropertyChanging", _tPropertyChangingEventArgs);
        WriteSetMethod(sourceGen, implNotifyPropertyChanged, implNotifyPropertyChanging);
        WriteSetMultipleMethod(sourceGen, implNotifyPropertyChanged, implNotifyPropertyChanging);

        var force = type.NotifyPropertyChanged || type.NotifyPropertyChanging;
        foreach (var property in type.Properties)
        {
            WriteProperty(sourceGen, property, force);
        }

        sourceGen.Indent--;
        sourceGen.WriteLine("}");

        foreach (var container in type.ContainingTypes)
        {
            sourceGen.Indent--;
            sourceGen.WriteLine("}");
        }

        if (type.Namespace is { })
        {
            sourceGen.Indent--;
            sourceGen.WriteLine("}");
        }

        return source.ToString();
    }

    private static void WriteRaiseEventMethod(
        IndentedTextWriter sourceGen,
        string kind,
        string eventArgs
    )
    {
        sourceGen.WriteLine(
            $"private void On{kind}(params {_tReadOnlySpan}<{_tString}?> propertyNames)"
        );
        sourceGen.WriteLine("{");
        sourceGen.Indent++;
        {
            sourceGen.WriteLine($"{_tException}? exception = null;");
            sourceGen.WriteLine($"foreach (var propertyName in propertyNames)");
            sourceGen.WriteLine("{");
            sourceGen.Indent++;
            {
                sourceGen.WriteLine("try");
                sourceGen.WriteLine("{");
                sourceGen.Indent++;
                {
                    sourceGen.WriteLine($"{kind}?.Invoke(this, new {eventArgs}(propertyName));");
                }
                sourceGen.Indent--;
                sourceGen.WriteLine("}");
                sourceGen.WriteLine($"catch ({_tException} ex)");
                sourceGen.WriteLine("{");
                sourceGen.Indent++;
                {
                    sourceGen.WriteLine("exception = ex;");
                }
                sourceGen.Indent--;
                sourceGen.WriteLine("}");
            }
            sourceGen.Indent--;
            sourceGen.WriteLine("}");
            sourceGen.WriteLine($"if (!{_tObject}.ReferenceEquals(exception, null))");
            sourceGen.WriteLine("{");
            sourceGen.Indent++;
            {
                sourceGen.WriteLine($"{_tExceptionDispatchInfo}.Throw(exception);");
            }
            sourceGen.Indent--;
            sourceGen.WriteLine("}");
        }
        sourceGen.Indent--;
        sourceGen.WriteLine("}");
    }

    private static void WriteSetMethod(
        IndentedTextWriter sourceGen,
        bool implNotifyPropertyChanged,
        bool implNotifyPropertyChanging
    )
    {
        sourceGen.WriteLine(
            $"private void Set<T>(ref T field, T value, [{_tCallerMemberName}] {_tString}? propertyName = null)"
        );
        sourceGen.WriteLine("{");
        sourceGen.Indent++;
        {
            sourceGen.WriteLine($"if (!{_tEqualityComparer}<T>.Default.Equals(field, value))");
            sourceGen.WriteLine("{");
            sourceGen.Indent++;
            {
                if (implNotifyPropertyChanging)
                {
                    sourceGen.WriteLine("try");
                    sourceGen.WriteLine("{");
                    sourceGen.Indent++;
                    {
                        sourceGen.WriteLine($"OnPropertyChanging(propertyName);");
                    }
                    sourceGen.Indent--;
                    sourceGen.WriteLine("}");
                    sourceGen.WriteLine("finally");
                    sourceGen.WriteLine("{");
                    sourceGen.Indent++;
                    {
                        sourceGen.WriteLine("field = value;");
                        if (implNotifyPropertyChanged)
                        {
                            sourceGen.WriteLine($"OnPropertyChanged(propertyName);");
                        }
                    }
                    sourceGen.Indent--;
                    sourceGen.WriteLine("}");
                }
                else
                {
                    sourceGen.WriteLine("field = value;");
                    if (implNotifyPropertyChanged)
                    {
                        sourceGen.WriteLine($"OnPropertyChanged(propertyName);");
                    }
                }
            }
            sourceGen.Indent--;
            sourceGen.WriteLine("}");
        }
        sourceGen.Indent--;
        sourceGen.WriteLine("}");
    }

    private static void WriteSetMultipleMethod(
        IndentedTextWriter sourceGen,
        bool implNotifyPropertyChanged,
        bool implNotifyPropertyChanging
    )
    {
        sourceGen.WriteLine(
            $"private void Set<T>(ref T field, T value, params {_tReadOnlySpan}<{_tString}> propertyNames)"
        );
        sourceGen.WriteLine("{");
        sourceGen.Indent++;
        {
            sourceGen.WriteLine($"if (!{_tEqualityComparer}<T>.Default.Equals(field, value))");
            sourceGen.WriteLine("{");
            sourceGen.Indent++;
            {
                if (implNotifyPropertyChanging)
                {
                    sourceGen.WriteLine("try");
                    sourceGen.WriteLine("{");
                    sourceGen.Indent++;
                    {
                        sourceGen.WriteLine($"OnPropertyChanging(propertyNames);");
                    }
                    sourceGen.Indent--;
                    sourceGen.WriteLine("}");
                    sourceGen.WriteLine("finally");
                    sourceGen.WriteLine("{");
                    sourceGen.Indent++;
                    {
                        sourceGen.WriteLine("field = value;");
                        if (implNotifyPropertyChanged)
                        {
                            sourceGen.WriteLine($"OnPropertyChanged(propertyNames);");
                        }
                    }
                    sourceGen.Indent--;
                    sourceGen.WriteLine("}");
                }
                else
                {
                    sourceGen.WriteLine("field = value;");
                    if (implNotifyPropertyChanged)
                    {
                        sourceGen.WriteLine($"OnPropertyChanged(propertyNames);");
                    }
                }
            }
            sourceGen.Indent--;
            sourceGen.WriteLine("}");
        }
        sourceGen.Indent--;
        sourceGen.WriteLine("}");
    }

    private static void WriteProperty(
        IndentedTextWriter sourceGen,
        PropertyCapture property,
        bool force
    )
    {
        if (property is not { IsPartial: true, Setter: not null })
            return;

        if (!force && !property.NotifyPropertyChanged && !property.NotifyPropertyChanging)
            return;

        sourceGen.WriteLine(
            $"{ToString(property.Accessibility)} partial {property.Type} {property.Name} {{ get; set => Set(ref field, value, \"{property.Name}\"); }}"
        );
    }

    private static string ToString(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.Public => "public",
            _ => "",
        };
    }

    private static void WriteTypeHeader(IndentedTextWriter sourceGen, TypeHeaderCapture type)
    {
        sourceGen.Write($"partial {type.Kind} {type.Name}");
        if (type.TypeParameters.Count > 0)
        {
            sourceGen.Write("<");
            foreach (var typeParameter in type.TypeParameters.SkipLast(1))
            {
                sourceGen.Write(typeParameter);
                sourceGen.Write(", ");
            }
            sourceGen.Write(type.TypeParameters[^1]);
            sourceGen.Write(">");
        }
    }

    private static string ToHintName(TypeCapture type)
    {
        var hintBuilder = new StringBuilder();
        if (type.Namespace is { } ns)
            _ = hintBuilder.Append($"{ns}.");
        foreach (var container in type.ContainingTypes)
            _ = hintBuilder.Append($"{container.Name}`{container.TypeParameters.Count}.");
        _ = hintBuilder.Append(
            $"{type.Header.Name}`{type.Header.TypeParameters.Count}.PropertyChanged.g.cs"
        );
        return hintBuilder.ToString();
    }
}
