using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using TitanFx.PropertyChanged.Models;

namespace TitanFx.PropertyChanged;

internal static class PartialType
{
    public static IDisposable Write(
        IndentedTextWriter sourceGen,
        TypeLocationCapture type,
        Action? writeConstraints = null
    )
    {
        var scope = new List<IDisposable>();
        if (type.Namespace is { } ns)
            scope.Add(Util.WriteNamespace(sourceGen, ns));

        foreach (var container in type.ContainingTypes)
        {
            WriteTypeHeader(sourceGen, container);
            sourceGen.WriteLine();
            scope.Add(Util.WriteBlock(sourceGen));
        }

        WriteTypeHeader(sourceGen, type.Header);
        var indent = sourceGen.Indent;
        writeConstraints?.Invoke();
        sourceGen.Indent = indent;
        sourceGen.WriteLine();
        scope.Add(Util.WriteBlock(sourceGen));

        return new Scope(scope);
    }

    private static void WriteTypeHeader(IndentedTextWriter sourceGen, TypeHeaderCapture type)
    {
        if (type.Modifiers is not "")
            sourceGen.Write($"{type.Modifiers} ");
        sourceGen.Write($"partial {type.Kind} {type.Name}");
        if (type.TypeParameters.Count > 0)
        {
            sourceGen.Write("<");
            foreach (var typeParameter in type.TypeParameters.SkipLast(1))
            {
                sourceGen.Write(typeParameter);
                sourceGen.Write(", ");
            }
            sourceGen.Write(type.TypeParameters[^1]);
            sourceGen.Write(">");
        }
    }

    private sealed class Scope(IReadOnlyList<IDisposable> scopes) : IDisposable
    {
        public void Dispose()
        {
            for (var i = scopes.Count - 1; i >= 0; i--)
                scopes[i].Dispose();
        }
    }
}
