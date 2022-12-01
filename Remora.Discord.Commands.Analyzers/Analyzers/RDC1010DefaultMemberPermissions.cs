using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Remora.Discord.Commands.Analyzers.Extensions;

namespace Remora.Discord.Commands.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RDC1010DefaultMemberPermissions : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create
    (
        DiagnosticDescriptors.RDC1010DefaultMemberPermissions
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

        if (!method!.IsSlashCommandMethod(out _))
        {
            return;
        }
        
        var classDeclaration = method!.ContainingType.DeclaringSyntaxReferences[0].GetSyntax() as ClassDeclarationSyntax;
        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration!)!;

        var defaultMemberPermissionAttribute = context.Compilation.GetTypeByMetadataName("Remora.Discord.Commands.Attributes.DiscordDefaultMemberPermissionsAttribute");
        
        var hasDefaultPermissions = method.GetAttributes().Any(attr => attr.AttributeClass!.Equals(defaultMemberPermissionAttribute, SymbolEqualityComparer.Default));
        var containingAttributes = hasDefaultPermissions ? 1 : 0;
        
        
        do
        {
            if (!classSymbol.IsSlashCommandGroup())
                break;
            
            if (classSymbol.GetAttributes().Any(attr => attr.AttributeClass!.Equals(defaultMemberPermissionAttribute, SymbolEqualityComparer.Default)))
            {
                containingAttributes++;
                break;
            }
        } 
        while ((classSymbol = classSymbol.ContainingType) is not null);
        
        if (containingAttributes > 1)
        {
            context.ReportDiagnostic
            (
                Diagnostic.Create
                (
                    DiagnosticDescriptors.RDC1010DefaultMemberPermissions,
                    method.Locations[0]
                )
            );
        }
    }
}