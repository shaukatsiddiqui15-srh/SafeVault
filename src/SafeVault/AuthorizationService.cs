namespace SafeVault;

public sealed class AuthorizationService
{
    public bool CanAccessAdminDashboard(AuthenticatedUser? user)
    {
        return user is not null && user.Role == UserRole.Admin;
    }

    public void DemandAdminAccess(AuthenticatedUser? user)
    {
        if (!CanAccessAdminDashboard(user))
        {
            throw new UnauthorizedAccessException("Admin access is required.");
        }
    }
}
