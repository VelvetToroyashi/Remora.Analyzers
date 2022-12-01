using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Remora.Discord.Commands.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)] 
public class ContextMenuAnalyzer : DiagnosticAnalyzer
{
    private const string RemoraUserObject    = "Remora.Discord.API.Abstractions.Objects.IUser";
    private const string RemoraMessageObject = "Remora.Discord.API.Abstractions.Objects.IMessage";
    private const string RemoraCommandAttributeName = "Remora.Commands.Attributes.CommandAttribute";
    private const string RemoraCommandTypeAttributeName = "Remora.Discord.Commands.Attributes.CommandTypeAttribute";
    
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis
        (
            GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics
        );
        
        context.RegisterSyntaxNodeAction(action: Handle, SyntaxKind.MethodDeclaration);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create
    (
        DiagnosticDescriptors.RDC1001ContextMenuMissingTarget, 
        DiagnosticDescriptors.RDC1002ContextMenuTooManyParameters,
        DiagnosticDescriptors.RDC1003ContextMenuMismatchedTarget,
        DiagnosticDescriptors.RDC1004ContextMenuIncorrectParameterType
    );

    private void Handle(SyntaxNodeAnalysisContext context)
    {
        var model = (IMethodSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node);
        
        // Check if the method is public, which is a requirement for Remora.Commands
        if (model?.DeclaredAccessibility is not Accessibility.Public)
        {
            return;
        }
        
        // Check if the method is a context menu
        if (!IsContextMenu(context.Compilation, model, out var targetType))
        {
           return;
        }

        if (model.Parameters.Length == 1)
        {
            if (model.Parameters[0].Type.ToDisplayString() is not (RemoraMessageObject or RemoraUserObject))
            {
                context.ReportDiagnostic
                (
                    Diagnostic.Create
                    (
                        DiagnosticDescriptors.RDC1004ContextMenuIncorrectParameterType, 
                        model.Parameters[0].Locations[0],
                        model.Parameters[0].ToDisplayString(),
                        targetType
                    )
                );
                
                return;
            }
            
            if (model.Parameters[0].ToDisplayString() != targetType)
            {
                context.ReportDiagnostic
                (
                    Diagnostic.Create
                    (
                        DiagnosticDescriptors.RDC1003ContextMenuMismatchedTarget, 
                        context.Node.ChildNodes().First(static p => p is ParameterListSyntax or ArgumentListSyntax).GetLocation(),
                        model.Parameters[0].ToDisplayString(),
                        targetType
                    )
                );
            }

            return;
        }
        
        if (model.Parameters.Length is 0)
        {
            context.ReportDiagnostic
            (
                Diagnostic.Create
                (
                    DiagnosticDescriptors.RDC1001ContextMenuMissingTarget, 
                    context.Node.ChildNodes().OfType<BaseParameterListSyntax>().FirstOrDefault()?.GetLocation(),
                    targetType
                )   
            );
        }
        else
        {
            // Too many parameters
            context.ReportDiagnostic
            (
                Diagnostic.Create
                (
                    DiagnosticDescriptors.RDC1002ContextMenuTooManyParameters, 
                    context.Node.ChildNodes().OfType<BaseParameterListSyntax>().FirstOrDefault()?.GetLocation()
                )   
            );
        }
    }
    
    private bool IsContextMenu(Compilation compilation, IMethodSymbol model, out string parameterType)
    {
        parameterType = null;

        var attribtueType = compilation.GetTypeByMetadataName(RemoraCommandTypeAttributeName);
        
        var attribute = model.GetAttributes().FirstOrDefault
        (
            p => p.AttributeClass?.Equals(attribtueType, SymbolEqualityComparer.Default) ?? false
        );

        if (attribute is null)
        {
            return false;
        }

        var type = (int)(attribute.ConstructorArguments.FirstOrDefault().Value! ?? 0);

        // Who knows, could be more in the future.
        if (type is not (2 or 3))
        {
            return false;
        }

        parameterType = type is 2 ? RemoraUserObject : RemoraMessageObject;
        
        return true;
    }
}