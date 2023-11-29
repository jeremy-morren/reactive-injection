using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ReactiveInjection.SourceGenerators.ModelInjection;
using Shouldly;

namespace ReactiveInjection.Tests.ModelInjection;

[UsesVerify]
public class GeneratorTests
{
    [Fact]
    public Task Generate()
    {
        new InjectedViewModel1().ShouldNotBeNull();
        var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(Source));

        var compilation = CSharpCompilation.Create(
            assemblyName: "ReactiveInjection.ModelGeneratorTests",
            references: GetReferences(typeof(BackingModelAttribute), typeof(List<int>), typeof(IServiceProvider)),
            syntaxTrees: new[] { syntaxTree },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        
        compilation.GetDiagnostics().ShouldBeEmpty();

        var generator = new ModelInjectionGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);
        
        return Verifier.Verify(driver);
    }
    
    private static readonly string Source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ModelInjection/Models.cs");

    private static IEnumerable<MetadataReference> GetReferences(params Type[] types)
    {
        var standard = new[]
        {
            Assembly.Load("netstandard, Version=2.0.0.0"),
            Assembly.Load("System.Runtime"),
            Assembly.Load("System.ObjectModel")
        };
        var custom = types.Select(t => t.Assembly);
            
        return standard.Concat(custom)
            .Distinct()
            .Select(a => MetadataReference.CreateFromFile(a.Location));
    }
}