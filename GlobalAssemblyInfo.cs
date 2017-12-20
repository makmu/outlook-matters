using System.Reflection;

[assembly: AssemblyCompany("outlook-matters project")]
[assembly: AssemblyProduct("OutlookMatters Addin")]
[assembly: AssemblyDescription("An Outlook-Mattermost Bridge")]
[assembly: AssemblyCopyright("Copyright © 2016 by the outlook-matters developers")]
[assembly: AssemblyTrademark("This application has no Trademark.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyVersion(Version.Current)]
[assembly: AssemblyFileVersion(Version.Current)]
[assembly: AssemblyInformationalVersion(Version.FullCurrent)]

internal class Version
{
    // refer to http://semver.org for more information
    public const string Major = "1";
    public const string Minor = "3";
    public const string Patch = "1";

    public const string AdditionalReleaseInformation = "";
    public const string Current = Major + "." + Minor + "." + Patch;
    public const string FullCurrent = Current + "-release" + AdditionalReleaseInformation;
}
