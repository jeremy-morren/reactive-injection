using Xunit.Abstractions;

namespace ReactiveInjection.Tests;

public class MethodNameHelpers
{
    private readonly ITestOutputHelper _output;

    public MethodNameHelpers(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void WriteMethodNames()
    {
    }
}