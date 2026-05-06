using System.Security.Claims;
using SugboGo.Models;

namespace SugboGo.Services.Dashboard;

public sealed class DashboardExperienceService : IDashboardExperienceService
{
    public DashboardViewModel BuildForUser(ClaimsPrincipal user)
    {
        var fullName = user.FindFirstValue(ClaimTypes.Name) ?? "Traveler";
        var email = user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var firstName = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "Traveler";
        var seed = Math.Abs(email.GetHashCode());
        var hasTrip = !email.Contains("notrip", StringComparison.OrdinalIgnoreCase);

        return new DashboardViewModel
        {
            FirstName = firstName,
            Greeting = BuildGreeting(firstName),
            ActiveTrip = hasTrip ? BuildActiveTrip(firstName, seed) : null,
            VibeTags = BuildVibeTags(seed),
            CuratedGems = BuildCuratedGems(seed),
            Bookings = BuildBookings(hasTrip),
            SavedGems = BuildSavedGems(seed),
            PastAdventures = BuildPastAdventures(seed),
            TravelProfile = BuildTravelProfile(seed),
            FeatureSuggestions = BuildFeatureSuggestions()
        };
    }

    private static string BuildGreeting(string firstName)
    {
        var hour = DateTime.Now.Hour;
        var dayPart = hour < 12 ? "Good morning" : hour < 18 ? "Good afternoon" : "Good evening";
        return $"{dayPart}, {firstName}";
    }

    private static ActiveTripViewModel BuildActiveTrip(string firstName, int seed)
    {
        var startDate = DateTime.Today.AddDays(seed % 2 == 0 ? 3 : -1);
        var status = DateTime.Today >= startDate
            ? "You are currently in Cebu!"
            : $"Your Cebu Adventure begins in {(startDate - DateTime.Today).Days} days!";

        var stops = new List<ItineraryStopViewModel>
        {
            new() { Time = "10:00 AM", Title = "Hidden Heritage Cafe", Location = "Parian", Type = "Coffee ritual", PartnerStatus = "Priority table ready" },
            new() { Time = "2:00 PM", Title = "Private Mountain View", Location = "Busay", Type = "Soft adventure", PartnerStatus = "Guide confirmed" },
            new() { Time = "7:00 PM", Title = "Curated Rooftop Dinner", Location = "Cebu Business Park", Type = "Secret table", PartnerStatus = "Sogbo-Key perk active" }
        };

        return new ActiveTripViewModel
        {
            Title = $"{firstName}'s Urban Explorer Route",
            Status = status,
            DateRange = $"{startDate:MMM d} to {startDate.AddDays(2):MMM d, yyyy}",
            Hotel = "The Helix House, Banawa ridge",
            SogboKeyCode = $"SG-{DateTime.Today:MMdd}-{seed % 9000 + 1000}",
            Stops = stops,
            MapPins =
            [
                new() { Label = "Heritage Cafe", Time = "10:00 AM", Latitude = 10.2961m, Longitude = 123.8993m, X = 34, Y = 58 },
                new() { Label = "Mountain View", Time = "2:00 PM", Latitude = 10.3713m, Longitude = 123.8830m, X = 47, Y = 28 },
                new() { Label = "Rooftop Dinner", Time = "7:00 PM", Latitude = 10.3190m, Longitude = 123.9057m, X = 63, Y = 48 }
            ]
        };
    }

    private static List<VibeTagViewModel> BuildVibeTags(int seed)
    {
        var activeIndex = seed % 4;
        var tags = new[] { "Urban Explorer", "Island Minimalist", "Street Food Safe", "Design Stays", "Soft Adventure", "After-Dark Cebu" };

        return tags.Select((tag, index) => new VibeTagViewModel { Label = tag, IsActive = index == activeIndex || index == 0 }).ToList();
    }

    private static List<GemRecommendationViewModel> BuildCuratedGems(int seed)
    {
        var gems = new List<GemRecommendationViewModel>
        {
            new() { Title = "Kamagayan Vinyl Supper", Category = "Secret dining", Neighborhood = "Kamagayan", MatchReason = "Quiet room, smoky seafood, chef-hosted", ImageUrl = "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?auto=format&fit=crop&w=900&q=80", MatchScore = 96 },
            new() { Title = "Mactan Ceramic Courtyard", Category = "Hidden gallery", Neighborhood = "Mactan", MatchReason = "Design-led, low crowd, local maker access", ImageUrl = "https://images.unsplash.com/photo-1493106641515-6b5631de4bb9?auto=format&fit=crop&w=900&q=80", MatchScore = 92 },
            new() { Title = "Liloan Moon Tide Table", Category = "Coastal dinner", Neighborhood = "Liloan", MatchReason = "Sea breeze, clean kitchen, golden-hour transfer", ImageUrl = "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=900&q=80", MatchScore = 89 },
            new() { Title = "Busay Garden Hideout", Category = "Mountain pause", Neighborhood = "Busay", MatchReason = "Soft adventure with boutique comfort", ImageUrl = "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=900&q=80", MatchScore = 88 }
        };

        return gems.OrderBy(gem => (gem.MatchScore + seed) % 17).ToList();
    }

    private static List<BookingVaultItemViewModel> BuildBookings(bool hasTrip)
    {
        var bookings = new List<BookingVaultItemViewModel>();

        if (hasTrip)
        {
            bookings.Add(new() { Type = "Hotel", Title = "The Helix House", Date = DateTime.Today.AddDays(3).ToString("MMM d"), Status = "Confirmed" });
            bookings.Add(new() { Type = "Activity", Title = "Private Mountain View", Date = DateTime.Today.AddDays(4).ToString("MMM d"), Status = "Guide assigned" });
            bookings.Add(new() { Type = "Transport", Title = "Airport to Banawa", Date = DateTime.Today.AddDays(3).ToString("MMM d"), Status = "Driver pending" });
        }

        return bookings;
    }

    private static List<SavedGemViewModel> BuildSavedGems(int seed)
    {
        return
        [
            new() { Title = "Alcoy White Rock Swim", Note = seed % 2 == 0 ? "Save for a slow beach day" : "Pairs well with a south Cebu route" },
            new() { Title = "Parian After-Hours Walk", Note = "Marked for heritage and photo stops" },
            new() { Title = "North Reclamation Jazz Den", Note = "Great if you stay near the city" }
        ];
    }

    private static List<PastAdventureViewModel> BuildPastAdventures(int seed)
    {
        if (seed % 3 == 0)
        {
            return [];
        }

        return
        [
            new() { Title = "Old Cebu Food Crawl", Date = "March 2025" },
            new() { Title = "South Cebu Reef Reset", Date = "August 2025" }
        ];
    }

    private static TravelProfileViewModel BuildTravelProfile(int seed)
    {
        return new TravelProfileViewModel
        {
            StayPreference = seed % 2 == 0 ? "Boutique hotels over luxury chains" : "Quiet design stays near local food",
            FoodPreference = "Street food energy, high hygiene standards",
            PacePreference = "Two anchors per day, flexible in-between time",
            Notifications = "Hidden Gem alerts and itinerary changes enabled",
            PaymentSummary = "Visa ending 4242 ready for Quick-Pay"
        };
    }

    private static List<DashboardFeatureSuggestionViewModel> BuildFeatureSuggestions()
    {
        return
        [
            new() { Title = "Live partner check-ins", Description = "Let hotels, guides, and restaurants update arrival readiness in real time." },
            new() { Title = "Weather-aware route swaps", Description = "Automatically suggest indoor gems or safer beach timing when conditions change." },
            new() { Title = "Group vibe matching", Description = "Merge multiple travelers' quiz results into one route everyone can tolerate, maybe even love." },
            new() { Title = "Expense split and travel wallet", Description = "Let flashpacker groups split deposits, perks, and concierge add-ons inside SogboGo." }
        ];
    }
}
