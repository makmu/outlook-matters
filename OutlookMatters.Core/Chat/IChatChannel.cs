using OutlookMatters.Core.Settings;

namespace OutlookMatters.Core.Chat
{
    public interface IChatChannel
    {
        void CreatePost(string message);
        ChannelSetting ToSetting();
    }
}