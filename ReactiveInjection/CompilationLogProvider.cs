using Microsoft.CodeAnalysis;
using ReactiveInjection.Generator;

namespace ReactiveInjection;

public class CompilationLogProvider : IErrorLog
{
    private readonly SourceProductionContext _context;

    public CompilationLogProvider(SourceProductionContext context) => _context = context;

    public void WriteError(Location location,
        string id,
        string title,
        string messageFormat,
        params object?[] messageArgs)
    {
        var descriptor = new DiagnosticDescriptor(id,
            title,
            messageFormat,
            "ReactiveInjection.SourceGenerator",
            DiagnosticSeverity.Error,
            true);
        _context.ReportDiagnostic(Diagnostic.Create(descriptor, location, messageArgs));
    }
}