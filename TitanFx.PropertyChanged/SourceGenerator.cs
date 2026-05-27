using System.Linq;
using Microsoft.CodeAnalysis;
using TitanFx.PropertyChanged.Models;

namespace TitanFx.PropertyChanged;

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        AttributeGenerator.Initialize(context);
        var types = TypeCapture.Query(context);
        var declared = types
            .Where(static t => t.ImplicitDeclared || t.ExplicitDeclared)
            .Select(
                static (v, _) =>
                    v with
                    {
                        Properties = new(
                            v.Properties.Where(p =>
                                p.IsPartial
                                && (p.SetterModifiers is not null || p.InitModifiers is not null)
                                && !p.ExplicitIsDisabled
                                && (p.ExplicitIsEnabled || v.ExplicitDeclared)
                            )
                        ),
                    }
            );
        ImplementationGenerator.Initialize(context, declared);
        PropertiesGenerator.Initialize(context, declared);
        EventArgsGenerator.Initialize(context, declared);
    }
}
