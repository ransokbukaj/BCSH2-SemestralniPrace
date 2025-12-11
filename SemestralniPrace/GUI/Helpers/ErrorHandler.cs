using GUI.ViewModels;
using GUI.Views;
using System;
using System.Windows;

namespace GUI.Helpers
{
    public static class ErrorHandler
    {
        /// <summary>
        /// Zobrazí chybovou hlášku v dialogovém okně
        /// </summary>
        public static void ShowError(Exception exception, string customMessage = null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    var message = customMessage ?? exception.Message;
                    var viewModel = new ErrorDialogViewModel(exception)
                    {
                        ErrorMessage = message
                    };

                    var dialog = new ErrorDialog(viewModel)
                    {
                        Owner = Application.Current.MainWindow
                    };

                    dialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    // Fallback pokud selže i ErrorDialog
                    MessageBox.Show(
                        $"Došlo k chybě: {exception.Message}\n\nDalší chyba při zobrazení dialogu: {ex.Message}",
                        "Kritická chyba",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        /// Zobrazí chybovou hlášku s vlastním titulem a zprávou
        /// </summary>
        public static void ShowError(string title, string message, string details = null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    var viewModel = new ErrorDialogViewModel(title, message, details);
                    var dialog = new ErrorDialog(viewModel)
                    {
                        Owner = Application.Current.MainWindow
                    };

                    dialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    // Fallback pokud selže i ErrorDialog
                    MessageBox.Show(
                        $"{message}\n\nDalší chyba při zobrazení dialogu: {ex.Message}",
                        title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            });
        }

        /// <summary>
        /// Bezpečně vykoná akci s ošetřením chyb
        /// </summary>
        public static void SafeExecute(Action action, string errorMessage = "Operace selhala")
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                ShowError(ex, errorMessage);
            }
        }

        /// <summary>
        /// Bezpečně vykoná akci s ošetřením chyb a návratovou hodnotou
        /// </summary>
        public static T SafeExecute<T>(Func<T> func, string errorMessage = "Operace selhala", T defaultValue = default)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                ShowError(ex, errorMessage);
                return defaultValue;
            }
        }
    }
}