using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject currentViewModel;

        [ObservableProperty]
        private bool isLogged;

        [ObservableProperty]
        private string logButtonText;

        public MainViewModel()
        {
            currentViewModel = new HomeViewModel();
            isLogged = false;
            logButtonText = "Log in";
        }

        [RelayCommand]
        public void UpdateLog()
        {
            if (IsLogged)
            {
                CurrentViewModel = new HomeViewModel();
                LogButtonText = "Log in";
            }
            else
            {
                LogButtonText = "Log out";
            }

                IsLogged = !IsLogged;
        }

        [RelayCommand]
        public void UpdateView(object parameter)
        {
            switch (parameter.ToString())
            {
                case "Home":
                    CurrentViewModel = new HomeViewModel();
                    break;
                case "P1":
                    CurrentViewModel = new P1ViewModel();
                    break;
                case "P2":
                    CurrentViewModel = new P2ViewModel();
                    break;
                case "P3":
                    CurrentViewModel = new P3ViewModel();
                    break;
                case "P4":
                    CurrentViewModel = new P4ViewModel();
                    break;
                case "P5":
                    CurrentViewModel = new P5ViewModel();
                    break;
                case "P6":
                    CurrentViewModel = new P6ViewModel();
                    break;
                // případně další stránky…
                default:
                    break;
            }
        }

    }
}
