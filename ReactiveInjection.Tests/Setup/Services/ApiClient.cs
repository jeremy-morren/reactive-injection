using ReactiveInjection.Tests.Setup.Models;

namespace ReactiveInjection.Tests.Setup.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public IObservable<T> GetAsync<T>(IObservable<ApiUrl> url,
        string requestPath,
        CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}