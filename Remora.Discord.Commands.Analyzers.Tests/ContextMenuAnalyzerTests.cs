using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Remora.Discord.API.Abstractions.Objects;

namespace Remora.Discord.Commands.Analyzers.Tests;

public class ContextMenuAnalyzerTests
{
    [Test]
    public async Task EmitsRDC1001WhenMissingTarget()
    {
        const string source = """
        using Remora.Commands;
        using Remora.Commands.Attributes;
        using Remora.Discord.API.Abstractions.Objects;
        using Remora.Discord.Commands.Attributes; 
        using Remora.Results;
        using System.Threading.Tasks; 
        
        public class A 
        {
                [Command("context_menu")]
                [CommandType(ApplicationCommandType.User)]
                public async Task<IResult> CommandAsync() { return Result.FromSuccess(); } 
        } 
        """;
        
        var expected = Verifier<ContextMenuAnalyzer>.Diagnostic(DiagnosticDescriptors.RDC1001ContextMenuMissingTarget.Id)
            .WithLocation(12, 48)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithArguments("Remora.Discord.API.Abstractions.Objects.IUser");
        
        await Verifier<ContextMenuAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task EmitsRDC1002WhenExcessiveParameters()
    {
        const string source = """
        using Remora.Commands;
        using Remora.Commands.Attributes;
        using Remora.Discord.API.Abstractions.Objects;
        using Remora.Discord.Commands.Attributes; 
        using Remora.Results;
        using System.Threading.Tasks; 
        
        public class A 
        {
                [Command("context_menu")]
                [CommandType(ApplicationCommandType.User)]
                public async Task<IResult> CommandAsync(IUser user, object a) { return Result.FromSuccess(); } 
        } 
        """;
        
        var expected = Verifier<ContextMenuAnalyzer>.Diagnostic(DiagnosticDescriptors.RDC1002ContextMenuTooManyParameters.Id)
            .WithLocation(12, 48)
            .WithSeverity(DiagnosticSeverity.Error);
        
        
        await Verifier<ContextMenuAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task EmitsRDC1003WhenParameterDoesntMatch()
    {
        const string source = """
        using Remora.Commands;
        using Remora.Commands.Attributes;
        using Remora.Discord.API.Abstractions.Objects;
        using Remora.Discord.Commands.Attributes; 
        using Remora.Results;
        using System.Threading.Tasks; 
        
        public class A 
        {
                [Command("context_menu")]
                [CommandType(ApplicationCommandType.User)]
                public async Task<IResult> CommandAsync(IMessage message) { return Result.FromSuccess(); } 
        } 
        """;
        
        var expected = Verifier<ContextMenuAnalyzer>.Diagnostic(DiagnosticDescriptors.RDC1003ContextMenuMismatchedTarget.Id)
            .WithLocation(12, 48)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithArguments(typeof(IMessage).FullName!, typeof(IUser).FullName!);
        
        
        await Verifier<ContextMenuAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task EmitsRDC1004WhenParameterIsInvalid()
    {
        const string source = """
        using Remora.Commands;
        using Remora.Commands.Attributes;
        using Remora.Discord.API.Abstractions.Objects;
        using Remora.Discord.Commands.Attributes; 
        using Remora.Results;
        using System.Threading.Tasks; 
        
        public class A 
        {
                [Command("context_menu")]
                [CommandType(ApplicationCommandType.User)]
                public async Task<IResult> CommandAsync(object invalidParameter) { return Result.FromSuccess(); } 
        } 
        """;
        
        var expected = Verifier<ContextMenuAnalyzer>.Diagnostic(DiagnosticDescriptors.RDC1004ContextMenuIncorrectParameterType.Id)
            .WithLocation(12, 56)
            .WithSeverity(DiagnosticSeverity.Error)
            .WithArguments("object", typeof(IUser).FullName!);
        
        
        await Verifier<ContextMenuAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }
}