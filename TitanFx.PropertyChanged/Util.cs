using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Text;
using Microsoft.CodeAnalysis;
using TitanFx.PropertyChanged.Models;

namespace TitanFx.PropertyChanged;

internal static class Util
{
    public static string ToSource(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.Public => "public",
            _ => "",
        };
    }

    public static string ToHintName(TypeLocationCapture type, string kind)
    {
        var hintBuilder = new StringBuilder();
        if (type.Namespace is { } ns)
            _ = hintBuilder.Append($"{ns}.");
        foreach (var container in type.ContainingTypes)
            Append(hintBuilder, container);
        Append(hintBuilder, type.Header);
        _ = hintBuilder.Append($"{kind}.g.cs");
        return hintBuilder.ToString();

        static void Append(StringBuilder builder, TypeHeaderCapture type)
        {
            _ = builder.Append(type.Name);
            if (type.TypeParameters.Count > 0)
                builder.Append($"`{type.TypeParameters.Count}");
            builder.Append('.');
        }
    }

    public static IDisposable WriteTry(IndentedTextWriter writer)
    {
        writer.WriteLine("try");
        return WriteBlock(writer);
    }

    public static IDisposable WriteFinally(IndentedTextWriter writer)
    {
        writer.WriteLine("finally");
        return WriteBlock(writer);
    }

    public static IDisposable WriteCatch(IndentedTextWriter writer, string? exception = null)
    {
        writer.Write("catch");
        if (exception is not null)
            writer.Write($"({exception})");
        writer.WriteLine();
        return WriteBlock(writer);
    }

    public static IDisposable WriteIf(IndentedTextWriter writer, string condition)
    {
        writer.WriteLine($"if ({condition})");
        return WriteBlock(writer);
    }

    public static IDisposable WriteForeach(IndentedTextWriter writer, string value, string source)
    {
        writer.WriteLine($"foreach (var {value} in {source})");
        return WriteBlock(writer);
    }

    public static IDisposable WriteNamespace(IndentedTextWriter writer, string @namespace)
    {
        writer.WriteLine($"namespace {@namespace}");
        return WriteBlock(writer);
    }

    public static IDisposable WriteBlock(IndentedTextWriter writer)
    {
        writer.WriteLine("{");
        writer.Indent++;

        return new CloseBlock(writer);
    }

    private sealed class CloseBlock(IndentedTextWriter writer) : IDisposable
    {
        public void Dispose()
        {
            writer.Indent--;
            writer.WriteLine("}");
        }
    }
}
