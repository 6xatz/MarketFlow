using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

