using ReactiveInjection.Tests.DependencyTreeTests.Tree.Models;
using ReactiveInjection.Tests.DependencyTreeTests.Tree.Services;

namespace ReactiveInjection.Tests.DependencyTreeTests.Tree.ViewModels;

public class ParentViewModel
{
    public ParentViewModel(SharedState sharedState,
        ViewModelFactory factory,
        [FromDI] Service service,
        [FromDI] IServiceProvider services)
    {
    }
}