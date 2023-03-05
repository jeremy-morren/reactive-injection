using ReactiveInjection.Abstractions;
using ReactiveInjection.Tests.Setup.Models;
using ReactiveInjection.Tests.Setup.Services;

namespace ReactiveInjection.Tests.Setup.ViewModels;

public class ParentViewModel
{
    public IObservable<ApiUrl> ApiUrl { get; }
    public ViewModelFactory Factory { get; }
    public ApiClient ApiClient { get; }
    public IServiceProvider Services { get; }

    public ParentViewModel(IObservable<ApiUrl> apiUrl,
        ViewModelFactory factory,
        [FromDI] ApiClient apiClient,
        [FromDI] IServiceProvider services)
    {
        ApiUrl = apiUrl;
        Factory = factory;
        ApiClient = apiClient;
        Services = services;
    }
}