using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Remora.Discord.Commands.Analyzers.Extensions;

namespace Remora.Discord.Commands.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RDC1006SlashCommandNameTooLongAnalyzer : DiagnosticAnalyzer
{

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } 
        = ImmutableArray.Create(DiagnosticDescriptors.RDC1006SlashCommandNameTooLong);
    
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
        
        if (method?.DeclaredAccessibility is not Accessibility.Public)
        {
            return;
        }

        if (!method.IsSlashCommandMethod(out var name))
        {
            return;
        }

        if (name.Length > 32)
        {
            context.ReportDiagnostic
            (
                Diagnostic.Create
                (
                    DiagnosticDescriptors.RDC1006SlashCommandNameTooLong,
                    GetCommandAttributeNode((MethodDeclarationSyntax)context.Node).GetLocation(),
                    name
                )
            );
        }
    }
    
    private SyntaxNode GetCommandAttributeNode(MethodDeclarationSyntax method)
    {
        return method.AttributeLists
            .SelectMany(al => al.Attributes)
            .FirstOrDefault(a => a.Name.ToString() == "Command")!
            .ArgumentList!
            .Arguments[0];
    }
}