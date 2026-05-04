// ExploreController.cs (Added to support multiple actions used in navbar)
using Microsoft.AspNetCore.Mvc;

namespace SugboGo.Controllers;

public class ExploreController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "SugboGo - Cebu like a local";
        return View();
    }

    public IActionResult Tours()
    {
        ViewData["Title"] = "Cebu Tours | SugboGo";
        return View();
    }

    public IActionResult Hiking()
    {
        ViewData["Title"] = "Hiking trails in Cebu | SugboGo";
        return View();
    }

    public IActionResult Gastronomy()
    {
        ViewData["Title"] = "Cebu food & gastronomy | SugboGo";
        return View();
    }

    public IActionResult Spots()
    {
        ViewData["Title"] = "Trending spots | SugboGo";
        return View();
    }
}