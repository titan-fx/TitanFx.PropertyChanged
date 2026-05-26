using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TitanFx.PropertyChanged.UnitTests;

public class UnitTest1
{
    [Fact]
    public async Task Test1Async()
    {
        var token = TestContext.Current.CancellationToken;
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            using TitanFx.PropertyChanged;

            namespace Root;

            [NotifyPropertyChanged, NotifyPropertyChanging]
            public sealed partial class NotifyEverything
            {
                public Guid Id { get; }
                public string? Name { get; set; }
                public int Age { get; set; }
                public bool IsEmployed { get; set; }
            }

            public sealed partial class NotifyIndividualProperties
            {
                public Guid Id { get; }

                [NotifyPropertyChanged, NotifyPropertyChanging]
                public string? Name { get; set; }

                [NotifyPropertyChanged]
                public int Age { get; set; }

                [NotifyPropertyChanging]
                public bool IsEmployed { get; set; }
            }

            """,
            cancellationToken: token
        );
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        };
        var compilation = CSharpCompilation.Create(
            assemblyName: "tests",
            syntaxTrees: new[] { syntaxTree },
            references: references
        );
        var generator = new SourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        var result = driver.RunGenerators(compilation, token);
        _ = await Verify(result);
    }
}
