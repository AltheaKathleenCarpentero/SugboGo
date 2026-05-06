using SugboGo.Models;

namespace SugboGo.Services.Admin;

public interface IAdminOperationsService
{
    AdminDashboardViewModel BuildDashboard();
}
