using Microsoft.AspNetCore.Identity;

namespace BlazorAppTest2.Tests.Fakes;

/// <summary>
/// Minimal in-memory IUserStore so UserManager can be constructed in tests
/// without a real database. Only the methods actually called by the pages
/// under test are implemented; everything else throws NotImplementedException.
/// </summary>
public class FakeUserStore : IUserStore<FakeApplicationUser>,
                              IUserEmailStore<FakeApplicationUser>,
                              IUserPasswordStore<FakeApplicationUser>
{
    private readonly List<FakeApplicationUser> _users = new();

    // ── IUserStore ──────────────────────────────────────────────────────────

    public Task<IdentityResult> CreateAsync(FakeApplicationUser user, CancellationToken ct)
    {
        user.Id = Guid.NewGuid().ToString();
        _users.Add(user);
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(FakeApplicationUser user, CancellationToken ct)
    {
        _users.Remove(user);
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<FakeApplicationUser?> FindByIdAsync(string userId, CancellationToken ct)
        => Task.FromResult(_users.FirstOrDefault(u => u.Id == userId));

    public Task<FakeApplicationUser?> FindByNameAsync(string normalizedName, CancellationToken ct)
        => Task.FromResult(_users.FirstOrDefault(u =>
            string.Equals(u.NormalizedUserName, normalizedName, StringComparison.OrdinalIgnoreCase)));

    public Task<string?> GetNormalizedUserNameAsync(FakeApplicationUser user, CancellationToken ct)
        => Task.FromResult(user.NormalizedUserName);

    public Task<string> GetUserIdAsync(FakeApplicationUser user, CancellationToken ct)
        => Task.FromResult(user.Id ?? string.Empty);

    public Task<string?> GetUserNameAsync(FakeApplicationUser user, CancellationToken ct)
        => Task.FromResult(user.UserName);

    public Task SetNormalizedUserNameAsync(FakeApplicationUser user, string? name, CancellationToken ct)
    {
        user.NormalizedUserName = name;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(FakeApplicationUser user, string? name, CancellationToken ct)
    {
        user.UserName = name;
        return Task.CompletedTask;
    }

    public Task<IdentityResult> UpdateAsync(FakeApplicationUser user, CancellationToken ct)
        => Task.FromResult(IdentityResult.Success);

    // ── IUserEmailStore ──────────────────────────────────────────────────────

    public Task<FakeApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken ct)
        => Task.FromResult(_users.FirstOrDefault(u =>
            string.Equals(u.NormalizedEmail, normalizedEmail, StringComparison.OrdinalIgnoreCase)));

    public Task<string?> GetEmailAsync(FakeApplicationUser user, CancellationToken ct)
        => Task.FromResult(user.Email);

    public Task<bool> GetEmailConfirmedAsync(FakeApplicationUser user, CancellationToken ct)
        => Task.FromResult(user.EmailConfirmed);

    public Task<string?> GetNormalizedEmailAsync(FakeApplicationUser user, CancellationToken ct)
        => Task.FromResult(user.NormalizedEmail);

    public Task SetEmailAsync(FakeApplicationUser user, string? email, CancellationToken ct)
    {
        user.Email = email;
        return Task.CompletedTask;
    }

    public Task SetEmailConfirmedAsync(FakeApplicationUser user, bool confirmed, CancellationToken ct)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public Task SetNormalizedEmailAsync(FakeApplicationUser user, string? email, CancellationToken ct)
    {
        user.NormalizedEmail = email;
        return Task.CompletedTask;
    }

    // ── IUserPasswordStore ───────────────────────────────────────────────────

    public Task<string?> GetPasswordHashAsync(FakeApplicationUser user, CancellationToken ct)
        => Task.FromResult(user.PasswordHash);

    public Task<bool> HasPasswordAsync(FakeApplicationUser user, CancellationToken ct)
        => Task.FromResult(user.PasswordHash is not null);

    public Task SetPasswordHashAsync(FakeApplicationUser user, string? passwordHash, CancellationToken ct)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    // ── IDisposable ──────────────────────────────────────────────────────────
    public void Dispose() { }
}
