using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Marketplace.Data;
using Marketplace.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Marketplace.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public OrdersController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    // Buyer
    public async Task<IActionResult> MyOrders()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.BuyerId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
        return View(orders);
    }

    // Seller
    public async Task<IActionResult> SellerOrders()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var orderIds = await _context.OrderItems
            .Where(oi => oi.Product.SellerId == userId)
            .Select(oi => oi.OrderId)
            .Distinct()
            .ToListAsync();
            
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Buyer)
            .Where(o => orderIds.Contains(o.Id))
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
        return View(orders);
    }

    // Buyer: Buy a produxt
    [HttpPost]
    public async Task<IActionResult> Create(int productId, int quantity)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var product = await _context.Products.FindAsync(productId);
        
        if (product == null) return NotFound();

        var orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        var order = new Order
        {
            OrderNumber = orderNumber,
            BuyerId = userId,
            TotalAmount = product.Price * quantity,
            Status = "Pending",
            CreatedAt = DateTime.Now
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var orderItem = new OrderItem
        {
            OrderId = order.Id,
            ProductId = productId,
            Quantity = quantity,
            Price = product.Price
        };

        _context.OrderItems.Add(orderItem);
        await _context.SaveChangesAsync();

        return RedirectToAction("MyOrders");
    }

    // Seller: Update status order
    [HttpPost]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> UpdateStatus(int orderId, string status, IFormFile? shippingProof)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId && 
                o.OrderItems.Any(oi => oi.Product.SellerId == userId));

        if (order == null) return NotFound();

        order.Status = status;

        if (status == "Shipped" && shippingProof != null && shippingProof.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "shipping");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(shippingProof.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await shippingProof.CopyToAsync(stream);
            }

            order.ShippingProofImageUrl = $"/uploads/shipping/{fileName}";
            order.ShippedAt = DateTime.Now;
        }

        if (status == "Delivered")
        {
            order.DeliveredAt = DateTime.Now;
        }

        if (status == "Completed")
        {
            order.DeliveredAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("SellerOrders");
    }

    // Export PDF
    public async Task<IActionResult> ExportPdf(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        Order? order;
        
        if (userRole == "Seller")
        {
            var orderIds = await _context.OrderItems
                .Where(oi => oi.Product.SellerId == userId)
                .Select(oi => oi.OrderId)
                .ToListAsync();
                
            order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Buyer)
                .FirstOrDefaultAsync(o => o.Id == id && orderIds.Contains(o.Id));
        }
        else
        {
            order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.BuyerId == userId);
        }

        if (order == null) return NotFound();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("STRUK PEMBELIAN")
                    .SemiBold().FontSize(20).AlignCenter();

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(col =>
                    {
                        col.Item().Text($"Nomor Order: {order.OrderNumber}").SemiBold();
                        col.Item().Text($"Tanggal: {order.CreatedAt:dd MMMM yyyy HH:mm}").FontSize(9);
                        col.Item().PaddingTop(10);

                        if (userRole == "Seller")
                        {
                            col.Item().Text($"Pembeli: {order.Buyer.Name}").SemiBold();
                            col.Item().Text($"Email: {order.Buyer.Email}").FontSize(9);
                        }

                        col.Item().PaddingTop(10);
                        col.Item().Text("Detail Produk:").SemiBold();
                        col.Item().PaddingTop(5);

                        foreach (var item in order.OrderItems)
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem(3).Text(item.Product.Name);
                                row.RelativeItem(1).Text($"Qty: {item.Quantity}");
                                row.RelativeItem(2).AlignRight().Text($"Rp {item.Price * item.Quantity:N0}");
                            });
                            col.Item().PaddingBottom(5);
                        }

                        col.Item().PaddingTop(10);
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Total:").SemiBold().FontSize(12);
                            row.RelativeItem().AlignRight().Text($"Rp {order.TotalAmount:N0}").SemiBold().FontSize(12);
                        });

                        col.Item().PaddingTop(10);
                        col.Item().Text($"Status: {order.Status}").SemiBold();
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Terima kasih atas pembelian Anda!").FontSize(9);
                    });
            });
        });

        var stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;

        return File(stream, "application/pdf", $"struk_{order.OrderNumber}.pdf");
    }
}

