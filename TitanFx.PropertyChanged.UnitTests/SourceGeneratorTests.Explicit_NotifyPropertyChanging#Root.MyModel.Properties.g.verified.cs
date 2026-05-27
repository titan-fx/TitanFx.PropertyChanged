//HintName: Root.MyModel.Properties.g.cs
namespace Root
{
    partial class MyModel
    {
        public partial global::System.Guid Id { get; init => Set<global::System.Guid>(ref field, value, "Id"); }
        public partial string Name { get; set => Set<string>(ref field, value, "Name"); }
        public partial int Age { get; set => Set<int>(ref field, value, "Age"); }
        public partial bool IsEmployed { get; set => Set<bool>(ref field, value, "IsEmployed"); }
    }
}
