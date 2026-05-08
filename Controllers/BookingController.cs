using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SugboGo.Data;
using SugboGo.Models;
using SugboGo.Services.Travel;

namespace SugboGo.Controllers;

public class BookingController : Controller
{
    private readonly ITravelPreferenceStore _preferenceStore;
    private readonly ICebuRecommendationService _recommendationService;
    private readonly SugboGoDbContext _dbContext;
    private readonly ILogger<BookingController> _logger;
    private static readonly IReadOnlyDictionary<string, BookingDestinationSeed> DestinationCatalog = BuildDestinationCatalog();

    public BookingController(
        ITravelPreferenceStore preferenceStore,
        ICebuRecommendationService recommendationService,
        SugboGoDbContext dbContext,
        ILogger<BookingController> logger)
    {
        _preferenceStore = preferenceStore;
        _recommendationService = recommendationService;
        _dbContext = dbContext;
        _logger = logger;
    }

    [Authorize]
    public async Task<IActionResult> Index(string? destination, decimal price = 5000, string? image = null)
    {
        if (string.IsNullOrWhiteSpace(destination))
        {
            destination = DefaultBookingDestination;
            price = DefaultBookingPrice;
            image = DefaultBookingImage;
        }

        var userId = GetUserId();
        var preferences = await _preferenceStore.FindLatestByUserIdAsync(userId);
        var destinationData = FindDestination(destination, price, image);

        var model = new BookingStepViewModel
        {
            CurrentStep = "details",
            Preferences = preferences,
            ActivityOptions = destinationData.Activities,
            AccommodationOptions = destinationData.Accommodations,
            TransportOptions = destinationData.TransportOptions,
            SmartRecommendations = BuildSmartRecommendations(preferences, destinationData),
            Data = new BookingDataViewModel
            {
                DestinationId = destinationData.Id,
                DestinationName = destinationData.Name,
                BasePrice = destinationData.BasePrice,
                ImageUrl = destinationData.ImageUrl,
                Description = destinationData.Description,
                Location = destinationData.Location,
                Duration = destinationData.Duration,
                BestTimeToVisit = destinationData.BestTimeToVisit,
                RatingSummary = destinationData.RatingSummary,
                MapUrl = destinationData.MapUrl,
                TotalPrice = destinationData.BasePrice,
                TravelerType = "Solo",
                TravelerCount = 1
            }
        };

        ViewData["Title"] = $"Book {destination}";
        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmBooking([FromBody] BookingDataViewModel data)
    {
        if (data == null || string.IsNullOrWhiteSpace(data.DestinationName))
        {
            return BadRequest();
        }

        var booking = new Booking
        {
            UserId = GetUserId(),
            DestinationName = data.DestinationName,
            ImageUrl = data.ImageUrl,
            Location = data.Location,
            TravelDate = EnsureUtc(data.TravelDate ?? DateTime.UtcNow.AddDays(7)),
            TravelerType = data.TravelerType,
            TravelerCount = data.TravelerCount,
            SelectedActivitiesJson = JsonSerializer.Serialize(data.SelectedActivities),
            SelectedAccommodationJson = JsonSerializer.Serialize(new { Name = data.SelectedAccommodation }),
            SelectedTransportationJson = JsonSerializer.Serialize(new { Name = data.SelectedTransportation }),
            BasePrice = data.BasePrice,
            AddOnsPrice = data.AddOnsPrice,
            TaxesAndFees = data.TaxesAndFees,
            TotalPrice = data.TotalPrice,
            TravelerNotes = data.TravelerNotes,
            Status = "Confirmed",
            PaymentMethod = string.IsNullOrWhiteSpace(data.PaymentMethod) ? "Card" : data.PaymentMethod.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Bookings.Add(booking);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Booking could not be saved for user {UserId}.", booking.UserId);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                success = false,
                message = "Booking is temporarily unavailable while the database finishes syncing. Please try again in a moment."
            });
        }
        catch (InvalidOperationException exception) when (exception.InnerException is not null)
        {
            _logger.LogError(exception, "Booking could not connect to the database for user {UserId}.", booking.UserId);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                success = false,
                message = "Booking is temporarily unavailable because the database connection timed out. Please try again."
            });
        }

        return Json(new { success = true, bookingId = booking.Id, qrCode = booking.QrCode });
    }

    public IActionResult Start()
    {
        var bookingUrl = BuildDefaultBookingUrl();

        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction("Index", "Account", new { returnUrl = bookingUrl });
        }

        return LocalRedirect(bookingUrl);
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

    private const string DefaultBookingDestination = "Kawasan Falls expedition";
    private const decimal DefaultBookingPrice = 2450m;
    private const string DefaultBookingImage = "/images/hero-bg.jpg";

    private string BuildDefaultBookingUrl()
    {
        return Url.Action(nameof(Index), "Booking", new
        {
            destination = DefaultBookingDestination,
            price = DefaultBookingPrice,
            image = DefaultBookingImage
        }) ?? "/Booking";
    }

    private static BookingDestinationSeed FindDestination(string destination, decimal price, string? image)
    {
        var key = NormalizeDestinationKey(destination);
        if (DestinationCatalog.TryGetValue(key, out var seed))
        {
            return seed;
        }

        return new BookingDestinationSeed(
            key,
            destination.Trim(),
            string.IsNullOrWhiteSpace(image) ? "https://images.unsplash.com/photo-1518509562904-e7ef99cdcc86?auto=format&fit=crop&w=1200&q=80" : image,
            $"A Cebu travel experience for {destination.Trim()}, matched to your saved traveler profile.",
            "Cebu, Philippines",
            "1-2 days",
            "4.7 (community rated)",
            "November to May",
            "https://www.openstreetmap.org/export/embed.html?bbox=123.70%2C9.50%2C124.20%2C11.40&layer=mapnik",
            price,
            BuildDefaultActivities(),
            BuildDefaultAccommodations(),
            BuildDefaultTransport());
    }

    private static List<string> BuildSmartRecommendations(TravelPreferenceRecord? preferences, BookingDestinationSeed destination)
    {
        var interests = preferences?.Interests.Select(i => i.Trim().ToLowerInvariant()).ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];
        var recommendations = new List<string>();

        if (interests.Contains("hiking") || preferences?.AdventureLevel >= 4)
        {
            recommendations.Add("Since your profile leans adventurous, add a private guide for safer hidden-trail access.");
            recommendations.Add("Pair this trip with Osmena Peak Sunrise Trek if your travel date has a clear early forecast.");
        }

        if (interests.Contains("food") || interests.Contains("culture"))
        {
            recommendations.Add("Add a local food stop near the route so your itinerary is not just transport and scenery.");
        }

        if (string.Equals(preferences?.BudgetRange, "Budget", StringComparison.OrdinalIgnoreCase))
        {
            recommendations.Add("Shared transport and hostel stay keep this trip inside a budget-friendly range.");
        }

        recommendations.Add($"Best matched schedule: {destination.BestTimeToVisit}, with support check-ins before departure.");
        return recommendations.Distinct().Take(4).ToList();
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Date, DateTimeKind.Utc)
        };
    }

    private static string NormalizeDestinationKey(string value)
    {
        return value.Trim().ToLowerInvariant().Replace(" ", "-").Replace("&", "and");
    }

    private static IReadOnlyDictionary<string, BookingDestinationSeed> BuildDestinationCatalog()
    {
        var items = new[]
        {
            new BookingDestinationSeed(
                "kawasan-falls-expedition",
                "Kawasan Falls expedition",
                "/images/hero-bg.jpg",
                "Canyoneering through turquoise Badian gorges with local guides, safety gear, cliff-jump options, and post-adventure recovery stops.",
                "Badian, Cebu",
                "6-7 hours",
                "4.9 (1,200 reviews)",
                "December to May",
                "https://www.openstreetmap.org/export/embed.html?bbox=123.33%2C9.76%2C123.46%2C9.88&layer=mapnik&marker=9.8025%2C123.3747",
                2450m,
                BuildDefaultActivities(),
                BuildDefaultAccommodations(),
                BuildDefaultTransport()),
            new BookingDestinationSeed(
                "osmena-peak-and-moalboal-sardines",
                "Osmena Peak and Moalboal sardines",
                "/images/hero-bg.jpg",
                "A mountain-to-sea Cebu day: sunrise ridgelines at Osmena Peak followed by the Moalboal sardine run and reef swim.",
                "Dalaguete and Moalboal, Cebu",
                "10-11 hours",
                "4.8 (860 reviews)",
                "November to April",
                "https://www.openstreetmap.org/export/embed.html?bbox=123.26%2C9.84%2C123.54%2C10.18&layer=mapnik&marker=9.8192%2C123.4376",
                3250m,
                BuildDefaultActivities(),
                BuildDefaultAccommodations(),
                BuildDefaultTransport()),
            new BookingDestinationSeed(
                "lechon-legacy-heritage-tour",
                "Lechon legacy and heritage tour",
                "/images/hero-bg.jpg",
                "Cebu City heritage landmarks, local food stops, and guided context for travelers who want the city beyond a photo route.",
                "Cebu City, Cebu",
                "5 hours",
                "4.7 (540 reviews)",
                "Year-round",
                "https://www.openstreetmap.org/export/embed.html?bbox=123.86%2C10.27%2C123.94%2C10.34&layer=mapnik&marker=10.2930%2C123.9020",
                1990m,
                BuildDefaultActivities(),
                BuildDefaultAccommodations(),
                BuildDefaultTransport())
        };

        return items.ToDictionary(item => item.Id, item => item, StringComparer.OrdinalIgnoreCase);
    }

    private static List<BookingActivityOption> BuildDefaultActivities() =>
    [
        new() { Id = "snorkeling", Name = "Snorkeling", Price = 500m, Description = "Mask, fins, and guide-supported swim window." },
        new() { Id = "private-guide", Name = "Private guide", Price = 1500m, Description = "A vetted local guide focused on pacing, safety, and stories." },
        new() { Id = "drone-photography", Name = "Drone photography", Price = 2500m, Description = "Aerial clips and stills delivered after the trip." },
        new() { Id = "island-hopping", Name = "Island hopping", Price = 3000m, Description = "Extended boat route to sandbars and quieter swim spots." },
        new() { Id = "transport-addon", Name = "Transportation support", Price = 700m, Description = "Door-to-door pickup coordination and schedule monitoring." }
    ];

    private static List<BookingAccommodationOption> BuildDefaultAccommodations() =>
    [
        new() { Id = "hostel", Name = "Flashpacker hostel", Type = "Hostel", PricePerNight = 0m, Amenities = "AC dorm, lockers, social lounge", Distance = "Near pickup point", Rating = "4.5", ImageUrl = "/images/hero-bg.jpg" },
        new() { Id = "hotel", Name = "Boutique hotel", Type = "Hotel", PricePerNight = 3500m, Amenities = "Private room, breakfast, pool", Distance = "10-20 min from route", Rating = "4.7", ImageUrl = "/images/hero-bg.jpg" },
        new() { Id = "resort", Name = "Seaside resort", Type = "Resort", PricePerNight = 5200m, Amenities = "Beach access, concierge, breakfast", Distance = "Beachfront area", Rating = "4.8", ImageUrl = "/images/hero-bg.jpg" },
        new() { Id = "villa", Name = "Private villa", Type = "Private villa", PricePerNight = 7000m, Amenities = "Private deck, kitchen, quiet stay", Distance = "Private transfer recommended", Rating = "4.9", ImageUrl = "/images/hero-bg.jpg" }
    ];

    private static List<BookingTransportOption> BuildDefaultTransport() =>
    [
        new() { Id = "shared-van", Name = "Shared van", Price = 0m, Details = "Scheduled pickup with other travelers." },
        new() { Id = "private-car", Name = "Private car", Price = 2000m, Details = "Flexible departure and direct drop-off." },
        new() { Id = "ferry", Name = "Ferry coordination", Price = 1200m, Details = "Ticket guidance for island routes." },
        new() { Id = "airport-transfer", Name = "Airport transfer", Price = 1800m, Details = "Mactan airport pickup or drop-off." },
        new() { Id = "motorbike", Name = "Motorbike rental", Price = 800m, Details = "Best for confident riders on shorter routes." }
    ];

    private sealed record BookingDestinationSeed(
        string Id,
        string Name,
        string ImageUrl,
        string Description,
        string Location,
        string Duration,
        string RatingSummary,
        string BestTimeToVisit,
        string MapUrl,
        decimal BasePrice,
        List<BookingActivityOption> Activities,
        List<BookingAccommodationOption> Accommodations,
        List<BookingTransportOption> TransportOptions);
}
