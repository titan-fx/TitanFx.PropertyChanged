using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static TitanFx.PropertyChanged.Models.Constants;

namespace TitanFx.PropertyChanged.Models;

internal sealed record TypeCapture
{
    public required TypeLocationCapture Location { get; init; }
    public required ValueArray<PropertyCapture> Properties { get; init; }
    public required bool ImplicitNotifyPropertyChanged { get; init; }
    public required bool ImplicitNotifyPropertyChanging { get; init; }
    public required bool ExplicitNotifyPropertyChanged { get; init; }
    public required bool ExplicitNotifyPropertyChanging { get; init; }
    public required bool IsRefStruct { get; init; }

    public bool NotifyPropertyChanged =>
        ImplicitNotifyPropertyChanged || ExplicitNotifyPropertyChanged;
    public bool NotifyPropertyChanging =>
        ImplicitNotifyPropertyChanging || ExplicitNotifyPropertyChanging;
    public bool ImplicitDeclared => ImplicitNotifyPropertyChanged || ImplicitNotifyPropertyChanging;
    public bool ExplicitDeclared => ExplicitNotifyPropertyChanged || ExplicitNotifyPropertyChanging;

    public static IncrementalValuesProvider<TypeCapture> Query(
        IncrementalGeneratorInitializationContext context
    )
    {
        var notifyPropertyChanged = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                Types.NotifyPropertyChangedAttribute["global::".Length..],
                predicate: static (n, _) =>
                    n
                        is ClassDeclarationSyntax
                            or StructDeclarationSyntax
                            or RecordDeclarationSyntax,
                transform: static (n, token) => From((INamedTypeSymbol)n.TargetSymbol, token)
            )
            .Collect();
        var notifyPropertyChanging = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                Types.NotifyPropertyChangingAttribute["global::".Length..],
                predicate: static (n, _) =>
                    n
                        is ClassDeclarationSyntax
                            or StructDeclarationSyntax
                            or RecordDeclarationSyntax,
                transform: static (n, token) => From((INamedTypeSymbol)n.TargetSymbol, token)
            )
            .Collect();
        var notifyPropertyChange = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                Types.NotifyPropertyChangeAttribute["global::".Length..],
                predicate: static (n, _) => n is PropertyDeclarationSyntax,
                transform: static (n, token) => From((IPropertySymbol)n.TargetSymbol, token)
            )
            .Collect();

        return notifyPropertyChanged
            .Combine(notifyPropertyChanging)
            .Select(static (x, _) => new ValueArray<TypeCapture>([.. x.Left, .. x.Right]))
            .Combine(notifyPropertyChange)
            .Select(static (x, _) => new ValueArray<TypeCapture>([.. x.Left, .. x.Right]))
            .SelectMany(
                static (x, _) =>
                {
                    try
                    {
                        return x.GroupBy(
                            static x => x.Location,
                            static (k, g) =>
                                g.First() with
                                {
                                    ImplicitNotifyPropertyChanged = g.Any(static x =>
                                        x.ImplicitNotifyPropertyChanged
                                    ),
                                    ImplicitNotifyPropertyChanging = g.Any(static x =>
                                        x.ImplicitNotifyPropertyChanging
                                    ),
                                    ExplicitNotifyPropertyChanged = g.Any(static x =>
                                        x.ExplicitNotifyPropertyChanged
                                    ),
                                    ExplicitNotifyPropertyChanging = g.Any(static x =>
                                        x.ExplicitNotifyPropertyChanging
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
                                                    ExplicitIsEnabled = g.Any(static x =>
                                                        x.ExplicitIsEnabled
                                                    ),
                                                    ExplicitIsDisabled = g.Any(static x =>
                                                        x.ExplicitIsDisabled
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

    public static TypeCapture From(IPropertySymbol property, CancellationToken token)
    {
        var type = property.ContainingType;
        var interfaces = type.AllInterfaces.ToLookup(static i =>
            i.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
        );
        return new()
        {
            Location = TypeLocationCapture.From(type),
            Properties = new(new[] { property }.Select(v => PropertyCapture.From(v, token))),
            ImplicitNotifyPropertyChanged = interfaces[Types.INotifyPropertyChanged].Any(),
            ImplicitNotifyPropertyChanging = interfaces[Types.INotifyPropertyChanging].Any(),
            ExplicitNotifyPropertyChanged = false,
            ExplicitNotifyPropertyChanging = false,
            IsRefStruct = type is { IsValueType: true, IsRefLikeType: true },
        };
    }

    public static TypeCapture From(INamedTypeSymbol type, CancellationToken token)
    {
        var attributes = type.GetAttributes()
            .ToLookup(static a =>
                a.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            );

        return new()
        {
            Location = TypeLocationCapture.From(type),
            Properties = new(
                type.GetMembers()
                    .OfType<IPropertySymbol>()
                    .Select(v => PropertyCapture.From(v, token))
            ),
            ImplicitNotifyPropertyChanged = false,
            ImplicitNotifyPropertyChanging = false,
            ExplicitNotifyPropertyChanged = attributes[Types.NotifyPropertyChangedAttribute].Any(),
            ExplicitNotifyPropertyChanging = attributes[Types.NotifyPropertyChangingAttribute]
                .Any(),
            IsRefStruct = type is { IsValueType: true, IsRefLikeType: true },
        };
    }
}
