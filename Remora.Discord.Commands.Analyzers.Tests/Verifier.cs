using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Remora.Commands.Trees.Nodes;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Results;

namespace Remora.Discord.Commands.Analyzers.Tests;

public static class Verifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public static DiagnosticResult Diagnostic(string diagnosticId)
    {
        return CSharpAnalyzerVerifier<TAnalyzer, NUnitVerifier>.Diagnostic(diagnosticId);
    }

    public static async Task VerifyAnalyzerAsync(
        string source,
        params DiagnosticResult[] expected)
    {
        var test = new AnalyzerTest(source, expected);
        await test.RunAsync(CancellationToken.None);
    }

    private sealed class AnalyzerTest : CSharpAnalyzerTest<TAnalyzer, NUnitVerifier>
    {
        public AnalyzerTest(
            string source,
            params DiagnosticResult[] expected)
        {
            TestCode = source;
            ExpectedDiagnostics.AddRange(expected);
            ReferenceAssemblies = new ReferenceAssemblies(
                "net7.0",
                new PackageIdentity("Microsoft.NETCore.App.Ref", "7.0.0"),
                Path.Combine("ref", "net7.0"));
            TestState.AdditionalReferences.Add(typeof(InvalidNodeException).Assembly);
            TestState.AdditionalReferences.Add(typeof(IChildNode).Assembly);
            TestState.AdditionalReferences.Add(typeof(IUser).Assembly);
            TestState.AdditionalReferences.Add(typeof(Result).Assembly);
        }
    }
}