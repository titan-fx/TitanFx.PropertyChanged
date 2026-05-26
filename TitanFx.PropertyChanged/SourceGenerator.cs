using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TitanFx.PropertyChanged;

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        AttributeGenerator.Initialize(context);
        var candidates = TypeCapture.Query(context);

        context.RegisterSourceOutput(candidates, ImplementationGenerator.GenerateSource);
    }
}

internal sealed record PropertyCapture
{
    public required string Name { get; init; }
    public required Accessibility? Setter { get; init; }
    public required string Type { get; init; }
    public required ValueArray<string> Dependents { get; init; }
    public required bool NotifyPropertyChanged { get; init; }
    public required bool NotifyPropertyChanging { get; init; }
    public required bool IsPartial { get; init; }
    public Accessibility Accessibility { get; internal set; }

    public static PropertyCapture From(IPropertySymbol property, CancellationToken token)
    {
        var attributes = property
            .GetAttributes()
            .ToLookup(a =>
                a.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            );
        return new()
        {
            Name = property.Name,
            Setter = property.SetMethod?.DeclaredAccessibility,
            Type = property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            Accessibility = property.DeclaredAccessibility,
            Dependents = new(
                attributes["global::TitanFx.PropertyChanged.NotifyAdditionalPropertiesAttribute"]
                    .SelectMany(a => a.ConstructorArguments)
                    .Select(v => v.Value)
                    .OfType<string>()
                    .Distinct()
            ),
            IsPartial = property.IsPartialDefinition,
            NotifyPropertyChanged = attributes[
                "global::TitanFx.PropertyChanged.NotifyPropertyChangedAttribute"
            ]
                .Any(),
            NotifyPropertyChanging = attributes[
                "global::TitanFx.PropertyChanged.NotifyPropertyChangingAttribute"
            ]
                .Any(),
        };
    }
}

internal sealed record TypeCapture
{
    public required string? Namespace { get; init; }
    public required TypeHeaderCapture Header { get; init; }
    public required ValueArray<TypeHeaderCapture> ContainingTypes { get; init; }
    public required ValueArray<PropertyCapture> Properties { get; init; }
    public required bool NotifyPropertyChanged { get; init; }
    public required bool NotifyPropertyChanging { get; init; }

    public static IncrementalValuesProvider<TypeCapture> Query(
        IncrementalGeneratorInitializationContext context
    )
    {
        var notifyPropertyChanged = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                "TitanFx.PropertyChanged.NotifyPropertyChangedAttribute",
                predicate: static (n, _) =>
                    n
                        is ClassDeclarationSyntax
                            or StructDeclarationSyntax
                            or RecordDeclarationSyntax
                            or PropertyDeclarationSyntax,
                transform: static (n, token) =>
                    From(
                        n.TargetSymbol switch
                        {
                            INamedTypeSymbol x => x,
                            IPropertySymbol x => x.ContainingType,
                            _ => (INamedTypeSymbol)n.TargetSymbol,
                        },
                        token
                    )
            )
            .Collect();
        var notifyPropertyChanging = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                "TitanFx.PropertyChanged.NotifyPropertyChangingAttribute",
                predicate: static (n, _) =>
                    n
                        is ClassDeclarationSyntax
                            or StructDeclarationSyntax
                            or RecordDeclarationSyntax
                            or PropertyDeclarationSyntax,
                transform: static (n, token) =>
                    From(
                        n.TargetSymbol switch
                        {
                            INamedTypeSymbol x => x,
                            IPropertySymbol x => x.ContainingType,
                            _ => (INamedTypeSymbol)n.TargetSymbol,
                        },
                        token
                    )
            )
            .Collect();
        var notifyAdditionalProperties = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                "TitanFx.PropertyChanged.NotifyAdditionalPropertiesAttribute",
                predicate: static (n, _) => n is PropertyDeclarationSyntax,
                transform: static (n, token) =>
                    From(((IPropertySymbol)n.TargetSymbol).ContainingType, token)
            )
            .Collect();

        return notifyPropertyChanged
            .Combine(notifyPropertyChanging)
            .Select(static (x, _) => new ValueArray<TypeCapture>([.. x.Left, .. x.Right]))
            .Combine(notifyAdditionalProperties)
            .Select(static (x, _) => new ValueArray<TypeCapture>([.. x.Left, .. x.Right]))
            .SelectMany(
                static (x, _) =>
                {
                    try
                    {
                        return x.GroupBy(
                            x => x.Header,
                            (k, g) =>
                                g.First() with
                                {
                                    NotifyPropertyChanged = g.Any(static x =>
                                        x.NotifyPropertyChanged
                                    ),
                                    NotifyPropertyChanging = g.Any(static x =>
                                        x.NotifyPropertyChanging
                                    ),
                                    Properties = new(
                                        g.SelectMany(static x => x.Properties)
                                            .GroupBy(static p => p.Name)
                                            .Select(static g =>
                                                g.First() with
                                                {
                                                    IsPartial = g.Any(static x => x.IsPartial),
                                                    Dependents = new(
                                                        g.SelectMany(static x => x.Dependents)
                                                            .Distinct()
                                                    ),
                                                    NotifyPropertyChanged = g.Any(static x =>
                                                        x.NotifyPropertyChanged
                                                    ),
                                                    NotifyPropertyChanging = g.Any(static x =>
                                                        x.NotifyPropertyChanging
                                                    ),
                                                }
                                            )
                                    ),
                                }
                        );
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString().Replace("\r", "").Replace("\n", ""));
                    }
                }
            );
    }

    public static TypeCapture From(INamedTypeSymbol type, CancellationToken token)
    {
        var attributes = type.GetAttributes()
            .ToLookup(a =>
                a.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            );

        return new()
        {
            Namespace = type.ContainingNamespace?.ToDisplayString(),
            ContainingTypes = new(
                GetContainingTypes(type).Select(v => TypeHeaderCapture.From(v, token))
            ),
            Header = TypeHeaderCapture.From(type, token),
            Properties = new(
                type.GetMembers()
                    .OfType<IPropertySymbol>()
                    .Select(s => PropertyCapture.From(s, token))
                    .Where(x => x is not null)
                    .Select(x => x!)
            ),
            NotifyPropertyChanged = attributes[
                "global::TitanFx.PropertyChanged.NotifyPropertyChangedAttribute"
            ]
                .Any(),
            NotifyPropertyChanging = attributes[
                "global::TitanFx.PropertyChanged.NotifyPropertyChangingAttribute"
            ]
                .Any(),
        };
    }

    private static IEnumerable<INamedTypeSymbol> GetContainingTypes(INamedTypeSymbol type)
    {
        while (type.ContainingType is { } container)
        {
            yield return container;
            type = container;
        }
    }
}

internal sealed record TypeHeaderCapture
{
    public required string Name { get; init; }
    public required ValueArray<string> TypeParameters { get; init; }
    public required string Kind { get; init; }

    public static TypeHeaderCapture From(INamedTypeSymbol type, CancellationToken token)
    {
        var kind = new List<string>();
        if (type.IsRefLikeType)
            kind.Add("ref");
        if (type.IsRecord)
            kind.Add("record");
        kind.Add(
            type.TypeKind switch
            {
                TypeKind.Interface => "interface",
                TypeKind.Class => "class",
                TypeKind.Struct => "struct",
                _ => "UNKNOWN",
            }
        );

        return new()
        {
            Name = type.Name,
            Kind = string.Join(' ', kind),
            TypeParameters = new(type.TypeParameters.Select(x => x.Name)),
        };
    }
}
