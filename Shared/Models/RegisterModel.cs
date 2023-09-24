using System.ComponentModel.DataAnnotations;

namespace Shared.Models; 

public class RegisterModel {
    [Required(ErrorMessage = "Invalid email.")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Invalid password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Invalid password confirmation.")]
    public string ConfirmPassword { get; set; }
}