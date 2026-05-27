using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TitanFx.PropertyChanged.Models;
using static TitanFx.PropertyChanged.Models.Constants;

namespace TitanFx.PropertyChanged;

internal static class PropertiesGenerator
{
    internal static void Initialize(
        IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<TypeCapture> types
    )
    {
        context.RegisterSourceOutput(
            types.Select(
                static (v, _) => new State { Location = v.Location, Properties = v.Properties }
            ),
            GenerateSource
        );
    }

    private record class State
    {
        public required TypeLocationCapture Location { get; init; }
        public required ValueArray<PropertyCapture> Properties { get; init; }
    }

    private static void GenerateSource(SourceProductionContext context, State type)
    {
        context.AddSource(Util.ToHintName(type.Location, "Properties"), ToSourceCode(type));
    }

    private static string ToSourceCode(State type)
    {
        var source = new StringWriter();
        var sourceGen = new IndentedTextWriter(source);

        using (PartialType.Write(sourceGen, type.Location))
        {
            foreach (var property in type.Properties)
                WriteProperty(sourceGen, property);
        }

        return source.ToString();
    }

    private static void WriteProperty(IndentedTextWriter sourceGen, PropertyCapture property)
    {
        WriteModifiers(sourceGen, property.Modifiers);
        sourceGen.Write($"{property.Type} {property.Name} {{ ");
        if (property.GetterModifiers is { } get)
        {
            WriteModifiers(sourceGen, get);
            sourceGen.Write("get; ");
        }
        if (property.SetterModifiers is { } set)
        {
            WriteModifiers(sourceGen, set);
            sourceGen.Write("set => ");
            WriteSetCall(sourceGen, property);
            sourceGen.Write("; ");
        }
        if (property.InitModifiers is { } init)
        {
            WriteModifiers(sourceGen, init);
            sourceGen.Write("init => ");
            WriteSetCall(sourceGen, property);
            sourceGen.Write("; ");
        }
        sourceGen.WriteLine("}");
    }

    private static void WriteSetCall(IndentedTextWriter sourceGen, PropertyCapture property)
    {
        var propertyNames = new List<string> { property.Name };
        propertyNames.AddRange(property.Dependents);
        var escapedNames = propertyNames
            .Distinct()
            .Select(static v => SymbolDisplay.FormatLiteral(v, quote: true));

        if (property.IsRefStruct)
        {
            sourceGen.Write(
                $"{Set}<{property.Type}>(field == value, ref field, value, {string.Join(", ", escapedNames)})"
            );
        }
        else
        {
            sourceGen.Write(
                $"{Set}<{property.Type}>(ref field, value, {string.Join(", ", escapedNames)})"
            );
        }
    }

    private static void WriteModifiers(IndentedTextWriter sourceGen, string? modifiers)
    {
        if (modifiers is not { Length: > 0 })
            return;
        sourceGen.Write(modifiers);
        sourceGen.Write(" ");
    }
}
