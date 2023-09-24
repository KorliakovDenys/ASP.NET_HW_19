namespace MVC.Repository.Product;

public interface IProductRepository {
    public Task<IEnumerable<Shared.Models.Product>?> GetProducts(string jwtToken);
    public Task<Shared.Models.Product?> GetProduct(int id, string jwtToken);
    public Task AddProduct(Shared.Models.Product product, string jwtToken);
    public Task UpdateProduct(int id, Shared.Models.Product product, string jwtToken);
    public Task RemoveProduct(int id, string jwtToken);
}