using System.Text.Json;
using Npgsql;
using SugboGo.Models;

namespace SugboGo.Services.Travel;

public sealed class PostgresTravelPreferenceStore : ITravelPreferenceStore
{
    private static readonly SemaphoreSlim InitializationLock = new(1, 1);
    private static bool _initialized;
    private readonly string _connectionString;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public PostgresTravelPreferenceStore(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");
    }

    public async Task<List<TravelPreferenceRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);

        const string sql = """
            select id, user_id, email, interests_json, adventure_level, travel_pace, budget_range, notes, created_at, updated_at
            from sogbogo_travel_preferences
            order by updated_at desc;
            """;

        var preferences = new List<TravelPreferenceRecord>();
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            preferences.Add(ReadPreference(reader));
        }

        return preferences;
    }

    public async Task<TravelPreferenceRecord?> FindLatestByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);

        const string sql = """
            select id, user_id, email, interests_json, adventure_level, travel_pace, budget_range, notes, created_at, updated_at
            from sogbogo_travel_preferences
            where user_id = @userId
            order by updated_at desc
            limit 1;
            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("userId", userId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken)
            ? ReadPreference(reader)
            : null;
    }

    public async Task<TravelPreferenceRecord> SaveAsync(TravelPreferenceRecord preference, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);

        preference.UpdatedAt = DateTimeOffset.UtcNow;

        const string sql = """
            insert into sogbogo_travel_preferences
                (id, user_id, email, interests_json, adventure_level, travel_pace, budget_range, notes, created_at, updated_at)
            values
                (@id, @userId, @email, @interestsJson, @adventureLevel, @travelPace, @budgetRange, @notes, @createdAt, @updatedAt)
            on conflict (user_id) do update set
                email = excluded.email,
                interests_json = excluded.interests_json,
                adventure_level = excluded.adventure_level,
                travel_pace = excluded.travel_pace,
                budget_range = excluded.budget_range,
                notes = excluded.notes,
                updated_at = excluded.updated_at;
            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", preference.Id);
        command.Parameters.AddWithValue("userId", preference.UserId);
        command.Parameters.AddWithValue("email", preference.Email);
        command.Parameters.AddWithValue("interestsJson", JsonSerializer.Serialize(preference.Interests, _jsonOptions));
        command.Parameters.AddWithValue("adventureLevel", preference.AdventureLevel);
        command.Parameters.AddWithValue("travelPace", preference.TravelPace);
        command.Parameters.AddWithValue("budgetRange", preference.BudgetRange);
        command.Parameters.AddWithValue("notes", (object?)preference.Notes ?? DBNull.Value);
        command.Parameters.AddWithValue("createdAt", preference.CreatedAt);
        command.Parameters.AddWithValue("updatedAt", preference.UpdatedAt);

        await command.ExecuteNonQueryAsync(cancellationToken);
        return preference;
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
        {
            return;
        }

        await InitializationLock.WaitAsync(cancellationToken);
        try
        {
            if (_initialized)
            {
                return;
            }

            const string sql = """
                create table if not exists sogbogo_travel_preferences (
                    id text primary key,
                    user_id text not null unique,
                    email text not null,
                    interests_json text not null,
                    adventure_level integer not null,
                    travel_pace text not null,
                    budget_range text not null,
                    notes text null,
                    created_at timestamptz not null default now(),
                    updated_at timestamptz not null default now()
                );

                create index if not exists ix_sogbogo_travel_preferences_user_id
                    on sogbogo_travel_preferences (user_id);
                """;

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);

            _initialized = true;
        }
        finally
        {
            InitializationLock.Release();
        }
    }

    private TravelPreferenceRecord ReadPreference(NpgsqlDataReader reader)
    {
        return new TravelPreferenceRecord
        {
            Id = reader.GetString(0),
            UserId = reader.GetString(1),
            Email = reader.GetString(2),
            Interests = JsonSerializer.Deserialize<List<string>>(reader.GetString(3), _jsonOptions) ?? [],
            AdventureLevel = reader.GetInt32(4),
            TravelPace = reader.GetString(5),
            BudgetRange = reader.GetString(6),
            Notes = reader.IsDBNull(7) ? null : reader.GetString(7),
            CreatedAt = reader.GetFieldValue<DateTimeOffset>(8),
            UpdatedAt = reader.GetFieldValue<DateTimeOffset>(9)
        };
    }
}
