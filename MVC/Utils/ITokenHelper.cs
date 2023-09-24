using Shared.Models;

namespace MVC.Utils; 

public interface ITokenHelper {
    public Task<TokenControllerPostResponse?> GetToken(LoginModel loginModel);
}