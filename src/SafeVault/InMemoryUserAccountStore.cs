namespace SafeVault;

public sealed class InMemoryUserAccountStore : IUserAccountStore
{
    private readonly Dictionary<string, UserAccount> _accounts;

    public InMemoryUserAccountStore(IEnumerable<UserAccount> accounts)
    {
        ArgumentNullException.ThrowIfNull(accounts);

        _accounts = accounts.ToDictionary(account => account.Username, StringComparer.OrdinalIgnoreCase);
    }

    public UserAccount? FindByUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }

        return _accounts.TryGetValue(username, out var account) ? account : null;
    }
}
