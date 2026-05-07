using SugboGo.Models;

namespace SugboGo.Services.Travel;

public interface ITravelPreferenceStore
{
    Task<TravelPreferenceRecord?> FindLatestByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<TravelPreferenceRecord> SaveAsync(TravelPreferenceRecord preference, CancellationToken cancellationToken = default);
}
