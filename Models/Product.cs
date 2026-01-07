namespace Marketplace.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int SellerId { get; set; }
    public User Seller { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

