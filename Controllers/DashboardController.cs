using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SugboGo.Services.Auth;
using SugboGo.Services.Dashboard;

namespace SugboGo.Controllers;

[Authorize(Roles = AccountRoles.AdminOrClient)]
public sealed class DashboardController : Controller
{
    private readonly IDashboardExperienceService _dashboardExperienceService;

    public DashboardController(IDashboardExperienceService dashboardExperienceService)
    {
        _dashboardExperienceService = dashboardExperienceService;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Dashboard";
        return View(_dashboardExperienceService.BuildForUser(User));
    }
}
