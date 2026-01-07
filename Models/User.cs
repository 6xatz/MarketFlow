namespace Marketplace.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Buyer"; // Buyer or Seller
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
