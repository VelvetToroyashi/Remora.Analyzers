using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Results;

namespace Remora.Discord.Commands.Analyzers.Tests;

public class SlashCommandAnalyzerTests 
{
    [Test]
    public async Task EmitsRDC1005WhenNestedTooDeeply()
    {
        const string source = """
        using Remora.Commands;
        using Remora.Commands.Groups;
        using Remora.Commands.Attributes;
        using Remora.Discord.API.Abstractions.Objects;
        using Remora.Discord.Commands.Attributes; 
        using Remora.Results;
        using System.Threading.Tasks; 
        
        [Group("a")]
        public class A : CommandGroup
        {
            [Group("b")]
            public class B : CommandGroup
            {
                [Group("c")]
                public class C : CommandGroup
                {
                    [Command("d")]
                    public async Task<IResult> D() { return Result.FromSuccess(); }
                } 
            }
        }
        """;
        
        var expected = Verifier<RDC1005SlashCommandNestedTooDeeplyAnalyzer>.Diagnostic(DiagnosticDescriptors.RDC1005SlashCommandNestedTooDeeply.Id)
            .WithLocation(16, 22)
            .WithSeverity(DiagnosticSeverity.Error);
        
        
        await Verifier<RDC1005SlashCommandNestedTooDeeplyAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }

    [Test]
    public async Task EmitsRDC1006WhenSlashCommandNameIsTooLong()
    {
        const string source = """
        using Remora.Commands;
        using Remora.Commands.Groups;
        using Remora.Commands.Attributes;
        using Remora.Discord.API.Abstractions.Objects;
        using Remora.Discord.Commands.Attributes; 
        using Remora.Results;
        using System.Threading.Tasks; 
        
        public class A : CommandGroup
        {
            [Command("this is a really long (and invalid!) command name!")]
            public async Task<IResult> D() { return Result.FromSuccess(); }
        }
        """;
        
        var expected = Verifier<RDC1006SlashCommandNameTooLongAnalyzer>.Diagnostic(DiagnosticDescriptors.RDC1006SlashCommandNameTooLong.Id)
            .WithLocation(11, 14)
            .WithSeverity(DiagnosticSeverity.Error);

        await Verifier<RDC1006SlashCommandNameTooLongAnalyzer>.VerifyAnalyzerAsync(source, expected);
    }
}