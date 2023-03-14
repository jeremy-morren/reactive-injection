using Microsoft.CodeAnalysis;
using ReactiveInjection.DependencyTree;

// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace ReactiveInjection.Tests.DependencyTreeTests;

public class FakeErrorLog : IErrorLog
{
    private readonly List<(string Id, string Title, string MessageFormat)> _errors = new();
    
    public IReadOnlyList<(string Id, string Title, string MessageFormat)> Errors => _errors.AsReadOnly();
    
    public void WriteError(Location location, string id, string title, string messageFormat, params object[] messageArgs)
    {
        _errors.Add((id, title, messageFormat));
    }
}