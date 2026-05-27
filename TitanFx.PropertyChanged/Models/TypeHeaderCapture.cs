using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TitanFx.PropertyChanged.Models;

internal sealed record TypeHeaderCapture
{
    public required string Name { get; init; }
    public required ValueArray<string> TypeParameters { get; init; }
    public required string Kind { get; init; }
    public required string Modifiers { get; init; }

    public static TypeHeaderCapture From(INamedTypeSymbol type)
    {
        return new()
        {
            Name = type.Name,
            Kind = type switch
            {
                { TypeKind: TypeKind.Interface } => "interface",
                { TypeKind: TypeKind.Struct, IsRecord: true } => "record struct",
                { TypeKind: TypeKind.Struct, IsRecord: false } => "struct",
                { TypeKind: TypeKind.Class, IsRecord: true } => "record class",
                { TypeKind: TypeKind.Class, IsRecord: false } => "class",
                _ => "UNKNOWN",
            },
            Modifiers = type.IsRefLikeType ? "ref" : "",
            TypeParameters = new(type.TypeParameters.Select(static x => x.Name)),
        };
    }
}
