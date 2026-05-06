using SugboGo.Models;

namespace SugboGo.Services.Admin;

public sealed class AdminOperationsService : IAdminOperationsService
{
    public AdminDashboardViewModel BuildDashboard()
    {
        return new AdminDashboardViewModel
        {
            Kpis =
            [
                new() { Label = "Active Adventures", Value = "18", Delta = "6 travelers currently in Cebu City" },
                new() { Label = "Pending Curations", Value = "11", Delta = "4 need first draft today" },
                new() { Label = "Revenue", Value = "PHP 842K", Delta = "+18% this month" },
                new() { Label = "Partner Holds", Value = "37", Delta = "9 expire within 24h" }
            ],
            VibeTrends =
            [
                new() { Vibe = "Urban Explorer", Percentage = 60 },
                new() { Vibe = "Heritage Hunter", Percentage = 30 },
                new() { Vibe = "Island Minimalist", Percentage = 7 },
                new() { Vibe = "Soft Adventure", Percentage = 3 }
            ],
            UrgentAlerts =
            [
                new() { Traveler = "Mika Santos", Issue = "Transport delay near Carbon Market", Location = "Carbon Market", ChatUrl = "https://wa.me/639170002841" },
                new() { Traveler = "Jon Reed", Issue = "Guide awaiting weather approval", Location = "Busay ridge", ChatUrl = "https://wa.me/639170002841" },
                new() { Traveler = "Lea Tan", Issue = "Dietary note missing from dinner partner", Location = "Kamagayan", ChatUrl = "https://wa.me/639170002841" }
            ],
            Gems =
            [
                new() { Name = "Hidden Heritage Cafe", Category = "Cafe", FlashpackerScore = 9, QualityCheckDate = "May 1, 2026", ContactPerson = "Ana Lim", Latitude = 10.2961m, Longitude = 123.8993m, Status = "Active", MapX = 34, MapY = 58 },
                new() { Name = "Private Mountain View", Category = "Viewpoint", FlashpackerScore = 8, QualityCheckDate = "Apr 22, 2026", ContactPerson = "Ramon Uy", Latitude = 10.3713m, Longitude = 123.8830m, Status = "Seasonal", MapX = 47, MapY = 28 },
                new() { Name = "Museo Alley Studio", Category = "Museum", FlashpackerScore = 7, QualityCheckDate = "Apr 18, 2026", ContactPerson = "Tessa Co", Latitude = 10.3002m, Longitude = 123.8967m, Status = "Under Review", MapX = 39, MapY = 54 },
                new() { Name = "Curated Rooftop Dinner", Category = "Dining", FlashpackerScore = 10, QualityCheckDate = "May 3, 2026", ContactPerson = "Marco Dizon", Latitude = 10.3190m, Longitude = 123.9057m, Status = "Active", MapX = 63, MapY = 48 }
            ],
            Templates =
            [
                new() { Name = "Urban Explorer 36h", Vibe = "Urban Explorer", Stops = "Cafe, gallery, rooftop dinner", AvgDuration = "1.5 days" },
                new() { Name = "Heritage Hunter Core", Vibe = "Heritage Hunter", Stops = "Parian walk, museum, ancestral supper", AvgDuration = "1 day" },
                new() { Name = "Soft Mountain Reset", Vibe = "Soft Adventure", Stops = "Design stay, ridge route, garden hideout", AvgDuration = "2 days" }
            ],
            Pipeline = BuildPipeline(),
            Flashpackers =
            [
                new() { Name = "Mika Santos", Vibe = "Urban Explorer", Constraints = "Allergic to seafood, private cars only", Feedback = "Hidden cafe was great, road was rough for luggage.", CebuTrips = 2 },
                new() { Name = "Andre Costa", Vibe = "Island Minimalist", Constraints = "No early hikes, prefers quiet rooms", Feedback = "Loved the low-crowd beach timing.", CebuTrips = 1 },
                new() { Name = "Priya Shah", Vibe = "Heritage Hunter", Constraints = "Vegetarian, needs flexible pickup", Feedback = "Guide pacing was excellent.", CebuTrips = 3 }
            ],
            Partners =
            [
                new() { Name = "The Helix House", Type = "Boutique hotel", Contact = "Nina Yu", Commission = "15%", LastAudit = "Apr 30, 2026", Status = "Excellent" },
                new() { Name = "South Ridge Guides", Type = "Local guide", Contact = "Ramon Uy", Commission = "Per route", LastAudit = "Apr 22, 2026", Status = "Watch weather" },
                new() { Name = "Red Door Supper Club", Type = "Dining", Contact = "Marco Dizon", Commission = "12%", LastAudit = "May 3, 2026", Status = "Excellent" }
            ],
            CollaborationQueue =
            [
                new() { SuggestedBy = "Local Expert: Ana", Spot = "Mactan Ceramic Courtyard", Reason = "Strong design signal for Urban Explorer profiles.", ApprovalStatus = "Needs admin approval" },
                new() { SuggestedBy = "Developer: Ken", Spot = "Liloan Moon Tide Table", Reason = "Route engine says it pairs well with north transfers.", ApprovalStatus = "Map validation pending" }
            ]
        };
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
}
