using Npgsql;
using SugboGo.Models;

namespace SugboGo.Services.Auth;

public sealed class PostgresUserAccountStore : IUserAccountStore
{
    private static readonly SemaphoreSlim InitializationLock = new(1, 1);
    private static bool _initialized;
    private readonly string _connectionString;

    public PostgresUserAccountStore(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");
    }

    public async Task<UserAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);

        const string sql = """
            select id, email, full_name, password_hash, role, created_at
            from sogbogo_users
            where email = @email
            limit 1;
            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("email", NormalizeEmail(email));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new UserAccount
        {
            Id = reader.GetString(0),
            Email = reader.GetString(1),
            FullName = reader.GetString(2),
            PasswordHash = reader.GetString(3),
            Role = AccountRoles.Normalize(reader.GetString(4)),
            CreatedAt = reader.GetFieldValue<DateTimeOffset>(5)
        };
    }

    public async Task<UserAccount> CreateAsync(UserAccount account, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken);

        account.Email = NormalizeEmail(account.Email);
        account.CreatedAt = DateTimeOffset.UtcNow;

        const string sql = """
            insert into sogbogo_users (id, email, full_name, password_hash, role, created_at)
            values (@id, @email, @fullName, @passwordHash, @role, @createdAt);
            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", account.Id);
        command.Parameters.AddWithValue("email", account.Email);
        command.Parameters.AddWithValue("fullName", account.FullName);
        command.Parameters.AddWithValue("passwordHash", account.PasswordHash);
        command.Parameters.AddWithValue("role", AccountRoles.Normalize(account.Role));
        command.Parameters.AddWithValue("createdAt", account.CreatedAt);

        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new InvalidOperationException("An account already exists for this email address.", ex);
        }

        return account;
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
                create table if not exists sogbogo_users (
                    id text primary key,
                    email text not null unique,
                    full_name text not null,
                    password_hash text not null,
                    role text not null default 'Client',
                    created_at timestamptz not null default now()
                );

                alter table sogbogo_users
                add column if not exists role text not null default 'Client';

                update sogbogo_users
                set role = 'Client'
                where role is null or trim(role) = '';

                create index if not exists ix_sogbogo_users_email on sogbogo_users (email);
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

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
