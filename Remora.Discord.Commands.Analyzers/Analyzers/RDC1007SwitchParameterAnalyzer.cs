using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Remora.Discord.Commands.Analyzers.Extensions;

namespace Remora.Discord.Commands.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RDC1007SwitchParameterAnalyzer : DiagnosticAnalyzer
{
    private const string AttributeName = "Remora.Commands.Attributes.SwitchAttribute";
    
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create
    (
        DiagnosticDescriptors.RDC1007SwitchParameter
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

        if (!method.IsSlashCommandMethod(out _))
        {
            return;
        }
        
        var methodParameters = method!.Parameters;

        foreach (var parameter in methodParameters)
        {
            if (parameter.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == AttributeName))
                context.ReportDiagnostic
                (
                    Diagnostic.Create
                    (
                        DiagnosticDescriptors.RDC1007SwitchParameter,
                        parameter.DeclaringSyntaxReferences[0].GetSyntax().GetLocation()
                    )
                );
        }
    }
}