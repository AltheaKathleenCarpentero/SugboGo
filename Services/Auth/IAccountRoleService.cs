namespace SugboGo.Services.Auth;

public interface IAccountRoleService
{
    string GetRegistrationRole(string email);
    string ResolveEffectiveRole(string email, string? storedRole);
}
