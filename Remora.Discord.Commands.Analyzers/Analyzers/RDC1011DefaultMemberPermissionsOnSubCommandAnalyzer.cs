using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Remora.Discord.Commands.Analyzers.Extensions;

namespace Remora.Discord.Commands.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RDC1011DefaultMemberPermissionsOnSubCommandAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create
    (
        DiagnosticDescriptors.RDC1011DefaultMemberPermissionsOnSubCommand
    );

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis
        (
            GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics
        );
        
        context.RegisterSyntaxNodeAction(action: Handle, SyntaxKind.MethodDeclaration);
    }

    private void Handle(SyntaxNodeAnalysisContext context)
    {
        var method = context.SemanticModel.GetDeclaredSymbol(context.Node) as IMethodSymbol;

        if (!method!.IsSlashSubCommnad())
        {
            return;
        }
        
        var defaultMemberPermissionAttribute = context.Compilation.GetTypeByMetadataName("Remora.Discord.Commands.Attributes.DiscordDefaultMemberPermissionsAttribute");
        
        var defaultPermissionAttribute = method!.GetAttributes().FirstOrDefault(attr => attr.AttributeClass!.Equals(defaultMemberPermissionAttribute, SymbolEqualityComparer.Default));

        if (defaultPermissionAttribute is not null)
        {
            context.ReportDiagnostic
            (
                Diagnostic.Create
                (
                    DiagnosticDescriptors.RDC1011DefaultMemberPermissionsOnSubCommand,
                    defaultPermissionAttribute.ApplicationSyntaxReference!.GetSyntax().GetLocation()
                )
            );
        }

    }
}