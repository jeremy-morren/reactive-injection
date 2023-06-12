using Microsoft.CodeAnalysis;

namespace ReactiveInjection.DependencyTree;

internal interface IErrorLog
{
    void WriteError(Location location,
        string id,
        string title,
        string messageFormat,
        params object[] messageArgs);
}