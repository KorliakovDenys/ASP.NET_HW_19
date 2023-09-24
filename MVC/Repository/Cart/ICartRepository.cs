using Shared.Models;

namespace MVC.Repository.Cart;

public interface ICartRepository {
    public Task<IEnumerable<CartPosition>?> GetCart(string jwtToken);
    public Task<CartPosition?> GetCartPosition(int id, string jwtToken);
    public Task AddCartPosition(CartPosition cartPosition, string jwtToken);
    public Task UpdateCartPosition(int id, CartPosition cartPosition, string jwtToken);
    public Task RemoveCartPosition(int id, string jwtToken);
}