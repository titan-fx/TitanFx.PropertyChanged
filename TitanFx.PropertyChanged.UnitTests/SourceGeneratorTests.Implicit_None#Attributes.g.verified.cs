//HintName: Attributes.g.cs
#nullable enable

namespace TitanFx.PropertyChanged
{
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    [global::System.AttributeUsageAttribute(global::System.AttributeTargets.Class | global::System.AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    internal sealed class NotifyPropertyChangedAttribute : global::System.Attribute { }

    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    [global::System.AttributeUsageAttribute(global::System.AttributeTargets.Class | global::System.AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    internal sealed class NotifyPropertyChangingAttribute : global::System.Attribute { }

    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    [global::System.AttributeUsageAttribute(global::System.AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class NotifyPropertyChangeAttribute : global::System.Attribute 
    {
        public global::System.Boolean Skip { get; init; } = false;

        private readonly global::System.String[] _affects = [];
        public global::System.String[] Affects
        {
            get
            {
                var result = new global::System.String[_affects.Length];
                _affects.CopyTo(result, 0);
                return result;
            }
            init 
            {
                _affects = new global::System.String[value.Length];
                value.CopyTo(_affects, 0);
            }
        }
    }

    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal delegate void Setter<TState, TValue>(in TState state, TValue value)
        where TState : allows ref struct
        where TValue : allows ref struct;
}