using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

public class CartPosition {
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Amount { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("ProductId")]
    public Product? Product { get; set; }
}