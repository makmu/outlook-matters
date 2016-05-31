namespace OutlookMatters.Core.Settings
{
    public interface ISettingsSaveService
    {
        void SaveCredentials(string mattermostUrl, string teamId, string username, MattermostVersion version);
        void SaveChannels(string channelsMap);
    }
}