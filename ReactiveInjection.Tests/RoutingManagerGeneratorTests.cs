using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ReactiveInjection.SourceGenerators.DependencyInjection;
using ReactiveInjection.SourceGenerators.Routing;
using Shouldly;

namespace ReactiveInjection.Tests;

public class RoutingManagerGeneratorTests
{
    [Fact]
    public async Task Generate()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(await File.ReadAllTextAsync(Source));
        
        var compilation = CSharpCompilation.Create(
            assemblyName: "ReactiveInjection.GeneratorTests",
            references: GetReferences(typeof(NavigationRouteAttribute), 
                typeof(List<int>), typeof(IServiceProvider)),
            syntaxTrees: new[] {syntaxTree},
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        
        compilation.GetDiagnostics().ShouldBeEmpty();

        var generator = new RoutingManagerGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);
        
        await Verify(driver);
        
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