using System;
using System.Windows;

namespace OutlookMatters.Error
{
    public class MessageBoxErrorDisplay: IErrorDisplay
    {
        public void Display(Exception exception)
        {
            MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
