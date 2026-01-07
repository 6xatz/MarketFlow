namespace Marketplace.Models;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int BuyerId { get; set; }
    public User Buyer { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Shipped, Delivered, Completed
    public string? ShippingProofImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

