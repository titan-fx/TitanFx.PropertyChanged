//HintName: PropertyChangeEventArgs.g.cs
#nullable enable
namespace TitanFx.PropertyChanged
{
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal static partial class PropertyChangeEventArgs
    {
        private static readonly global::System.ComponentModel.PropertyChangedEventArgs _nullChanged = new global::System.ComponentModel.PropertyChangedEventArgs(propertyName: null);
        private static readonly global::System.Collections.Frozen.FrozenDictionary<global::System.String,global::System.ComponentModel.PropertyChangedEventArgs> _lookupChanged = global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(
            new global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.ComponentModel.PropertyChangedEventArgs>[]
            {
                new global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.ComponentModel.PropertyChangedEventArgs>("Name", new global::System.ComponentModel.PropertyChangedEventArgs("Name")),
                new global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.ComponentModel.PropertyChangedEventArgs>("Age", new global::System.ComponentModel.PropertyChangedEventArgs("Age")),
                new global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.ComponentModel.PropertyChangedEventArgs>("IsEmployed", new global::System.ComponentModel.PropertyChangedEventArgs("IsEmployed")),
            }
        );
        private static readonly global::System.ComponentModel.PropertyChangingEventArgs _nullChanging = new global::System.ComponentModel.PropertyChangingEventArgs(propertyName: null);
        private static readonly global::System.Collections.Frozen.FrozenDictionary<global::System.String,global::System.ComponentModel.PropertyChangingEventArgs> _lookupChanging = global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(
            new global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.ComponentModel.PropertyChangingEventArgs>[]
            {
                new global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.ComponentModel.PropertyChangingEventArgs>("Name", new global::System.ComponentModel.PropertyChangingEventArgs("Name")),
                new global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.ComponentModel.PropertyChangingEventArgs>("Age", new global::System.ComponentModel.PropertyChangingEventArgs("Age")),
                new global::System.Collections.Generic.KeyValuePair<global::System.String, global::System.ComponentModel.PropertyChangingEventArgs>("IsEmployed", new global::System.ComponentModel.PropertyChangingEventArgs("IsEmployed")),
            }
        );
        static partial void ChangedCore(global::System.String propertyName, ref global::System.ComponentModel.PropertyChangedEventArgs? result);
        public static global::System.ComponentModel.PropertyChangedEventArgs Changed(global::System.String? propertyName)
        {
            if (global::System.Object.ReferenceEquals(propertyName, null))
            {
                return _nullChanged;
            }
            global::System.ComponentModel.PropertyChangedEventArgs? result = null;
            ChangedCore(propertyName, ref result);
            if (!global::System.Object.ReferenceEquals(result, null))
            {
                return result;
            }
            if (_lookupChanged.TryGetValue(propertyName, out result))
            {
                return result;
            }
            return new global::System.ComponentModel.PropertyChangedEventArgs(propertyName);
        }
        static partial void ChangingCore(global::System.String propertyName, ref global::System.ComponentModel.PropertyChangingEventArgs? result);
        public static global::System.ComponentModel.PropertyChangingEventArgs Changing(global::System.String? propertyName)
        {
            if (global::System.Object.ReferenceEquals(propertyName, null))
            {
                return _nullChanging;
            }
            global::System.ComponentModel.PropertyChangingEventArgs? result = null;
            ChangingCore(propertyName, ref result);
            if (!global::System.Object.ReferenceEquals(result, null))
            {
                return result;
            }
            if (_lookupChanging.TryGetValue(propertyName, out result))
            {
                return result;
            }
            return new global::System.ComponentModel.PropertyChangingEventArgs(propertyName);
        }
    }
}
