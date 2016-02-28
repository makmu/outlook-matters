namespace OutlookMatters.Settings
{
    public interface ISettingsProvider
    {
        string ChannelId { get; set; }
        string Url { get; set; }
        string TeamId { get; set; }
        string Username { get; set; }
        string Password { get; set; }
    }
}
