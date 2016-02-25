using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace OutlookMatters
{
    [ComVisible(true)]
    public class MailItemContextMenuEntry : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;

        public MailItemContextMenuEntry()
        {
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonId)
        {
            switch (ribbonId)
            {
                case "Microsoft.Outlook.Explorer":
                    return GetResourceText("OutlookMatters.MailItemContextMenuEntry.xml");
                default:
                    return string.Empty;
            }
        }

        #endregion

        public void OnSettingsClick(Office.IRibbonControl control)
        {
            MessageBox.Show("Checkpoint");
        }

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
