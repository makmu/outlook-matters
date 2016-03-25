namespace OutlookMatters.Security
{
    public interface IPasswordProvider
    {
        string GetPassword(string username);
    }
}