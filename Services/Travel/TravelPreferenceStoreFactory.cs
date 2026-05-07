using Microsoft.Extensions.Options;
using SugboGo.Services.Auth;

namespace SugboGo.Services.Travel;

public sealed class TravelPreferenceStoreFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly SupabaseOptions _options;

    public TravelPreferenceStoreFactory(IServiceProvider serviceProvider, IConfiguration configuration, IOptions<SupabaseOptions> options)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _options = options.Value;
    }

    public ITravelPreferenceStore Create()
    {
        if (!string.IsNullOrWhiteSpace(_configuration.GetConnectionString("DefaultConnection")))
        {
            return _serviceProvider.GetRequiredService<PostgresTravelPreferenceStore>();
        }

        if (!string.IsNullOrWhiteSpace(_options.Url) && !string.IsNullOrWhiteSpace(_options.ServiceRoleKey))
        {
            return _serviceProvider.GetRequiredService<SupabaseTravelPreferenceStore>();
        }

        return _serviceProvider.GetRequiredService<LocalJsonTravelPreferenceStore>();
    }
}
