using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remora.Discord.Commands.Analyzers.Extensions;

internal static class INamedTypeSymbolExtensions
{
    private const string RemoraCommandGroupTypeName = "Remora.Commands.Groups.CommandGroup";
    private const string RemoraCommandAttributeTypeName = "Remora.Commands.Attributes.CommandAttribute";
    private const string RemoraCommandGroupAttributeTypeName = "Remora.Commands.Attributes.GroupAttribute";
    private const string RemoraExcludeFromSlashCommandsAttributeTypeName = "Remora.Discord.Commands.Attributes.ExcludeFromSlashCommandsAttribute";

    public static bool IsSlashCommandGroup(this INamedTypeSymbol classSymbol)
    {
        var isCommandGroup = classSymbol.BaseType?.ToDisplayString() == RemoraCommandGroupTypeName;

        var isAttributed = classSymbol.GetAttributes().Any
        (
            static a => a.AttributeClass?.ToDisplayString() == RemoraCommandGroupAttributeTypeName
        );
        
        var isExcluded = classSymbol.GetAttributes().Any
        (
            static a => a.AttributeClass?.ToDisplayString() == RemoraExcludeFromSlashCommandsAttributeTypeName
        );
        
        return isCommandGroup && isAttributed && !isExcluded;
    }

    public static bool IsSlashCommandMethod(this IMethodSymbol methodSymbol, out string name)
    {
        name = string.Empty; 
        
        if (methodSymbol.DeclaredAccessibility is not Accessibility.Public)
        {
            return false;
        }
        
        if (methodSymbol.IsStatic || methodSymbol.IsAbstract || methodSymbol.ReturnsVoid)
        {
            return false;
        }
        
        var attribute = methodSymbol.GetAttributes().FirstOrDefault
        (
            static a => a.AttributeClass?.ToDisplayString() == RemoraCommandAttributeTypeName
        );

        bool excluded = methodSymbol.GetAttributes().Any
        (
            static a => a.AttributeClass?.ToDisplayString() == RemoraExcludeFromSlashCommandsAttributeTypeName
        );

        if (excluded)
        {
            return false;
        }
        
        // If it's not on the method, check its parent
        var parent = methodSymbol.ContainingType;

        do
        {
            excluded = parent.GetAttributes().Any
            (
                static a => a.AttributeClass?.ToDisplayString() == RemoraExcludeFromSlashCommandsAttributeTypeName
            );

            if (excluded)
            {
                break;
            }

            parent = parent.ContainingType;
        } while (parent is not null);
        
        if (attribute is not null && !excluded)
        {
            name = attribute.ConstructorArguments[0].Value!.ToString();
            return true;
        }

        return false;
    }

    public static bool IsSlashSubCommnad(this IMethodSymbol methodSymbol) 
        => methodSymbol.IsSlashCommandMethod(out _) && methodSymbol.ContainingType.IsSlashCommandGroup();
}