using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace TitanFx.PropertyChanged.Models;

internal sealed record TypeLocationCapture
{
    public required string? Namespace { get; init; }
    public required TypeHeaderCapture Header { get; init; }
    public required ValueArray<TypeHeaderCapture> ContainingTypes { get; init; }

    internal static TypeLocationCapture From(INamedTypeSymbol type)
    {
        return new()
        {
            Namespace = type.ContainingNamespace?.ToDisplayString(),
            ContainingTypes = new(
                GetContainingTypes(type).Select(TypeHeaderCapture.From).Reverse()
            ),
            Header = TypeHeaderCapture.From(type),
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
