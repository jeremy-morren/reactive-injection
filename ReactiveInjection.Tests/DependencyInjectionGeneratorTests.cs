using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ReactiveInjection.SourceGenerators.DependencyInjection;
using Shouldly;

namespace ReactiveInjection.Tests;

public class DependencyInjectionGeneratorTests
{
    [Fact]
    public async Task Generate()
    {
        new Tree.Models.ViewModelFactory().ShouldNotBeNull();
        var syntaxTree = CSharpSyntaxTree.ParseText(await File.ReadAllTextAsync(Source));
        
        var compilation = CSharpCompilation.Create(
            assemblyName: "ReactiveInjection.GeneratorTests",
            references: GetReferences(typeof(ReactiveFactoryAttribute), 
                typeof(List<int>), typeof(IServiceProvider)),
            syntaxTrees: new[] {syntaxTree},
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        compilation.GetDiagnostics().ShouldBeEmpty();

        var generator = new ReactiveFactoryGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);

        await Verifier.Verify(driver);

        diagnostics.ShouldBeEmpty();
    }
    
    private static readonly string Source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ViewModels.cs");

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