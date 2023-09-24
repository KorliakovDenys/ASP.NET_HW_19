using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

public class UserRole {
    public int UserId { get; set; }
    public int RoleId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("RoleId")]
    public Role? Role { get; set; }
}