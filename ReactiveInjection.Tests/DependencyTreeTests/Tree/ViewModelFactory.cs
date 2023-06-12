using ReactiveInjection.Tests.DependencyTreeTests.Tree.Models;
using ReactiveInjection.Tests.DependencyTreeTests.Tree.ViewModels;

namespace ReactiveInjection.Tests.DependencyTreeTests.Tree;

public class ViewModelFactory
{
    [ReactiveFactory]
    public ParentViewModel NewParent() => throw new NotImplementedException();

    [ReactiveFactory]
    public ChildViewModel NewChild(Model model) => throw new NotImplementedException();
}