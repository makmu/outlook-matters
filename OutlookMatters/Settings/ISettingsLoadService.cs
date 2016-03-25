using System;

namespace OutlookMatters.Settings
{
    public interface ISettingsLoadService
    {
        DateTime LastChanged { get; }
        Settings Load();
    }
}