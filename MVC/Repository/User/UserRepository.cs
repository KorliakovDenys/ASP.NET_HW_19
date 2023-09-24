using System.Text;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace MVC.Repository.User;

public class UserRepository : Repository, IUserRepository {
    private readonly string _path;

    public UserRepository(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory) {
        _path = configuration["UserPath"]!;
    }

    public async Task<IEnumerable<Shared.Models.User>?> GetUsers(string jwtToken) {
        var response = await MakeRequest(HttpMethod.Get, _path, jwtToken);

        if (!response.IsSuccessStatusCode) return null;

        var jsonResponseBody = await response.Content.ReadAsStringAsync();
        var users = JsonConvert.DeserializeObject<IEnumerable<Shared.Models.User>>(jsonResponseBody);

        return users;
    }

    public async Task<Shared.Models.User?> GetUser(int id, string jwtToken) {
        var response = await MakeRequest(HttpMethod.Get, _path + id, jwtToken);

        if (!response.IsSuccessStatusCode) return null;

        var jsonResponseBody = await response.Content.ReadAsStringAsync();
        var user = JsonConvert.DeserializeObject<Shared.Models.User>(jsonResponseBody);

        return user;
    }

    public async Task AddUser(Shared.Models.User user, string jwtToken) {
        var stringContent = new StringContent(
            JsonConvert.SerializeObject(user), Encoding.UTF8, Application.Json);
        await MakeRequest(HttpMethod.Post, _path, jwtToken, stringContent);
    }

    public async Task UpdateUser(int id, Shared.Models.User user, string jwtToken) {
        var stringContent = new StringContent(
            JsonConvert.SerializeObject(user), Encoding.UTF8, Application.Json);
        await MakeRequest(HttpMethod.Put, _path + id, jwtToken, stringContent);
    }

    public async Task RemoveUser(int id, string jwtToken) {
        await MakeRequest(HttpMethod.Delete, _path + id, jwtToken);
    }
}