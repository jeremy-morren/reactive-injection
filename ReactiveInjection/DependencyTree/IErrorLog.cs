using Microsoft.CodeAnalysis;

namespace ReactiveInjection.DependencyTree;

internal interface IErrorLog
{
    void WriteError(string id,
        string title,
        string message,
        Location location);
}