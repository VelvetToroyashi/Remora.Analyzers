using Microsoft.CodeAnalysis;

namespace Remora.Discord.Commands.Analyzers.Extensions;

public static class ITypeSymbolExtensions
{
    public static bool IsCollection(this ITypeSymbol type, Compilation compilation)
    {
        // Special case: strings
        if (type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        var enumerableType = compilation.GetTypeByMetadataName(typeof(IEnumerable<>).FullName!);
        
        var interfaces = type.AllInterfaces;
        
        return interfaces.Any
        (
            i => i.OriginalDefinition.Equals(enumerableType, SymbolEqualityComparer.Default)
        );
    }
}