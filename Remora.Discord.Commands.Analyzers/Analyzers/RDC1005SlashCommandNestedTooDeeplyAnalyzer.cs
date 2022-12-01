using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Remora.Discord.Commands.Analyzers.Extensions;

namespace Remora.Discord.Commands.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RDC1005SlashCommandNestedTooDeeplyAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DiagnosticDescriptors.RDC1005SlashCommandNestedTooDeeply);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis
        (
            GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics
        );
        
        context.RegisterSyntaxNodeAction(action: Handle, SyntaxKind.ClassDeclaration);
    }

    private void Handle(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration)!;

        var depth = 0;

        do 
        {
            if (!classSymbol.IsSlashCommandGroup())
            {
                break;
            }

            depth++;
        }
        while ((classSymbol = classSymbol?.ContainingType) is not null);
        
        if (depth > 2)
        {
            context.ReportDiagnostic
            (
                Diagnostic.Create
                (
                    DiagnosticDescriptors.RDC1005SlashCommandNestedTooDeeply,
                    classDeclaration.Identifier.GetLocation()
                )
            );
        }
    }
}
