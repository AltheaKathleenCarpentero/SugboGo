using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SugboGo.Models;
using SugboGo.Services.Travel;

namespace SugboGo.Controllers;

public class BookingController : Controller
{
    private readonly ITravelPreferenceStore _preferenceStore;
    private readonly ICebuRecommendationService _recommendationService;

    public BookingController(ITravelPreferenceStore preferenceStore, ICebuRecommendationService recommendationService)
    {
        _preferenceStore = preferenceStore;
        _recommendationService = recommendationService;
    }

    [Authorize]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Start));
    }

    public IActionResult Start()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction("Index", "Account", new { returnUrl = Url.Action(nameof(Survey), "Booking") });
        }

        return RedirectToAction(nameof(Survey));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Survey(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Cebu Preference Survey";

        var existing = await _preferenceStore.FindLatestByUserIdAsync(GetUserId(), cancellationToken);
        return View(new TravelPreferenceSurveyViewModel
        {
            SelectedInterests = existing?.Interests ?? [],
            AdventureLevel = existing?.AdventureLevel is > 0 ? existing.AdventureLevel : 3,
            TravelPace = string.IsNullOrWhiteSpace(existing?.TravelPace) ? "Balanced" : existing.TravelPace,
            BudgetRange = string.IsNullOrWhiteSpace(existing?.BudgetRange) ? "Mid-range" : existing.BudgetRange,
            Notes = existing?.Notes
        });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Survey(TravelPreferenceSurveyViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Cebu Preference Survey";

        model.SelectedInterests = model.SelectedInterests
            .Select(interest => interest.Trim().ToLowerInvariant())
            .Where(interest => TravelInterestCatalog.Options.Any(option => option.Key == interest))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (model.SelectedInterests.Count == 0)
        {
            ModelState.AddModelError(nameof(model.SelectedInterests), "Choose at least one Cebu interest.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _preferenceStore.SaveAsync(new TravelPreferenceRecord
        {
            UserId = GetUserId(),
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Interests = model.SelectedInterests,
            AdventureLevel = model.AdventureLevel,
            TravelPace = model.TravelPace,
            BudgetRange = model.BudgetRange,
            Notes = model.Notes
        }, cancellationToken);

        return RedirectToAction(nameof(Recommendations));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Recommendations(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "AI Cebu Recommendations";

        var preferences = await _preferenceStore.FindLatestByUserIdAsync(GetUserId(), cancellationToken);
        if (preferences is null)
        {
            return RedirectToAction(nameof(Survey));
        }

        return View(_recommendationService.BuildRecommendations(preferences));
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Signed-in user is missing a name identifier claim.");
    }
}
