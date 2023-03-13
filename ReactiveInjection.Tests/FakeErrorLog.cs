using Microsoft.CodeAnalysis;
using ReactiveInjection.DependencyTree;

namespace ReactiveInjection.Tests;

public class FakeErrorLog : IErrorLog
{
    private readonly List<(string Id, string Title, string Message)> _errors = new();

    public void WriteError(string id, string title, string message, Location location)
    {
        _errors.Add((id, title, message));
    }

    public IReadOnlyList<(string Id, string Title, string Message)> Errors => _errors.AsReadOnly();
}