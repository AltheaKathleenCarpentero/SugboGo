using System.Security.Claims;
using SugboGo.Models;

namespace SugboGo.Services.Dashboard;

public interface IDashboardExperienceService
{
    DashboardViewModel BuildForUser(ClaimsPrincipal user);
}
