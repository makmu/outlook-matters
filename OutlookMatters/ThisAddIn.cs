using System;
using Office = Microsoft.Office.Core;

namespace OutlookMatters
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
        }

        protected override Office.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            var httpClient = new DotNetHttpClient();
            return new MailItemContextMenuEntry(
                new OutlookMailExplorer(),
                new RestMattermost(new UserSessionFactory(httpClient), httpClient),
                new ApplicationSettingsProvider());
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += ThisAddIn_Startup;
            this.Shutdown += ThisAddIn_Shutdown;
        }
        
        #endregion
    }
}
