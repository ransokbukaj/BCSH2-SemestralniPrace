using DatabaseAccess;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Uzavřít connection
            ConnectionManager.CloseConnection();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            ConnectionManager.CloseConnection();
            base.OnExit(e);

        }
    }

}
