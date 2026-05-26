namespace TitanFx.PropertyChanged.Tests;

[NotifyPropertyChanged, NotifyPropertyChanging]
public sealed partial class NotifyEverything
{
    public partial Guid Id { get; set; }
    public partial string? Name { get; set; }
    public partial int Age { get; set; }
    public partial bool IsEmployed { get; set; }
}

public sealed partial class NotifyIndividualProperties
{
    public partial Guid Id { get; set; }

    [NotifyPropertyChanged, NotifyPropertyChanging]
    public partial string? Name { get; set; }

    [NotifyPropertyChanged]
    public partial int Age { get; set; }

    [NotifyPropertyChanging]
    public partial bool IsEmployed { get; set; }
}
