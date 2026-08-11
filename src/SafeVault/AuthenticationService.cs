namespace SafeVault;

public sealed class AuthenticationService
{
    private readonly IUserAccountStore _accountStore;
    private readonly PasswordHasher _passwordHasher;

    public AuthenticationService(IUserAccountStore accountStore, PasswordHasher? passwordHasher = null)
    {
        _accountStore = accountStore ?? throw new ArgumentNullException(nameof(accountStore));
        _passwordHasher = passwordHasher ?? new PasswordHasher();
    }

    public AuthenticatedUser? Authenticate(string username, string password)
    {
        var normalizedUsername = InputSanitizer.SanitizeUsername(username);
        var account = _accountStore.FindByUsername(normalizedUsername);

        if (account is null)
        {
            return null;
        }

        if (!_passwordHasher.VerifyPassword(password, account.PasswordHash))
        {
            return null;
        }

        return new AuthenticatedUser(account.Username, account.Role);
    }
}
