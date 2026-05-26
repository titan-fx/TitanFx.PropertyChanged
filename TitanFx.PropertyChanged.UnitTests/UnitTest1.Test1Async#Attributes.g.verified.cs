//HintName: Attributes.g.cs
#nullable enable
namespace Microsoft.CodeAnalysis
{
    internal sealed partial class EmbeddedAttribute : global::System.Attribute { }
}

namespace TitanFx.PropertyChanged
{
    [global::System.AttributeUsageAttribute(global::System.AttributeTargets.Class | global::System.AttributeTargets.Struct | global::System.AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class NotifyPropertyChangedAttribute : global::System.Attribute { }

    [global::System.AttributeUsageAttribute(global::System.AttributeTargets.Class | global::System.AttributeTargets.Struct | global::System.AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class NotifyPropertyChangingAttribute : global::System.Attribute { }

    [global::System.AttributeUsageAttribute(global::System.AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class NotifyAdditionalPropertiesAttribute(params global::System.String[] dependencies) : global::System.Attribute { }
}