using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GUI.ViewModels
{
    public partial class ErrorDialogViewModel : ObservableObject
    {
        [ObservableProperty]
        private string errorTitle;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private string errorDetails;

        [ObservableProperty]
        private bool showDetails;

        public event Action RequestClose;

        public ErrorDialogViewModel(Exception exception)
        {
            ErrorTitle = "Došlo k chybě";
            ErrorMessage = exception.Message;
            ErrorDetails = exception.ToString();
            ShowDetails = false;
        }

        public ErrorDialogViewModel(string title, string message, string details = null)
        {
            ErrorTitle = title;
            ErrorMessage = message;
            ErrorDetails = details;
            ShowDetails = false;
        }

        [RelayCommand]
        private void ToggleDetails()
        {
            ShowDetails = !ShowDetails;
        }

        [RelayCommand]
        private void Close()
        {
            RequestClose?.Invoke();
        }
    }
}