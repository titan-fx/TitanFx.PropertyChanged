using Microsoft.CodeAnalysis;

namespace TitanFx.PropertyChanged;

internal static class AttributeGenerator
{
    public const string NotifyPropertyChangedAttribute = nameof(NotifyPropertyChangedAttribute);
    public const string NotifyPropertyChangingAttribute = nameof(NotifyPropertyChangingAttribute);
    public const string NotifyAdditionalPropertiesAttribute = nameof(
        NotifyAdditionalPropertiesAttribute
    );

    internal static void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(Output);
    }

    private static void Output(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("Attributes.g.cs", _sourceCode);
    }

    private const string _tAttribute = "global::System.Attribute";
    private const string _tAttributeUsage = "global::System.AttributeUsageAttribute";
    private const string _tAttributeTargets = "global::System.AttributeTargets";
    private const string _tString = "global::System.String";

    private const string _sourceCode = $$"""
        #nullable enable
        namespace Microsoft.CodeAnalysis
        {
            internal sealed partial class EmbeddedAttribute : {{_tAttribute}} { }
        }

        namespace TitanFx.PropertyChanged
        {
            [{{_tAttributeUsage}}({{_tAttributeTargets}}.Class | {{_tAttributeTargets}}.Struct | {{_tAttributeTargets}}.Property, AllowMultiple = false, Inherited = false)]
            internal sealed class {{NotifyPropertyChangedAttribute}} : {{_tAttribute}} { }

            [{{_tAttributeUsage}}({{_tAttributeTargets}}.Class | {{_tAttributeTargets}}.Struct | {{_tAttributeTargets}}.Property, AllowMultiple = false, Inherited = false)]
            internal sealed class {{NotifyPropertyChangingAttribute}} : {{_tAttribute}} { }

            [{{_tAttributeUsage}}({{_tAttributeTargets}}.Property, AllowMultiple = false, Inherited = false)]
            internal sealed class {{NotifyAdditionalPropertiesAttribute}}(params {{_tString}}[] dependencies) : {{_tAttribute}} { }
        }
        """;
}
