//HintName: Root.RefStructTest.Properties.g.cs
namespace Root
{
    ref partial struct RefStructTest
    {
        public partial global::System.ReadOnlySpan<char> Name { get; set => Set<global::System.ReadOnlySpan<char>>(field == value, ref field, value, "Name"); }
    }
}
