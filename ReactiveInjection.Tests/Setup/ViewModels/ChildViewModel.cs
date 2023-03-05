using ReactiveInjection.Abstractions;
using ReactiveInjection.Tests.Setup.Models;
using ReactiveInjection.Tests.Setup.Services;

namespace ReactiveInjection.Tests.Setup.ViewModels;

public class ChildViewModel
{
    public IObservable<ApiUrl> SharedState { get; }
    public ChildModel Model { get; }
    public ApiClient ApiClient { get; }

    public ChildViewModel(IObservable<ApiUrl> sharedState,
        ChildModel model,
        [FromDI] ApiClient apiClient)
    {
        SharedState = sharedState;
        Model = model;
        ApiClient = apiClient;
    }
}