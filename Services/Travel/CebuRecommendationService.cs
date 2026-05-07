using SugboGo.Models;

namespace SugboGo.Services.Travel;

public interface ICebuRecommendationService
{
    TravelRecommendationsViewModel BuildRecommendations(TravelPreferenceRecord preferences);
}

public sealed class CebuRecommendationService : ICebuRecommendationService
{
    private static readonly IReadOnlyList<CebuDestination> Destinations =
    [
        new("Bantayan Island White Beach", "Bantayan Island", "Wide sand, mellow villages, bikeable coastal roads, and sunset seafood.", ["beaches", "wellness", "food"], 2, "Travelers who want a soft beach reset", "https://images.unsplash.com/photo-1500375592092-40eb2168fd21?auto=format&fit=crop&w=900&q=80"),
        new("Moalboal Sardine Run", "Moalboal", "A shore-entry reef experience with turtles, sardine clouds, and easy cafe stops nearby.", ["beaches", "adventure"], 4, "Water lovers who still want a comfortable base", "https://images.unsplash.com/photo-1544551763-46a013bb70d5?auto=format&fit=crop&w=900&q=80"),
        new("Osmena Peak Dawn Trail", "Dalaguete", "Cebu's highest viewpoint, best timed for sunrise and a slow breakfast after descent.", ["hiking", "adventure", "wellness"], 3, "Mountain mornings without a brutal trek", "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=900&q=80"),
        new("Kawasan Canyoneering", "Badian", "River jumps, limestone corridors, guides, and a high-energy route through blue water.", ["adventure", "hiking"], 5, "Travelers who asked for adrenaline", "https://images.unsplash.com/photo-1519451241324-20b4ea2c4220?auto=format&fit=crop&w=900&q=80"),
        new("Carbon Market Food Crawl", "Cebu City", "Street food, local produce, heritage bites, and vendor stories in one walkable route.", ["food", "culture", "history"], 2, "Curious eaters who want Cebu through taste", "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?auto=format&fit=crop&w=900&q=80"),
        new("Fort San Pedro and Colon Heritage Walk", "Cebu City", "A compact route through fort walls, old streets, churches, museums, and snack stops.", ["history", "culture", "food"], 1, "First-timers who want context before exploring farther", "https://images.unsplash.com/photo-1518005020951-eccb494ad742?auto=format&fit=crop&w=900&q=80"),
        new("IT Park Rooftop and Live Music Night", "Cebu City", "Cocktail rooms, casual clubs, late bites, and safe transfer-friendly nightlife.", ["nightlife", "food"], 2, "Travelers who want city energy after dark", "https://images.unsplash.com/photo-1514525253161-7a46d19cd819?auto=format&fit=crop&w=900&q=80"),
        new("Sirao Garden and Busay Ridge Cafes", "Cebu City Highlands", "Mountain gardens, city views, scenic cafes, and a flexible half-day route.", ["culture", "wellness", "food"], 1, "Easy views and soft local texture", "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?auto=format&fit=crop&w=900&q=80")
    ];

    public TravelRecommendationsViewModel BuildRecommendations(TravelPreferenceRecord preferences)
    {
        var selected = preferences.Interests.Select(Normalize).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var recommendations = Destinations
            .Select(destination => Score(destination, selected, preferences))
            .OrderByDescending(item => item.MatchScore)
            .ThenBy(item => Math.Abs(item.Destination.AdventureLevel - preferences.AdventureLevel))
            .Take(4)
            .ToList();

        return new TravelRecommendationsViewModel
        {
            Preferences = preferences,
            Recommendations = recommendations,
            Explanation =
            [
                new("Preference collection", "The survey stores selected interests, adventure level, budget range, travel pace, and optional notes against the signed-in user ID and Gmail address."),
                new("Destination categories", "Each Cebu destination has searchable tags such as beach, hiking, nightlife, food, history, adventure, wellness, and culture, plus metadata for intensity and trip style."),
                new("AI matching", "The first version uses transparent scoring: category overlap, adventure fit, and travel pace. In production, this scoring can be sent to an LLM or embeddings model to explain and re-rank results."),
                new("Continuous learning", "Clicks, swaps, saved places, completed bookings, ratings, and skipped recommendations can adjust category weights so future suggestions become more personal.")
            ]
        };
    }

    private static RecommendedDestination Score(CebuDestination destination, HashSet<string> selected, TravelPreferenceRecord preferences)
    {
        var matched = destination.Categories.Where(category => selected.Contains(category)).ToList();
        var interestScore = matched.Count * 25;
        var adventureFit = Math.Max(0, 20 - (Math.Abs(destination.AdventureLevel - preferences.AdventureLevel) * 5));
        var paceBoost = preferences.TravelPace.Equals("Relaxed", StringComparison.OrdinalIgnoreCase) && destination.AdventureLevel <= 2 ? 8 : 0;
        paceBoost += preferences.TravelPace.Equals("Packed", StringComparison.OrdinalIgnoreCase) && destination.Categories.Contains("adventure") ? 8 : 0;
        var score = Math.Clamp(interestScore + adventureFit + paceBoost, 0, 98);

        var reason = matched.Count > 0
            ? $"Matches your {string.Join(", ", matched.Select(ToLabel))} interests with a {preferences.TravelPace.ToLowerInvariant()} pace."
            : $"Adds variety while staying close to your adventure level of {preferences.AdventureLevel}/5.";

        return new RecommendedDestination(destination, score, matched.Select(ToLabel).ToList(), reason);
    }

    private static string Normalize(string value) => value.Trim().ToLowerInvariant();

    private static string ToLabel(string key)
    {
        return TravelInterestCatalog.Options.FirstOrDefault(option => option.Key == key)?.Label ?? key;
    }
}
