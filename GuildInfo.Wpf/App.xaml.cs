using System.Windows;
using System.Windows.Threading;

namespace GuildInfo.Wpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Current.DispatcherUnhandledException += AppDispatcherUnhandledException;
            base.OnStartup(e);
        }

        private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("OMG CRITICAL ERROR!!!:\r\n\r\n" + e.Exception.Message + "\r\n\r\n" + "Stack trace:\r\n" + e.Exception.StackTrace, "OMG Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            Current.Shutdown();
        }
    }
}
