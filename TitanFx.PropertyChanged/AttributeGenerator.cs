using Microsoft.CodeAnalysis;
using static TitanFx.PropertyChanged.Models.Constants;

namespace TitanFx.PropertyChanged;

internal static class AttributeGenerator
{
    internal static void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(Output);
    }

    private static void Output(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("Attributes.g.cs", _sourceCode);
        context.AddEmbeddedAttributeDefinition();
    }

    private const string _sourceCode = $$"""
        #nullable enable

        namespace {{RootNamespace}}
        {
            [{{Types.EmbeddedAttribute}}]
            [{{Types.AttributeUsage}}({{Types.AttributeTargets}}.Class | {{Types.AttributeTargets}}.Struct, AllowMultiple = false, Inherited = false)]
            internal sealed class {{NotifyPropertyChanged}} : {{Types.Attribute}} { }

            [{{Types.EmbeddedAttribute}}]
            [{{Types.AttributeUsage}}({{Types.AttributeTargets}}.Class | {{Types.AttributeTargets}}.Struct, AllowMultiple = false, Inherited = false)]
            internal sealed class {{NotifyPropertyChanging}} : {{Types.Attribute}} { }

            [{{Types.EmbeddedAttribute}}]
            [{{Types.AttributeUsage}}({{Types.AttributeTargets}}.Property, AllowMultiple = false, Inherited = false)]
            internal sealed class {{NotifyPropertyChange}} : {{Types.Attribute}} 
            {
                public {{Types.Boolean}} {{Skip}} { get; init; } = false;

                private readonly {{Types.String}}[] {{AffectsField}} = [];
                public {{Types.String}}[] {{AffectsProp}}
                {
                    get
                    {
                        var result = new {{Types.String}}[{{AffectsField}}.Length];
                        {{AffectsField}}.CopyTo(result, 0);
                        return result;
                    }
                    init 
                    {
                        {{AffectsField}} = new {{Types.String}}[value.Length];
                        value.CopyTo({{AffectsField}}, 0);
                    }
                }
            }

            [{{Types.EmbeddedAttribute}}]
            internal delegate void {{Setter}}<TState, TValue>(in TState state, TValue value)
                where TState : allows ref struct
                where TValue : allows ref struct;
        }
        """;
}
