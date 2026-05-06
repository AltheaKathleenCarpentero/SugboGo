using Microsoft.Extensions.Options;

namespace SugboGo.Services.Auth;

public sealed class UserAccountStoreFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly SupabaseOptions _options;

    public UserAccountStoreFactory(IServiceProvider serviceProvider, IConfiguration configuration, IOptions<SupabaseOptions> options)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _options = options.Value;
    }

    public IUserAccountStore Create()
    {
        if (!string.IsNullOrWhiteSpace(_configuration.GetConnectionString("DefaultConnection")))
        {
            return _serviceProvider.GetRequiredService<PostgresUserAccountStore>();
        }

        if (!string.IsNullOrWhiteSpace(_options.Url) && !string.IsNullOrWhiteSpace(_options.ServiceRoleKey))
        {
            return _serviceProvider.GetRequiredService<SupabaseUserAccountStore>();
        }

        return _serviceProvider.GetRequiredService<LocalJsonUserAccountStore>();
    }
}
