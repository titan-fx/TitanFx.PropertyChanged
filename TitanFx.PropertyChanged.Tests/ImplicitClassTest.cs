using System.ComponentModel;

namespace TitanFx.PropertyChanged.Tests;

public partial class ImplicitClassTest : INotifyPropertyChanged, INotifyPropertyChanging
{
    public required string Unreactive1 { get; set; }
    public string Unreactive2 => Unreactive1;

    [NotifyPropertyChange]
    public partial string Unreactive3 { get; set; }

    public string ReactiveDerived => $"ReactiveDerived.{ReactiveSource}";

    [NotifyPropertyChange(Affects = [nameof(ReactiveDerived)])]
    public required partial string ReactiveSource { get; set; }

    [NotifyPropertyChange]
    public required partial string ReactiveSet { get; set; }

    [NotifyPropertyChange]
    public required partial string ReactiveInit { get; init; }

    [NotifyPropertyChange]
    public required partial string ReactivePublicSet { get; set; }

    [NotifyPropertyChange]
    public required partial string ReactivePublicInit { get; init; }

    [NotifyPropertyChange]
    public partial string? ReactivePrivateSet { get; private set; }

    [NotifyPropertyChange]
    public partial string? ReactivePrivateInit { get; private init; }

    [NotifyPropertyChange]
    public partial string? ReactiveInternalSet { get; internal set; }

    [NotifyPropertyChange]
    public partial string? ReactiveInternalInit { get; internal init; }

    [NotifyPropertyChange]
    public partial string? ReactiveProtectedInternalSet { get; protected internal set; }

    [NotifyPropertyChange]
    public partial string? ReactiveProtectedInternalInit { get; protected internal init; }

    [NotifyPropertyChange]
    public partial string? ReactivePrivateProtectedSet { get; private protected set; }

    [NotifyPropertyChange]
    public partial string? ReactivePrivateProtectedInit { get; private protected init; }

    [NotifyPropertyChange]
    private partial string PrivateReactiveSet { get; set; }

    [NotifyPropertyChange]
    private partial string PrivateReactiveInit { get; init; }

    [NotifyPropertyChange]
    protected partial string ProtectedReactiveSet { get; set; }

    [NotifyPropertyChange]
    protected partial string ProtectedReactiveInit { get; init; }

    [NotifyPropertyChange]
    internal partial string InternalReactiveSet { get; set; }

    [NotifyPropertyChange]
    internal partial string InternalReactiveInit { get; init; }

    [NotifyPropertyChange]
    protected internal partial string? ProtectedInternalReactiveSet { get; set; }

    [NotifyPropertyChange]
    protected internal partial string? ProtectedInternalReactiveInit { get; init; }

    [NotifyPropertyChange]
    private protected partial string? PrivateProtectedReactiveSet { get; set; }

    [NotifyPropertyChange]
    private protected partial string? PrivateProtectedReactiveInit { get; init; }
}

partial class ImplicitClassTest
{
    public partial string Unreactive3
    {
        get;
        set => field = value;
    } = "";
}
