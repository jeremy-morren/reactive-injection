using ReactiveInjection.Tests.DependencyTreeTests.Tree.Models;
using ReactiveInjection.Tests.DependencyTreeTests.Tree.Services;

namespace ReactiveInjection.Tests.DependencyTreeTests.Tree.ViewModels;

public class ChildViewModel
{
    public ChildViewModel(SharedState sharedState,
        Model model,
        [FromDI] Service service,
        [FromDI] List<int> values)
    {
    }
}