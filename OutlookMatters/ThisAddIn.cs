using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using OutlookMatters.ContextMenu;
using OutlookMatters.Error;
using OutlookMatters.Http;
using OutlookMatters.Mail;
using OutlookMatters.Mattermost;
using OutlookMatters.Mattermost.Session;
using OutlookMatters.Security;
using OutlookMatters.Settings;
using Office = Microsoft.Office.Core;

namespace OutlookMatters
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            CheckVersions();
        }

        private void CheckVersions()
        {
            if (!Debugger.IsAttached)
            {
                var assemblyVersion = Assembly.GetAssembly(GetType()).GetName().Version;
                var applicationVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;

                if (assemblyVersion.Major == applicationVersion.Major &&
                    assemblyVersion.Minor == applicationVersion.Minor &&
                    assemblyVersion.Build == applicationVersion.Build) return;

                MessageBox.Show(
                    "Addin configuration error: application version (" + applicationVersion + ") and assembly version (" +
                    assemblyVersion + ") do not match!",
                    "OutlookMatters", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
        }

        protected override Office.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            var httpClient = new DotNetHttpClient();
            var mattermost = new RestMattermost(new UserSessionFactory(httpClient), httpClient);
            var passwordDialog = new PasswordDialogShell();
            var caches = new CompositeCache();
            var settingsService = new ApplicationSettingsService(caches);
            var sessionCache = new TransientSession(mattermost, settingsService, passwordDialog);
            caches.Add(sessionCache);
            var explorerService = new OutlookExplorerService();
            var mailExplorer = new OutlookMailExplorer(explorerService);
            var errorDisplay = new MessageBoxErrorDisplay();
            var settingsUi = new WpfSettingsUserInterface(settingsService, settingsService);
            var permalinkUi = new PermalinkDialogShell();
            var postIdFilter = new PostIdFromPermalinkFilter(permalinkUi);
            var rootPostIdResolver = new RootPostIdResolver(postIdFilter, sessionCache);

            return new MailItemContextMenuEntry(
                mailExplorer,
                settingsService,
                errorDisplay,
                settingsUi,
                sessionCache,
                rootPostIdResolver);
        }

        #region VSTO generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            Startup += ThisAddIn_Startup;
            Shutdown += ThisAddIn_Shutdown;
        }

        #endregion
    }
}