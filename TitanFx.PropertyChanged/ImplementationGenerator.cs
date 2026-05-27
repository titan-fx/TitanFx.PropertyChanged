using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using TitanFx.PropertyChanged.Models;
using static TitanFx.PropertyChanged.Models.Constants;

namespace TitanFx.PropertyChanged;

internal static class ImplementationGenerator
{
    internal static void Initialize(
        IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<TypeCapture> types
    )
    {
        context.RegisterSourceOutput(
            types.Select(
                static (v, _) =>
                    new State
                    {
                        Location = v.Location,
                        NotifyPropertyChanged = v.NotifyPropertyChanged,
                        NotifyPropertyChanging = v.NotifyPropertyChanging,
                        IsRefStruct = v.IsRefStruct,
                    }
            ),
            GenerateSource
        );
    }

    private static void GenerateSource(SourceProductionContext context, State type)
    {
        context.AddSource(Util.ToHintName(type.Location, "Implementation"), ToSourceCode(type));
    }

    private sealed record State
    {
        public required TypeLocationCapture Location { get; init; }
        public required bool NotifyPropertyChanged { get; init; }
        public required bool NotifyPropertyChanging { get; init; }
        public required bool IsRefStruct { get; init; }
    }

    private static string ToSourceCode(State type)
    {
        var source = new StringWriter();
        var sourceGen = new IndentedTextWriter(source);

        sourceGen.WriteLine("#nullable enable");
        using (
            PartialType.Write(
                sourceGen,
                type.Location,
                () =>
                {
                    if (type.NotifyPropertyChanged)
                    {
                        sourceGen.Write($" : {Types.INotifyPropertyChanged}");
                        if (type.NotifyPropertyChanging)
                            sourceGen.Write($", {Types.INotifyPropertyChanging}");
                    }
                    else if (type.NotifyPropertyChanging)
                        sourceGen.Write($" : {Types.INotifyPropertyChanging}");
                }
            )
        )
        {
            sourceGen.WriteLine($"partial void LoadSender(ref object? sender);");
            if (type.NotifyPropertyChanged)
            {
                sourceGen.WriteLine(
                    $"public event {Types.PropertyChangedEventHandler}? PropertyChanged;"
                );
                WriteRaiseEventMethod(sourceGen, "Changed", type.IsRefStruct);
            }
            if (type.NotifyPropertyChanging)
            {
                sourceGen.WriteLine(
                    $"public event {Types.PropertyChangingEventHandler}? PropertyChanging;"
                );
                WriteRaiseEventMethod(sourceGen, "Changing", type.IsRefStruct);
            }

            foreach (var (parameter, name) in _setMethods)
            {
                foreach (var writer in _setMethodWriters)
                {
                    writer(
                        sourceGen,
                        type.NotifyPropertyChanged,
                        type.NotifyPropertyChanging,
                        parameter,
                        name
                    );
                }
            }
        }

        return source.ToString();
    }

    private static void WriteRaiseEventMethod(
        IndentedTextWriter sourceGen,
        string kind,
        bool isRefStruct
    )
    {
        sourceGen.WriteLine(
            $"private void OnProperty{kind}(params {Types.ReadOnlySpan}<{Types.String}?> propertyNames)"
        );
        using (Util.WriteBlock(sourceGen))
        {
            sourceGen.WriteLine($"{Types.Object}? sender = {(isRefStruct ? "null" : "this")};");
            sourceGen.WriteLine($"LoadSender(ref sender);");
            sourceGen.WriteLine($"{Types.Exception}? exception = null;");
            using (Util.WriteForeach(sourceGen, "propertyName", "propertyNames"))
            {
                using (Util.WriteTry(sourceGen))
                {
                    sourceGen.WriteLine(
                        $"Property{kind}?.Invoke(sender, {Types.PropertyChangeEventArgs}.{kind}(propertyName));"
                    );
                }
                using (Util.WriteCatch(sourceGen, $"{Types.Exception} ex"))
                {
                    sourceGen.WriteLine("exception = ex;");
                }
            }
            using (Util.WriteIf(sourceGen, $"!{Types.Object}.ReferenceEquals(exception, null)"))
            {
                sourceGen.WriteLine($"{Types.ExceptionDispatchInfo}.Throw(exception);");
            }
        }
    }

    private static void WriteSetRefMethod(
        IndentedTextWriter sourceGen,
        bool implNotifyPropertyChanged,
        bool implNotifyPropertyChanging,
        string propertyNameArgDef,
        string propertyNameArg
    )
    {
        sourceGen.WriteLine($"private void {Set}<T>(ref T field, T value, {propertyNameArgDef})");
        using (Util.WriteBlock(sourceGen))
        {
            WriteSetMethodBody(
                sourceGen,
                $"!{Types.EqualityComparer}<T>.Default.Equals(field, value)",
                "field = value;",
                implNotifyPropertyChanged,
                implNotifyPropertyChanging,
                propertyNameArg
            );
        }
    }

    private static void WriteSetConditionalActionMethod(
        IndentedTextWriter sourceGen,
        bool implNotifyPropertyChanged,
        bool implNotifyPropertyChanging,
        string propertyNameArgDef,
        string propertyNameArg
    )
    {
        sourceGen.WriteLine(
            $"private void {Set}<T>(bool condition, T value, Action<T> assign, {propertyNameArgDef})"
        );
        sourceGen.Indent++;
        sourceGen.WriteLine($"where T : allows ref struct");
        sourceGen.Indent--;
        using (Util.WriteBlock(sourceGen))
        {
            WriteSetMethodBody(
                sourceGen,
                "condition",
                "assign(value);",
                implNotifyPropertyChanged,
                implNotifyPropertyChanging,
                propertyNameArg
            );
        }
    }

    private static void WriteSetConditionalActionStateMethod(
        IndentedTextWriter sourceGen,
        bool implNotifyPropertyChanged,
        bool implNotifyPropertyChanging,
        string propertyNameArgDef,
        string propertyNameArg
    )
    {
        sourceGen.WriteLine(
            $"private void {Set}<TValue, TState>(bool condition, in TState state, TValue value, {Types.Setter}<TState, TValue> assign, {propertyNameArgDef})"
        );
        sourceGen.Indent++;
        sourceGen.WriteLine($"where TValue : allows ref struct");
        sourceGen.WriteLine($"where TState : allows ref struct");
        sourceGen.Indent--;
        using (Util.WriteBlock(sourceGen))
        {
            WriteSetMethodBody(
                sourceGen,
                "condition",
                "assign(in state, value);",
                implNotifyPropertyChanged,
                implNotifyPropertyChanging,
                propertyNameArg
            );
        }
    }

    private static void WriteSetConditionalRefMethod(
        IndentedTextWriter sourceGen,
        bool implNotifyPropertyChanged,
        bool implNotifyPropertyChanging,
        string propertyNameArgDef,
        string propertyNameArg
    )
    {
        sourceGen.WriteLine(
            $"private void {Set}<T>(bool condition, ref T field, T value, {propertyNameArgDef})"
        );
        sourceGen.Indent++;
        sourceGen.WriteLine($"where T : allows ref struct");
        sourceGen.Indent--;
        using (Util.WriteBlock(sourceGen))
        {
            WriteSetMethodBody(
                sourceGen,
                "condition",
                "field = value;",
                implNotifyPropertyChanged,
                implNotifyPropertyChanging,
                propertyNameArg
            );
        }
    }

    private static void WriteSetMethodBody(
        IndentedTextWriter sourceGen,
        string condition,
        string assign,
        bool implNotifyPropertyChanged,
        bool implNotifyPropertyChanging,
        string propertyNameArg
    )
    {
        using (Util.WriteIf(sourceGen, condition))
        {
            if (implNotifyPropertyChanging)
            {
                using (Util.WriteTry(sourceGen))
                {
                    sourceGen.WriteLine($"OnPropertyChanging({propertyNameArg});");
                }
                using (Util.WriteFinally(sourceGen))
                {
                    sourceGen.WriteLine(assign);
                    if (implNotifyPropertyChanged)
                    {
                        sourceGen.WriteLine($"OnPropertyChanged({propertyNameArg});");
                    }
                }
            }
            else
            {
                sourceGen.WriteLine(assign);
                if (implNotifyPropertyChanged)
                {
                    sourceGen.WriteLine($"OnPropertyChanged({propertyNameArg});");
                }
            }
        }
    }

    private delegate void SetMethodWriter(
        IndentedTextWriter sourceGen,
        bool implNotifyPropertyChanged,
        bool implNotifyPropertyChanging,
        string propertyNameArgDef,
        string propertyNameArg
    );

    private static readonly (string parameter, string name)[] _setMethods =
    [
        ($"[{Types.CallerMemberName}] {Types.String}? propertyName = null", "propertyName"),
        ($"params {Types.ReadOnlySpan}<{Types.String}> propertyNames", "propertyNames"),
    ];

    private static readonly SetMethodWriter[] _setMethodWriters =
    [
        WriteSetRefMethod,
        WriteSetConditionalRefMethod,
        WriteSetConditionalActionMethod,
        WriteSetConditionalActionStateMethod,
    ];
}
