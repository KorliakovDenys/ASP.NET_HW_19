namespace MVC.Repository.User;

public interface IUserRepository {
    public Task<IEnumerable<Shared.Models.User>?> GetUsers(string jwtToken);
    public Task<Shared.Models.User?> GetUser(int id, string jwtToken);
    public Task AddUser(Shared.Models.User user, string jwtToken);
    public Task UpdateUser(int id, Shared.Models.User user, string jwtToken);
    public Task RemoveUser(int id, string jwtToken);
}