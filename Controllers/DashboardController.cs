using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Marketplace.Data;

namespace Marketplace.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null) return RedirectToAction("Login", "Account");

        ViewBag.UserRole = user.Role;
        ViewBag.UserName = user.Name;

        if (user.Role == "Seller")
        {
            var totalProducts = await _context.Products.CountAsync(p => p.SellerId == userId);
            var totalOrders = await _context.Orders
                .CountAsync(o => o.OrderItems.Any(oi => oi.Product.SellerId == userId));
            var totalRevenue = await _context.Orders
                .Where(o => o.OrderItems.Any(oi => oi.Product.SellerId == userId))
                .SumAsync(o => o.TotalAmount);

            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
        }
        else
        {
            var totalOrders = await _context.Orders.CountAsync(o => o.BuyerId == userId);
            var pendingOrders = await _context.Orders
                .CountAsync(o => o.BuyerId == userId && o.Status == "Pending");
            var completedOrders = await _context.Orders
                .CountAsync(o => o.BuyerId == userId && o.Status == "Completed");

            ViewBag.TotalOrders = totalOrders;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.CompletedOrders = completedOrders;
        }

        return View();
    }
}

