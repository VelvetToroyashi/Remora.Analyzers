using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Remora.Discord.Commands.Analyzers.Extensions;

namespace Remora.Discord.Commands.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RDC1008CollectionParameterAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create
    (
        DiagnosticDescriptors.RDC1008CollectionParameter
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

        foreach (var parameter in method!.Parameters)
        {
            
            if (parameter.Type.IsCollection(context.Compilation))
            {
                context.ReportDiagnostic
                (
                    Diagnostic.Create
                    (
                        DiagnosticDescriptors.RDC1008CollectionParameter,
                        parameter.DeclaringSyntaxReferences[0].GetSyntax().GetLocation(),
                        parameter.Name
                    )
                );
            }
        }
    }
}