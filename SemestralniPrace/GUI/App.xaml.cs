using DatabaseAccess;
using GUI.Helpers;
using System;
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
            // Zachycení UI thread výjimek
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Zachycení výjimek z jiných threadů
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Zachycení async výjimek
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Zobrazit chybu v dialogu
            ErrorHandler.ShowError(e.Exception, "Došlo k neočekávané chybě v aplikaci");

            // Označit jako zpracovanou, aby aplikace nespadla
            e.Handled = true;

            // Uzavřít connection
            try
            {
                ConnectionManager.CloseConnection();
            }
            catch
            {

            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                ErrorHandler.ShowError(exception, "Došlo ke kritické chybě v aplikaci");
            }

            try
            {
                ConnectionManager.CloseConnection();
            }
            catch
            {
                // Ignorovat chyby při zavírání spojení
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            ErrorHandler.ShowError(e.Exception, "Došlo k chybě v asynchronní operaci");

            // Označit jako zpracovanou
            e.SetObserved();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                ConnectionManager.CloseConnection();
            }
            catch
            {
                // Ignorovat chyby při zavírání spojení
            }

            base.OnExit(e);
        }
    }
}