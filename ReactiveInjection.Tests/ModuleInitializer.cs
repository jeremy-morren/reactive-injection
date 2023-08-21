using System.Runtime.CompilerServices;

namespace ReactiveInjection.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    [Obsolete("Obsolete")]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}