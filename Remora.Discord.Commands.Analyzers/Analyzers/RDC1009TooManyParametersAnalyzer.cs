using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Remora.Discord.Commands.Analyzers.Extensions;

namespace Remora.Discord.Commands.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RDC1009TooManyParametersAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create
    (
        DiagnosticDescriptors.RDC1009TooManyParameters
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
        
        if (method!.Parameters.Length > 25)
        {
            context.ReportDiagnostic
            (
                Diagnostic.Create
                (
                    DiagnosticDescriptors.RDC1009TooManyParameters,
                    //TODO: Somehow get the locations of ALL parameters that exceed the limit. For now, this works.
                    method.Parameters[24].DeclaringSyntaxReferences[0].GetSyntax().GetLocation()
                )
            );
        }
    }
}