using NUnit.Framework;

namespace SafeVault.Tests;

[TestFixture]
public class TestAuthenticationAuthorization
{
    private readonly PasswordHasher _passwordHasher = new();

    [Test]
    public void TestValidAdminLoginReturnsAuthenticatedUser()
    {
        var admin = CreateAccount("adminuser", "P@ssw0rd!", UserRole.Admin);
        var store = new InMemoryUserAccountStore([admin]);
        var authService = new AuthenticationService(store, _passwordHasher);

        var result = authService.Authenticate(" adminuser ", "P@ssw0rd!");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Username, Is.EqualTo("adminuser"));
        Assert.That(result.Role, Is.EqualTo(UserRole.Admin));
    }

    [Test]
    public void TestInvalidLoginAttemptIsRejected()
    {
        var user = CreateAccount("alice", "CorrectHorseBatteryStaple1!", UserRole.User);
        var store = new InMemoryUserAccountStore([user]);
        var authService = new AuthenticationService(store, _passwordHasher);

        var result = authService.Authenticate("alice", "wrong-password");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void TestUnauthorizedAccessIsDeniedForStandardUser()
    {
        var authorizationService = new AuthorizationService();
        var user = new AuthenticatedUser("alice", UserRole.User);

        Assert.That(authorizationService.CanAccessAdminDashboard(user), Is.False);
        Assert.That(() => authorizationService.DemandAdminAccess(user), Throws.TypeOf<UnauthorizedAccessException>());
    }

    [Test]
    public void TestAdminAccessIsAllowedForAdminUser()
    {
        var authorizationService = new AuthorizationService();
        var admin = new AuthenticatedUser("adminuser", UserRole.Admin);

        Assert.That(authorizationService.CanAccessAdminDashboard(admin), Is.True);
        Assert.That(() => authorizationService.DemandAdminAccess(admin), Throws.Nothing);
    }

    [Test]
    public void TestPasswordHasherProducesVerifiableHash()
    {
        var hash = _passwordHasher.HashPassword("CorrectHorseBatteryStaple1!");

        Assert.That(hash, Does.Contain("."));
        Assert.That(_passwordHasher.VerifyPassword("CorrectHorseBatteryStaple1!", hash), Is.True);
        Assert.That(_passwordHasher.VerifyPassword("wrong-password", hash), Is.False);
    }

    private UserAccount CreateAccount(string username, string password, UserRole role)
    {
        return new UserAccount(username, _passwordHasher.HashPassword(password), role);
    }
}
