using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ReactiveInjection.DependencyInjection;
using Shouldly;

namespace ReactiveInjection.Tests.DependencyInjection.Generator;

[UsesVerify]
public class GeneratorTests
{
    [Fact]
    public Task Generate()
    {
        new Tree.Models.ViewModelFactory().ShouldNotBeNull();
        var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(Source));
        
        var compilation = CSharpCompilation.Create(
            assemblyName: "ReactiveInjection.GeneratorTests",
            references: GetReferences(typeof(ReactiveFactoryAttribute), typeof(List<int>), typeof(IServiceProvider)),
            syntaxTrees: new[] {syntaxTree},
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        compilation.GetDiagnostics().ShouldBeEmpty();

        var generator = new ReactiveFactoryGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);

        return Verifier.Verify(driver);
    }
    
    private static readonly string Source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DependencyInjection/ViewModels.cs");

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