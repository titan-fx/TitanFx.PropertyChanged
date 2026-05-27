using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TitanFx.PropertyChanged.UnitTests;

public class SourceGeneratorTests
{
    [Fact]
    public async Task Explicit_Both()
    {
        var result = AssertGenerator(
            """
            using System;
            using TitanFx.PropertyChanged;

            namespace Root;

            [NotifyPropertyChanged, NotifyPropertyChanging]
            public sealed partial class MyModel
            {
                public partial Guid Id { get; init; }
                public partial string? Name { get; set; }
                public partial int Age { get; set; }
                public partial bool IsEmployed { get; set; }
            }

            """,
            TestContext.Current.CancellationToken
        );

        await Verify(result);
    }

    [Fact]
    public async Task Explicit_NotifyPropertyChanged()
    {
        var result = AssertGenerator(
            """
            using System;
            using TitanFx.PropertyChanged;

            namespace Root;

            [NotifyPropertyChanged]
            public sealed partial class MyModel
            {
                public partial Guid Id { get; init; }
                public partial string? Name { get; set; }
                public partial int Age { get; set; }
                public partial bool IsEmployed { get; set; }
            }

            """,
            TestContext.Current.CancellationToken
        );
        await Verify(result);
    }

    [Fact]
    public async Task Explicit_NotifyPropertyChanging()
    {
        var result = AssertGenerator(
            """
            using System;
            using TitanFx.PropertyChanged;

            namespace Root;

            [NotifyPropertyChanging]
            public sealed partial class MyModel
            {
                [NotifyPropertyChange(IsDisabled = true)]
                public partial Guid Id { get; init; }
                public partial string? Name { get; set; }
                public partial int Age { get; set; }
                public partial bool IsEmployed { get; set; }
            }

            """,
            TestContext.Current.CancellationToken
        );
        await Verify(result);
    }

    [Fact]
    public async Task Explicit_None()
    {
        var result = AssertGenerator(
            """
            using System;
            using TitanFx.PropertyChanged;

            namespace Root;

            public sealed partial class MyModel
            {
                [NotifyPropertyChange(IsDisabled = true)]
                public partial Guid Id { get; init; }
                public partial string? Name { get; set; }
                public partial int Age { get; set; }
                public partial bool IsEmployed { get; set; }
            }

            """,
            TestContext.Current.CancellationToken
        );
        await Verify(result);
    }

    [Fact]
    public async Task Implicit_None()
    {
        var result = AssertGenerator(
            """
            using System;
            using TitanFx.PropertyChanged;
            using System.ComponentModel;

            namespace Root;

            public sealed partial class MyModel
            {
                public partial Guid Id { get; init; }
                [NotifyPropertyChange]
                public partial string? Name { get; set; }
                [NotifyPropertyChange]
                public partial int Age { get; set; }
                [NotifyPropertyChange]
                public partial bool IsEmployed { get; set; }
            }

            """,
            TestContext.Current.CancellationToken
        );
        await Verify(result);
    }

    [Fact]
    public async Task Implicit_Both()
    {
        var result = AssertGenerator(
            """
            using System;
            using TitanFx.PropertyChanged;
            using System.ComponentModel;

            namespace Root;

            public sealed partial class MyModel 
                : INotifyPropertyChanged, 
                  INotifyPropertyChanging
            {
                public partial Guid Id { get; init; }
                [NotifyPropertyChange]
                public partial string? Name { get; set; }
                [NotifyPropertyChange]
                public partial int Age { get; set; }
                [NotifyPropertyChange]
                public partial bool IsEmployed { get; set; }
            }

            """,
            TestContext.Current.CancellationToken
        );
        await Verify(result);
    }

    [Fact]
    public async Task Implicit_NotifyPropertyChanged()
    {
        var result = AssertGenerator(
            """
            using System;
            using TitanFx.PropertyChanged;
            using System.ComponentModel;

            namespace Root;

            public sealed partial class MyModel 
                : INotifyPropertyChanged
            {
                public partial Guid Id { get; init; }
                [NotifyPropertyChange]
                public partial string? Name { get; set; }
                [NotifyPropertyChange]
                public partial int Age { get; set; }
                [NotifyPropertyChange]
                public partial bool IsEmployed { get; set; }
            }

            """,
            TestContext.Current.CancellationToken
        );
        await Verify(result);
    }

    [Fact]
    public async Task Implicit_NotifyPropertyChanging()
    {
        var result = AssertGenerator(
            """
            using System;
            using TitanFx.PropertyChanged;
            using System.ComponentModel;

            namespace Root;

            public sealed partial class MyModel 
                : INotifyPropertyChanging
            {
                public partial Guid Id { get; init; }
                [NotifyPropertyChange]
                public partial string? Name { get; set; }
                [NotifyPropertyChange]
                public partial int Age { get; set; }
                [NotifyPropertyChange]
                public partial bool IsEmployed { get; set; }
            }

            """,
            TestContext.Current.CancellationToken
        );
        await Verify(result);
    }

    [Fact]
    public async Task Nested()
    {
        var result = AssertGenerator(
            """
            using System;
            using TitanFx.PropertyChanged;

            namespace Root;

            public partial interface IContainer0
            {
                public partial struct Container1
                {
                    public partial class Container2
                    {
                        [NotifyPropertyChanged, NotifyPropertyChanging]
                        public sealed partial class MyModel
                        {
                            [NotifyPropertyChange(IsDisabled = true)]
                            public partial Guid Id { get; init; }
                            public partial string? Name { get; set; }
                            public partial int Age { get; set; }
                            public partial bool IsEmployed { get; set; }
                        }
                    }
                }
            }

            """,
            TestContext.Current.CancellationToken
        );

        await Verify(result);
    }

    [Fact]
    public async Task RefStruct()
    {
        var result = AssertGenerator(
            """
            using System;
            using TitanFx.PropertyChanged;

            namespace Root;

            [NotifyPropertyChanged, NotifyPropertyChanging]
            public ref partial struct RefStructTest
            {
                public partial ReadOnlySpan<char> Name { get; set; }
            }

            """,
            TestContext.Current.CancellationToken
        );

        await Verify(result);
    }

    private static GeneratorDriverRunResult AssertGenerator(
        string source,
        CancellationToken cancellationToken
    )
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, cancellationToken: cancellationToken);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(INotifyPropertyChanging).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            assemblyName: "tests",
            syntaxTrees: [syntaxTree],
            references: references
        );
        var generator = new SourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        var result = driver.RunGenerators(compilation, cancellationToken);
        return result.GetRunResult();
    }
}
