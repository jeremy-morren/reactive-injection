using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Generator;

internal interface IErrorLog
{
    void WriteError(Location location,
        string id,
        string title,
        string messageFormat,
        params object[] messageArgs);
}