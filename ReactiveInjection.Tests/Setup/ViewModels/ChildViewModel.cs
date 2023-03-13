using ReactiveInjection.Tests.Setup.Models;
using ReactiveInjection.Tests.Setup.Services;

namespace ReactiveInjection.Tests.Setup.ViewModels;

public class ChildViewModel
{
    public ChildViewModel(SharedState sharedState,
        ChildModel model,
        [FromDI] Service service,
        [FromDI] List<int> values)
    {
    }
}