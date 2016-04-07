namespace OutlookMatters.Settings
{
    public interface ISettingsSaveService
    {
        void SaveCredentials(string mattermostUrl, string teamId, string username);
        void SaveChannels(string channelsMap);
    }
}