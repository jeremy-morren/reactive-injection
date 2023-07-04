namespace ReactiveInjection.Tests.DependencyTreeTests.Tree.ViewModels;

public class SimpleViewModel
{
    public string Str { get; }

    public SimpleViewModel(string str) => Str = str;
}