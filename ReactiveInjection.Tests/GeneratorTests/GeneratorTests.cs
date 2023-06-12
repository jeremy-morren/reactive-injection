using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ReactiveInjection.Tests.GeneratorTests;

[UsesVerify]
public class GeneratorTests
{
    [Fact]
    public Task Generate()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(Source);

        var compilation = CSharpCompilation.Create(
            assemblyName: "ReactiveInjection.GeneratorTests",
            references: GetReferences(typeof(FromDIAttribute), typeof(List<int>), typeof(List<int[]>), typeof(IServiceProvider)).ToArray(),
            syntaxTrees: new[] {syntaxTree});

        var generator = new ReactiveFactoryGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);

        return Verifier.Verify(driver);
    }
    
    private static readonly string Source = File.ReadAllText(Path.Combine(BaseDirectory, "GeneratorTests/Source.txt"));

    private static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;

    private static IEnumerable<MetadataReference> GetReferences(params Type[] types) =>
        types
            .Select(t => t.Assembly.Location)
            .Distinct()
            .Select(r => MetadataReference.CreateFromFile(r));
}