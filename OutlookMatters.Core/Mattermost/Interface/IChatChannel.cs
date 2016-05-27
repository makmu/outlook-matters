using OutlookMatters.Core.Settings;

namespace OutlookMatters.Core.Mattermost.Interface
{
    public interface IChatChannel
    {
        void CreatePost(string message);
        ChannelSetting ToSetting();
    }
}