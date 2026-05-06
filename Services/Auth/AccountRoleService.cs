using Microsoft.Extensions.Options;

namespace SugboGo.Services.Auth;

public sealed class AccountRoleService : IAccountRoleService
{
    private readonly HashSet<string> _adminEmails;

    public AccountRoleService(IOptions<AccountRoleOptions> options)
    {
        _adminEmails = options.Value.AdminEmails
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .Select(NormalizeEmail)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public string GetRegistrationRole(string email)
    {
        return _adminEmails.Contains(NormalizeEmail(email))
            ? AccountRoles.Admin
            : AccountRoles.Client;
    }

    public string ResolveEffectiveRole(string email, string? storedRole)
    {
        if (_adminEmails.Contains(NormalizeEmail(email)))
        {
            return AccountRoles.Admin;
        }

        return AccountRoles.Normalize(storedRole);
    }

    private static string NormalizeEmail(string email) => (email ?? string.Empty).Trim().ToLowerInvariant();
}
