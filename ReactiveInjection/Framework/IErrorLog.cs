using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Framework;

internal interface IErrorLog
{
    void WriteError(Location location,
        string id,
        string title,
        string messageFormat,
        params object[] messageArgs);
}