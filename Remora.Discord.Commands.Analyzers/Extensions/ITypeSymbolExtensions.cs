using Microsoft.CodeAnalysis;

namespace Remora.Discord.Commands.Analyzers.Extensions;

public static class ITypeSymbolExtensions
{
    public static bool IsCollection(this ITypeSymbol type) 
        => type.AllInterfaces.Any(x => x.SpecialType == SpecialType.System_Collections_IEnumerable);
}