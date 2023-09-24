using static System.Net.Mime.MediaTypeNames;
using System.Text;
using Newtonsoft.Json;
using Shared.Models;

namespace MVC.Utils; 

public class TokenHelper : Repository.Repository, ITokenHelper {
    private readonly string _path;
    
    public TokenHelper(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory) {
        _path = configuration["TokenPath"]!;
    }

    public async Task<TokenControllerPostResponse?> GetToken(LoginModel loginModel) {
        var stringContent = new StringContent(
            JsonConvert.SerializeObject(loginModel), Encoding.UTF8, Application.Json);
        var response = await MakeRequest(HttpMethod.Post, _path, content:stringContent);

        if (!response.IsSuccessStatusCode) return null;

        var jsonResponseBody = await response.Content.ReadAsStringAsync();
        var authData = JsonConvert.DeserializeObject<TokenControllerPostResponse>(jsonResponseBody);

        return authData;
    }
}