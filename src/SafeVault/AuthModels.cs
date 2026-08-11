namespace SafeVault;

public enum UserRole
{
    User = 0,
    Admin = 1
}

public sealed record UserAccount(
    string Username,
    string PasswordHash,
    UserRole Role);

public sealed record AuthenticatedUser(
    string Username,
    UserRole Role);
