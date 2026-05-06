namespace SugboGo.Services.Auth;

public static class AccountRoles
{
    public const string Admin = "Admin";
    public const string Client = "Client";
    public const string AdminOrClient = Admin + "," + Client;

    public static string Normalize(string? role)
    {
        return string.Equals(role, Admin, StringComparison.OrdinalIgnoreCase)
            ? Admin
            : Client;
    }
}
