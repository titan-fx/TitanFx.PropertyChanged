using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static TitanFx.PropertyChanged.Models.Constants;

namespace TitanFx.PropertyChanged.Models;

internal sealed record PropertyCapture
{
    public required string Name { get; init; }
    public required string? SetterModifiers { get; init; }
    public required string? InitModifiers { get; init; }
    public required string? GetterModifiers { get; init; }
    public required string Type { get; init; }
    public required ValueArray<string> Dependents { get; init; }
    public required bool IsRefStruct { get; init; }
    public required bool ExplicitIsEnabled { get; init; }
    public required bool ExplicitIsDisabled { get; init; }
    public required bool IsPartial { get; init; }
    public required string Modifiers { get; internal set; }

    public static PropertyCapture From(IPropertySymbol property, CancellationToken token)
    {
        var attributes = property
            .GetAttributes()
            .ToLookup(static a =>
                a.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            );
        var isDisabled = attributes[Types.NotifyPropertyChangeAttribute]
            .SelectMany(static x =>
                x.NamedArguments.Where(static a => a.Key is Skip)
                    .Select(static kvp => kvp.Value.Value)
                    .OfType<bool>()
                    .DefaultIfEmpty(false)
            );
        var syntax = property
            .DeclaringSyntaxReferences.Select(x => x.GetSyntax(token))
            .OfType<PropertyDeclarationSyntax>()
            .ToList();

        return new()
        {
            Name = property.Name,
            Type = property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            IsRefStruct = property.Type is { IsValueType: true, IsRefLikeType: true },
            SetterModifiers = GetModifiers(syntax, SyntaxKind.SetAccessorDeclaration),
            InitModifiers = GetModifiers(syntax, SyntaxKind.InitAccessorDeclaration),
            GetterModifiers = GetModifiers(syntax, SyntaxKind.GetAccessorDeclaration),
            Modifiers = GetModifiers(syntax),
            Dependents = new(
                attributes[Types.NotifyPropertyChangeAttribute]
                    .SelectMany(static x =>
                        x.NamedArguments.Where(static a => a.Key is AffectsProp)
                            .Select(static kvp => ValueRaw(kvp.Value))
                            .OfType<ImmutableArray<TypedConstant>>()
                            .SelectMany(static x => x)
                            .Select(static v => v.Value)
                            .OfType<string>()
                    )
                    .Distinct()
            ),
            IsPartial = property.IsPartialDefinition && property.PartialImplementationPart is null,
            ExplicitIsEnabled = isDisabled.Any(static v => !v),
            ExplicitIsDisabled = isDisabled.Any(static v => v),
        };
    }

    private static string GetModifiers(IReadOnlyCollection<PropertyDeclarationSyntax> property)
    {
        if (property.Count == 0)
            return "";

        return GetModifiers(property.SelectMany(static x => x.Modifiers));
    }

    private static string? GetModifiers(
        IReadOnlyCollection<PropertyDeclarationSyntax> property,
        SyntaxKind accessor
    )
    {
        if (property.Count == 0)
            return null;

        var accessors = property
            .SelectMany(static x => x.AccessorList?.Accessors ?? [])
            .Where(x => x.Kind() == accessor)
            .ToList();
        if (accessors.Count == 0)
            return null;

        return GetModifiers(accessors.SelectMany(static x => x.Modifiers));
    }

    private static string GetModifiers(IEnumerable<SyntaxToken> modifiers)
    {
        return string.Join(' ', modifiers.DistinctBy(x => x.Kind()).Select(x => x.ToString()));
    }

    private static object? ValueRaw(TypedConstant constant)
    {
        return constant.Kind is TypedConstantKind.Array ? constant.Values : constant.Value;
    }
}
