namespace OutlookMatters.Core.Security
{
    public interface IPasswordProvider
    {
        string GetPassword(string username);
    }
}