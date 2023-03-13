using ReactiveInjection.Tests.Setup.Models;
using ReactiveInjection.Tests.Setup.Services;

namespace ReactiveInjection.Tests.Setup.ViewModels;

public class ParentViewModel
{
    public ParentViewModel(SharedState sharedState,
        ViewModelFactory factory,
        [FromDI] Service service,
        [FromDI] IServiceProvider services)
    {
    }
}