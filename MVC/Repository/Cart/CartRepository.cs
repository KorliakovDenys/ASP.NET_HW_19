using System.Text;
using Newtonsoft.Json;
using Shared.Models;
using static System.Net.Mime.MediaTypeNames;

namespace MVC.Repository.Cart;

public class CartRepository : Repository, ICartRepository {
    private readonly string _path;

    public CartRepository(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory) {
        _path = configuration["CartPath"]!;
    }

    public async Task<IEnumerable<CartPosition>?> GetCart(string jwtToken) {
        var response = await MakeRequest(HttpMethod.Get, _path, jwtToken);

        if (!response.IsSuccessStatusCode) return null;

        var jsonResponseBody = await response.Content.ReadAsStringAsync();
        var products = JsonConvert.DeserializeObject<IEnumerable<CartPosition>>(jsonResponseBody);

        return products;
    }

    public async Task<CartPosition?> GetCartPosition(int id, string jwtToken) {
        var response = await MakeRequest(HttpMethod.Get, _path + id, jwtToken);

        if (!response.IsSuccessStatusCode) return null;

        var jsonResponseBody = await response.Content.ReadAsStringAsync();
        var products = JsonConvert.DeserializeObject<CartPosition>(jsonResponseBody);

        return products;
    }

    public async Task AddCartPosition(CartPosition cartPosition, string jwtToken) {
        var stringContent = new StringContent(
            JsonConvert.SerializeObject(cartPosition), Encoding.UTF8, Application.Json);
        await MakeRequest(HttpMethod.Post, _path, jwtToken, stringContent);
    }

    public async Task UpdateCartPosition(int id, CartPosition cartPosition, string jwtToken) {
        var stringContent = new StringContent(
            JsonConvert.SerializeObject(cartPosition), Encoding.UTF8, Application.Json);
        await MakeRequest(HttpMethod.Put, _path + id, jwtToken, stringContent);
    }

    public async Task RemoveCartPosition(int id, string jwtToken) {
        await MakeRequest(HttpMethod.Delete, _path + id, jwtToken);
    }
}