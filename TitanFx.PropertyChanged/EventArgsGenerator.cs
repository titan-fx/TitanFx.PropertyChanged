using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TitanFx.PropertyChanged.Models;
using static TitanFx.PropertyChanged.Models.Constants;

namespace TitanFx.PropertyChanged;

internal static class EventArgsGenerator
{
    internal static void Initialize(
        IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<TypeCapture> types
    )
    {
        context.RegisterSourceOutput(
            types
                .Collect()
                .Select(
                    static (v, _) =>
                        new ValueArray<string>(
                            v.SelectMany(static v =>
                                    v.Properties.SelectMany(static x =>
                                        x.Dependents.Prepend(x.Name)
                                    )
                                )
                                .Distinct()
                        )
                ),
            GenerateSource
        );
    }

    private static void GenerateSource(
        SourceProductionContext context,
        ValueArray<string> properties
    )
    {
        context.AddSource($"{PropertyChangeEventArgs}.g.cs", ToSourceCode(properties));
    }

    private static string ToSourceCode(ValueArray<string> properties)
    {
        var source = new StringWriter();
        var sourceGen = new IndentedTextWriter(source);

        sourceGen.WriteLine("#nullable enable");
        using (Util.WriteNamespace(sourceGen, RootNamespace))
        {
            sourceGen.WriteLine($"[{Types.EmbeddedAttribute}]");
            sourceGen.WriteLine($"internal static partial class {PropertyChangeEventArgs}");
            using (Util.WriteBlock(sourceGen))
            {
                foreach (var (name, type) in _eventArgs)
                {
                    var keyValuePair = $"{Types.KeyValuePair}<{Types.String}, {type}>";
                    sourceGen.WriteLine(
                        $"private static readonly {type} _null{name} = new {type}(propertyName: null);"
                    );
                    sourceGen.WriteLine(
                        $"private static readonly {Types.FrozenDictionary}<{Types.String},{type}> _lookup{name} = {Types.FrozenDictionary}.ToFrozenDictionary("
                    );
                    sourceGen.Indent++;
                    sourceGen.WriteLine($"new {keyValuePair}[]");
                    using (Util.WriteBlock(sourceGen))
                    {
                        foreach (var property in properties)
                        {
                            var literal = SymbolDisplay.FormatLiteral(property, quote: true);
                            sourceGen.WriteLine(
                                $"new {keyValuePair}({literal}, new {type}({literal})),"
                            );
                        }
                    }
                    sourceGen.Indent--;
                    sourceGen.WriteLine(");");
                }

                foreach (var (name, type) in _eventArgs)
                {
                    sourceGen.WriteLine(
                        $"static partial void {name}Core({Types.String} propertyName, ref {type}? result);"
                    );
                    sourceGen.WriteLine(
                        $"public static {type} {name}({Types.String}? propertyName)"
                    );
                    using (Util.WriteBlock(sourceGen))
                    {
                        using (
                            Util.WriteIf(
                                sourceGen,
                                $"{Types.Object}.ReferenceEquals(propertyName, null)"
                            )
                        )
                        {
                            sourceGen.WriteLine($"return _null{name};");
                        }

                        sourceGen.WriteLine($"{type}? result = null;");
                        sourceGen.WriteLine($"{name}Core(propertyName, ref result);");
                        using (
                            Util.WriteIf(
                                sourceGen,
                                $"!{Types.Object}.ReferenceEquals(result, null)"
                            )
                        )
                        {
                            sourceGen.WriteLine("return result;");
                        }

                        using (
                            Util.WriteIf(
                                sourceGen,
                                $"_lookup{name}.TryGetValue(propertyName, out result)"
                            )
                        )
                        {
                            sourceGen.WriteLine("return result;");
                        }

                        sourceGen.WriteLine($"return new {type}(propertyName);");
                    }
                }
            }
        }

        return source.ToString();
    }

    private static readonly (string Name, string Type)[] _eventArgs =
    [
        ("Changed", Types.PropertyChangedEventArgs),
        ("Changing", Types.PropertyChangingEventArgs),
    ];
}
