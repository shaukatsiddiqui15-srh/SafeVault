namespace SafeVault;

public interface IUserAccountStore
{
    UserAccount? FindByUsername(string username);
}
