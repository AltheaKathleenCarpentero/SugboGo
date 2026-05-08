using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SugboGo.Models;

public sealed class TravelPreferenceSurveyViewModel
{
    public IReadOnlyList<TravelInterestOption> InterestOptions { get; init; } = TravelInterestCatalog.Options;

    [Required(ErrorMessage = "Choose at least one Cebu interest.")]
    public List<string> SelectedInterests { get; set; } = [];

    [Range(1, 5)]
    [Display(Name = "Adventure level")]
    public int AdventureLevel { get; set; } = 3;

    [Required]
    [Display(Name = "Travel pace")]
    public string TravelPace { get; set; } = "Balanced";

    [Required]
    [Display(Name = "Budget range")]
    public string BudgetRange { get; set; } = "Mid-range";

    [Display(Name = "Notes for the curator")]
    [StringLength(500)]
    public string? Notes { get; set; }
}

public sealed class TravelPreferenceRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public UserAccount? User { get; set; }

    public string Email { get; set; } = string.Empty;
    public List<string> Interests { get; set; } = [];
    public int AdventureLevel { get; set; }
    public string TravelPace { get; set; } = string.Empty;
    public string BudgetRange { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public sealed record TravelInterestOption(string Key, string Label, string Description);

public sealed record CebuDestination(
    string Name,
    string Location,
    string Summary,
    IReadOnlyList<string> Categories,
    int AdventureLevel,
    string BestFor,
    string ImageUrl);

public sealed record RecommendedDestination(
    CebuDestination Destination,
    int MatchScore,
    IReadOnlyList<string> MatchedInterests,
    string Reason);

public sealed class TravelRecommendationsViewModel
{
    public TravelPreferenceRecord Preferences { get; init; } = new();
    public IReadOnlyList<RecommendedDestination> Recommendations { get; init; } = [];
    public IReadOnlyList<AiRecommendationStep> Explanation { get; init; } = [];
}

public sealed record AiRecommendationStep(string Title, string Body);

public static class TravelInterestCatalog
{
    public static IReadOnlyList<TravelInterestOption> Options { get; } =
    [
        new("beaches", "Beaches", "Island hopping, reefs, coves, and slower coastal days."),
        new("hiking", "Hiking", "Peaks, waterfalls, sunrise trails, and mountain towns."),
        new("nightlife", "Nightlife and clubs", "Rooftops, live music, cocktail rooms, and late Cebu energy."),
        new("food", "Food trips", "Markets, lechon, seafood, heritage snacks, and chef-led tables."),
        new("history", "Historical places", "Churches, forts, museums, old streets, and ancestral houses."),
        new("adventure", "Adventure activities", "Canyoneering, diving, sardine runs, caves, and high-energy routes."),
        new("wellness", "Wellness and slow travel", "Quiet stays, spa time, scenic cafes, and restorative pacing."),
        new("culture", "Local culture", "Community-led tours, festivals, crafts, and neighborhood stories.")
    ];
}
