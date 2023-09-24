namespace Shared.Models;

public class TokenControllerPostResponse {
    public string? JwtToken { get; set; }
    public string? Login { get; set; }
    public Role? Role { get; set; }
}