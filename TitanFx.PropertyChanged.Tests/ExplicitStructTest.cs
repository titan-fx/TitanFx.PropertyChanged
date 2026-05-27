namespace TitanFx.PropertyChanged.Tests;

[NotifyPropertyChanged]
#pragma warning disable CS0282 // There is no defined ordering between fields in multiple declarations of partial struct
public partial struct ExplicitStructTest()
#pragma warning restore CS0282 // There is no defined ordering between fields in multiple declarations of partial struct
{
    public required string Unreactive1 { get; set; }
    public readonly string Unreactive2 => Unreactive1;

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
    private partial string PrivateReactiveSet { get; set; }
    private partial string PrivateReactiveInit { get; init; }
    internal partial string InternalReactiveSet { get; set; }
    internal partial string InternalReactiveInit { get; init; }
}

partial struct ExplicitStructTest
{
    public partial string Unreactive3
    {
        get;
        set => field = value;
    } = "";
}
