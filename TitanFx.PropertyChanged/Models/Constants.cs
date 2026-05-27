namespace TitanFx.PropertyChanged.Models;

internal static class Constants
{
    public const string CodeAnalysisNamespace = "Microsoft.CodeAnalysis";
    public const string RootNamespace = "TitanFx.PropertyChanged";
    public const string NotifyPropertyChanged = $"{nameof(NotifyPropertyChanged)}Attribute";
    public const string NotifyPropertyChanging = $"{nameof(NotifyPropertyChanging)}Attribute";
    public const string NotifyPropertyChange = $"{nameof(NotifyPropertyChange)}Attribute";
    public const string Embedded = $"{nameof(Embedded)}Attribute";
    public const string PropertyChangeEventArgs = nameof(PropertyChangeEventArgs);
    public const string Setter = nameof(Setter);
    public const string Skip = "Skip";
    public const string AffectsProp = "Affects";
    public const string AffectsField = "_affects";
    public const string Set = "Set";

    public static class Types
    {
        public const string String = "global::System.String";
        public const string Boolean = "global::System.Boolean";
        public const string Object = "global::System.Object";
        public const string Attribute = "global::System.Attribute";
        public const string AttributeUsage = "global::System.AttributeUsageAttribute";
        public const string AttributeTargets = "global::System.AttributeTargets";
        public const string INotifyPropertyChanged =
            "global::System.ComponentModel.INotifyPropertyChanged";
        public const string INotifyPropertyChanging =
            "global::System.ComponentModel.INotifyPropertyChanging";
        public const string PropertyChangedEventHandler =
            "global::System.ComponentModel.PropertyChangedEventHandler";
        public const string PropertyChangedEventArgs =
            "global::System.ComponentModel.PropertyChangedEventArgs";
        public const string PropertyChangingEventHandler =
            "global::System.ComponentModel.PropertyChangingEventHandler";
        public const string PropertyChangingEventArgs =
            "global::System.ComponentModel.PropertyChangingEventArgs";
        public const string CallerMemberName =
            "global::System.Runtime.CompilerServices.CallerMemberNameAttribute";
        public const string ReadOnlySpan = "global::System.ReadOnlySpan";
        public const string Exception = "global::System.Exception";
        public const string ExceptionDispatchInfo =
            "global::System.Runtime.ExceptionServices.ExceptionDispatchInfo";
        public const string EqualityComparer =
            "global::System.Collections.Generic.EqualityComparer";
        public const string FrozenDictionary = "global::System.Collections.Frozen.FrozenDictionary";
        public const string KeyValuePair = "global::System.Collections.Generic.KeyValuePair";

        public const string Setter = $"global::{RootNamespace}.{Constants.Setter}";
        public const string NotifyPropertyChangedAttribute =
            $"global::{RootNamespace}.{NotifyPropertyChanged}";
        public const string NotifyPropertyChangingAttribute =
            $"global::{RootNamespace}.{NotifyPropertyChanging}";
        public const string NotifyPropertyChangeAttribute =
            $"global::{RootNamespace}.{NotifyPropertyChange}";
        public const string EmbeddedAttribute = $"global::{CodeAnalysisNamespace}.{Embedded}";
        public const string PropertyChangeEventArgs =
            $"global::{RootNamespace}.{Constants.PropertyChangeEventArgs}";
    }
}
