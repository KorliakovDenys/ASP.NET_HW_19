namespace MVC.Repository;

public abstract class Repository {
    private readonly IHttpClientFactory _clientFactory;

    protected Repository(IHttpClientFactory clientFactory) {
        _clientFactory = clientFactory;
    }

    protected async Task<HttpResponseMessage> MakeRequest(HttpMethod httpMethod, string requestUri,
        string? jwtToken = null,
        StringContent? content = null) {
        var api = _clientFactory.CreateClient(name: "Api");
        var request = new HttpRequestMessage(method: httpMethod, requestUri: requestUri);
        
        if (jwtToken is not null) request.Headers.Add("Authorization", $"Bearer {jwtToken}");
        if (content is not null) request.Content = content;
        
        return await api.SendAsync(request);
    }
}