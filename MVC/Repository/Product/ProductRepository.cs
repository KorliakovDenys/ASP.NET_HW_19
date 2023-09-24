using System.Text;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace MVC.Repository.Product;

public class ProductRepository : Repository, IProductRepository {
    private readonly string _path;

    public ProductRepository(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory) {
        _path = configuration["ProductPath"]!;
    }

    public async Task<IEnumerable<Shared.Models.Product>?> GetProducts(string jwtToken) {
        var response = await MakeRequest(HttpMethod.Get, _path, jwtToken);

        if (!response.IsSuccessStatusCode) return null;

        var jsonResponseBody = await response.Content.ReadAsStringAsync();
        var products = JsonConvert.DeserializeObject<IEnumerable<Shared.Models.Product>>(jsonResponseBody);

        return products;
    }

    public async Task<Shared.Models.Product?> GetProduct(int id, string jwtToken) {
        var response = await MakeRequest(HttpMethod.Get, _path + id, jwtToken);

        if (!response.IsSuccessStatusCode) return null;

        var jsonResponseBody = await response.Content.ReadAsStringAsync();
        var product = JsonConvert.DeserializeObject<Shared.Models.Product>(jsonResponseBody);

        return product;
    }

    public async Task AddProduct(Shared.Models.Product product, string jwtToken) {
        var stringContent = new StringContent(
            JsonConvert.SerializeObject(product), Encoding.UTF8, Application.Json);
        await MakeRequest(HttpMethod.Post, _path, jwtToken, stringContent);
    }

    public async Task UpdateProduct(int id, Shared.Models.Product product, string jwtToken) {
        var stringContent = new StringContent(
            JsonConvert.SerializeObject(product), Encoding.UTF8, Application.Json);
        await MakeRequest(HttpMethod.Put, _path + id, jwtToken, stringContent);
    }

    public async Task RemoveProduct(int id, string jwtToken) {
        await MakeRequest(HttpMethod.Delete, _path + id, jwtToken);
    }
}