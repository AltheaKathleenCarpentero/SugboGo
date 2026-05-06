using Microsoft.AspNetCore.Mvc;

namespace SugboGo.Controllers;

public class BookingController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Journey Timeline";
        return View();
    }
}
