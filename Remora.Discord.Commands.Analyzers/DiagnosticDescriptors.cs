using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

[assembly: InternalsVisibleTo("REmora.Discord.Commands.Analyzers.Tests")]
namespace Remora.Discord.Commands.Analyzers;

internal static class DiagnosticDescriptors
{
    internal static readonly DiagnosticDescriptor RDC1001ContextMenuMissingTarget =
        new(
            "RDC1001",
            "Missing parameter",
            "Missing expected parameter of type '{0}'.",
            Categories.ContextMenus,
            DiagnosticSeverity.Error,
            true
        );

    internal static readonly DiagnosticDescriptor RDC1002ContextMenuTooManyParameters =
        new(
            "RDC1002",
            "Too many parameters",
            "The context menu has too many parameters.",
            Categories.ContextMenus,
            DiagnosticSeverity.Error,
            true
        );
    
    
    internal static readonly DiagnosticDescriptor RDC1003ContextMenuMismatchedTarget =
        new(
            "RDC1003",
            "Mismatched target",
            "The context menu target is mismatched with the command parameter. Got {0}, expected {1}.",
            Categories.ContextMenus,
            DiagnosticSeverity.Error,
            true
        );
    
    internal static readonly DiagnosticDescriptor RDC1004ContextMenuIncorrectParameterType =
        new(
            "RDC1004",
            "Incorrect parameter type",
            "The context menu parameter is of an incorrect type.  Got {0}, expected {1}.",
            Categories.ContextMenus,
            DiagnosticSeverity.Error,
            true
        );
    
    internal static readonly DiagnosticDescriptor RDC1005SlashCommandNestedTooDeeply =
        new(
            "RDC1005",
            "Slash command nested too deeply",
            "The slash command group is nested too deeply. Maximum depth is 2.",
            Categories.SlashCommands,
            DiagnosticSeverity.Error,
            true
        );
    
    internal static readonly DiagnosticDescriptor RDC1006SlashCommandNameTooLong =
        new(
            "RDC1006",
            "Slash command name too long",
            "The slash command name is too long. Maximum length is 32 characters.",
            Categories.SlashCommands,
            DiagnosticSeverity.Error,
            true
        );

    internal static readonly DiagnosticDescriptor RDC1007SwitchParameter =
        new(
            "RDC1007",
            "Switch parameter",
            "Switch parameters are not supported on slash commands.",
            Categories.SlashCommands,
            DiagnosticSeverity.Error,
            true
        );
    
    internal static readonly DiagnosticDescriptor RDC1008CollectionParameter =
        new(
            "RDC1008",
            "Collection parameter",
            "Collection parameters are not supported on slash commands.",
            Categories.SlashCommands,
            DiagnosticSeverity.Error,
            true
        );
    
    internal static readonly DiagnosticDescriptor RDC1009TooManyParameters =
        new(
            "RDC1009",
            "Too many parameters",
            "The slash command has too many parameters. (Max of 25, got {0}).",
            Categories.SlashCommands,
            DiagnosticSeverity.Error,
            true
        );

    internal static readonly DiagnosticDescriptor RDC1010DefaultMemberPermissions =
        new(
            "RDC1010",
            "Default member permissions",
            "Only one default member permission attribute is permitted on a command branch.",
            Categories.SlashCommands,
            DiagnosticSeverity.Error,
            true
        );
    
    internal static readonly DiagnosticDescriptor RDC1011DefaultMemberPermissionsOnSubCommand =
        new(
            "RDC1011",
            "Default member permissions on subcommand",
            "Default member permissions are not permitted on sub-commands.",
            Categories.SlashCommands,
            DiagnosticSeverity.Error,
            true
        );


}