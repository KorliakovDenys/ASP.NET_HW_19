using System.ComponentModel.DataAnnotations;

namespace Shared.Models; 

public class LoginModel {
    [Required(ErrorMessage = "Invalid email.")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Invalid password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}