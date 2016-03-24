using System;

namespace OutlookMatters.Settings
{
    public interface ISettingsLoadService
    {
        Settings Load();
        DateTime LastChanged { get; }
    }
}
