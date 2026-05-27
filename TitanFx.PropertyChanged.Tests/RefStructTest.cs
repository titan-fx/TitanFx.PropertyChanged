using System;
using System.Collections.Generic;
using System.Text;

namespace TitanFx.PropertyChanged.Tests;

[NotifyPropertyChanged, NotifyPropertyChanging]
public ref partial struct RefStructTest
{
    public partial ReadOnlySpan<char> Name { get; set; }
}
