using SugboGo.Models;
using SugboGo.Services.Auth;
using SugboGo.Services.Dashboard;
using SugboGo.Services.Travel;

namespace SugboGo.Services.Admin;

public sealed class AdminOperationsService : IAdminOperationsService
{
    private readonly IUserAccountStore _userStore;
    private readonly ITravelPreferenceStore _preferenceStore;
    private readonly IDestinationPostStore _postStore;
    private readonly IAdminDataStore _adminDataStore;

    public AdminOperationsService(
        IUserAccountStore userStore,
        ITravelPreferenceStore preferenceStore,
        IDestinationPostStore postStore,
        IAdminDataStore adminDataStore)
    {
        _userStore = userStore;
        _preferenceStore = preferenceStore;
        _postStore = postStore;
        _adminDataStore = adminDataStore;
    }

    public async Task<AdminDashboardViewModel> BuildDashboardAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userStore.GetAllAsync(cancellationToken);
        var preferences = await _preferenceStore.GetAllAsync(cancellationToken);
        var posts = await _postStore.GetAllAsync(cancellationToken);
        var gems = await _adminDataStore.GetGemsAsync(cancellationToken);
        var templates = await _adminDataStore.GetTemplatesAsync(cancellationToken);
        var partners = await _adminDataStore.GetPartnersAsync(cancellationToken);
        var latestUser = users.OrderByDescending(user => user.CreatedAt).FirstOrDefault();

        return new AdminDashboardViewModel
        {
            Kpis = BuildKpis(users, preferences, posts, latestUser),
            VibeTrends = BuildVibeTrends(preferences),
            UrgentAlerts = BuildUrgentAlerts(users, preferences),
            Gems = gems.Select(g => new GemAdminViewModel
            {
                Name = g.Name,
                Category = g.Category,
                FlashpackerScore = g.FlashpackerScore,
                QualityCheckDate = g.QualityCheckDate,
                ContactPerson = g.ContactPerson,
                Latitude = g.Latitude,
                Longitude = g.Longitude,
                Status = g.Status,
                MapX = g.MapX,
                MapY = g.MapY
            }).ToList(),
            Templates = templates.Select(t => new ItineraryTemplateViewModel
            {
                Name = t.Name,
                Vibe = t.Vibe,
                Stops = t.Stops,
                AvgDuration = t.AvgDuration
            }).ToList(),
            Pipeline = BuildPipeline(),
            Flashpackers = BuildFlashpackers(users, preferences, posts),
            Partners = partners.Select(p => new PartnerAdminViewModel
            {
                Name = p.Name,
                Type = p.Type,
                Contact = p.Contact,
                Commission = p.Commission,
                LastAudit = p.LastAudit,
                Status = p.Status
            }).ToList(),
            CollaborationQueue = BuildCollaborationQueue()
        };
    }

    private static List<AdminKpiViewModel> BuildKpis(
        IReadOnlyCollection<UserAccount> users,
        IReadOnlyCollection<TravelPreferenceRecord> preferences,
        IReadOnlyCollection<DestinationPost> posts,
        UserAccount? latestUser)
    {
        return
        [
            new() { Label = "Total Users", Value = users.Count.ToString(), Delta = $"{users.Count(user => AccountRoles.Normalize(user.Role) == AccountRoles.Client)} client account(s)" },
            new() { Label = "Total Posts", Value = posts.Count.ToString(), Delta = $"{posts.Sum(post => post.Likes)} community like(s)" },
            new() { Label = "Preferences Submitted", Value = preferences.Count.ToString(), Delta = $"{preferences.Select(preference => preference.UserId).Distinct().Count()} traveler profile(s)" },
            new() { Label = "Latest Signup", Value = latestUser?.CreatedAt.ToLocalTime().ToString("MMM d") ?? "None", Delta = latestUser?.Email ?? "No registered users yet" }
        ];
    }

    private static List<VibeTrendViewModel> BuildVibeTrends(IEnumerable<TravelPreferenceRecord> preferences)
    {
        var interestCounts = preferences
            .SelectMany(preference => preference.Interests)
            .Select(NormalizeInterest)
            .Where(interest => !string.IsNullOrWhiteSpace(interest))
            .GroupBy(interest => interest)
            .Select(group => new { Key = group.Key, Count = group.Count() })
            .OrderByDescending(item => item.Count)
            .ThenBy(item => item.Key)
            .ToList();

        var total = interestCounts.Sum(item => item.Count);

        if (total == 0)
        {
            return [new() { Vibe = "No preferences yet", Percentage = 0 }];
        }

        return interestCounts
            .Take(6)
            .Select(item => new VibeTrendViewModel
            {
                Vibe = BuildInterestLabel(item.Key),
                Percentage = (int)Math.Round(item.Count * 100m / total)
            })
            .ToList();
    }

    private static List<UrgentAlertViewModel> BuildUrgentAlerts(
        IReadOnlyCollection<UserAccount> users,
        IReadOnlyCollection<TravelPreferenceRecord> preferences)
    {
        var missingPreferences = users
            .Where(user => AccountRoles.Normalize(user.Role) == AccountRoles.Client)
            .Where(user => preferences.All(preference => preference.UserId != user.Id))
            .Take(3)
            .Select(user => new UrgentAlertViewModel
            {
                Traveler = user.FullName,
                Issue = "No travel preference survey submitted yet",
                Location = user.Email,
                ChatUrl = "https://wa.me/639170002841"
            })
            .ToList();

        return missingPreferences.Count == 0
            ? [new() { Traveler = "System", Issue = "All registered clients have preference data or no clients are registered yet.", Location = "SugboGo data flow", ChatUrl = "https://wa.me/639170002841" }]
            : missingPreferences;
    }

    private static List<FlashpackerProfileViewModel> BuildFlashpackers(
        IEnumerable<UserAccount> users,
        IEnumerable<TravelPreferenceRecord> preferences,
        IEnumerable<DestinationPost> posts)
    {
        var preferenceByUser = preferences
            .GroupBy(preference => preference.UserId)
            .ToDictionary(group => group.Key, group => group.OrderByDescending(preference => preference.UpdatedAt).First());
        var postCounts = posts.GroupBy(post => post.UserId).ToDictionary(group => group.Key, group => group.Count());

        var profiles = users.Select(user =>
        {
            preferenceByUser.TryGetValue(user.Id, out var preference);
            postCounts.TryGetValue(user.Id, out var postCount);
            var interests = preference?.Interests.Select(BuildInterestLabel).ToList() ?? [];

            return new FlashpackerProfileViewModel
            {
                Name = string.IsNullOrWhiteSpace(user.FullName) ? user.Email : user.FullName,
                Vibe = interests.FirstOrDefault() ?? "Profile pending",
                Constraints = preference is null
                    ? "No preference survey submitted yet."
                    : $"{preference.TravelPace} pace, {preference.BudgetRange}, adventure {preference.AdventureLevel}/5",
                Feedback = preference?.Notes ?? (interests.Count == 0 ? "No notes yet." : string.Join(", ", interests)),
                CebuTrips = postCount
            };
        }).ToList();

        return profiles.Count == 0
            ? [new() { Name = "No registered users yet", Vibe = "Waiting for data", Constraints = "Register an account to populate the CRM.", Feedback = "Live user data will appear here.", CebuTrips = 0 }]
            : profiles;
    }

    private static List<PipelineColumnViewModel> BuildPipeline()
    {
        return
        [
            new()
            {
                Name = "New Requests",
                Cards =
                [
                    new() { Traveler = "Maya Klein", Vibe = "Urban Explorer", TravelWindow = "May 14 to 16", Priority = "High" },
                    new() { Traveler = "Noah Reyes", Vibe = "Soft Adventure", TravelWindow = "May 18 to 20", Priority = "Normal" }
                ]
            },
            new()
            {
                Name = "In Curation",
                Cards =
                [
                    new() { Traveler = "Jon Reed", Vibe = "Island Minimalist", TravelWindow = "May 10 to 12", Priority = "Weather watch" }
                ]
            },
            new()
            {
                Name = "Awaiting User Approval",
                Cards =
                [
                    new() { Traveler = "Lea Tan", Vibe = "Heritage Hunter", TravelWindow = "May 11 to 13", Priority = "Dietary note" }
                ]
            },
            new()
            {
                Name = "Confirmed/Paid",
                Cards =
                [
                    new() { Traveler = "Priya Shah", Vibe = "Heritage Hunter", TravelWindow = "May 9 to 11", Priority = "Paid" }
                ]
            },
            new()
            {
                Name = "In Progress",
                Cards =
                [
                    new() { Traveler = "Mika Santos", Vibe = "Urban Explorer", TravelWindow = "Now", Priority = "Transport alert" }
                ]
            },
            new()
            {
                Name = "Completed",
                Cards =
                [
                    new() { Traveler = "Andre Costa", Vibe = "Island Minimalist", TravelWindow = "May 1 to 3", Priority = "Feedback due" }
                ]
            }
        ];
    }

    private static List<PartnerAdminViewModel> BuildPartners()
    {
        return
        [
            new() { Name = "The Helix House", Type = "Boutique hotel", Contact = "Nina Yu", Commission = "15%", LastAudit = "Apr 30, 2026", Status = "Excellent" },
            new() { Name = "South Ridge Guides", Type = "Local guide", Contact = "Ramon Uy", Commission = "Per route", LastAudit = "Apr 22, 2026", Status = "Watch weather" },
            new() { Name = "Red Door Supper Club", Type = "Dining", Contact = "Marco Dizon", Commission = "12%", LastAudit = "May 3, 2026", Status = "Excellent" }
        ];
    }

    private static List<CollaborationSuggestionViewModel> BuildCollaborationQueue()
    {
        return
        [
            new() { SuggestedBy = "Local Expert: Ana", Spot = "Mactan Ceramic Courtyard", Reason = "Strong design signal for Urban Explorer profiles.", ApprovalStatus = "Needs admin approval" },
            new() { SuggestedBy = "Developer: Ken", Spot = "Liloan Moon Tide Table", Reason = "Route engine says it pairs well with north transfers.", ApprovalStatus = "Map validation pending" }
        ];
    }

    private static string BuildInterestLabel(string key)
    {
        return TravelInterestCatalog.Options.FirstOrDefault(option => option.Key == NormalizeInterest(key))?.Label
            ?? key.Trim();
    }

    private static string NormalizeInterest(string value) => value.Trim().ToLowerInvariant().Replace(" ", "-");
}
