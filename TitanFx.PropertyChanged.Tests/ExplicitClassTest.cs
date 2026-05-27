namespace TitanFx.PropertyChanged.Tests;

[NotifyPropertyChanged, NotifyPropertyChanging]
public partial class ExplicitClassTest
{
    public required string Unreactive1 { get; set; }
    public string Unreactive2 => Unreactive1;

    [NotifyPropertyChange(Skip = true)]
    public partial string Unreactive3 { get; set; }

    public string ReactiveDerived => $"ReactiveDerived.{ReactiveSource}";

    [NotifyPropertyChange(Affects = [nameof(ReactiveDerived)])]
    public required partial string ReactiveSource { get; set; }
    public required partial string ReactiveSet { get; set; }
    public required partial string ReactiveInit { get; init; }
    public required partial string ReactivePublicSet { get; set; }
    public required partial string ReactivePublicInit { get; init; }
    public partial string? ReactivePrivateSet { get; private set; }
    public partial string? ReactivePrivateInit { get; private init; }
    public partial string? ReactiveInternalSet { get; internal set; }
    public partial string? ReactiveInternalInit { get; internal init; }
    public partial string? ReactiveProtectedInternalSet { get; protected internal set; }
    public partial string? ReactiveProtectedInternalInit { get; protected internal init; }
    public partial string? ReactivePrivateProtectedSet { get; private protected set; }
    public partial string? ReactivePrivateProtectedInit { get; private protected init; }
    private partial string PrivateReactiveSet { get; set; }
    private partial string PrivateReactiveInit { get; init; }
    protected partial string ProtectedReactiveSet { get; set; }
    protected partial string ProtectedReactiveInit { get; init; }
    internal partial string InternalReactiveSet { get; set; }
    internal partial string InternalReactiveInit { get; init; }
    protected internal partial string? ProtectedInternalReactiveSet { get; set; }
    protected internal partial string? ProtectedInternalReactiveInit { get; init; }
    private protected partial string? PrivateProtectedReactiveSet { get; set; }
    private protected partial string? PrivateProtectedReactiveInit { get; init; }
}

public partial class ExplicitClassTest
{
    public partial string Unreactive3
    {
        get;
        set => field = value;
    } = "";
}
