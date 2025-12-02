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
                case "Exhibition":
                    CurrentViewModel = new ExhibitionViewModel();
                    break;
                case "EducationProgram":
                    CurrentViewModel = new EducationProgramViewModel();
                    break;
                case "Artist":
                    CurrentViewModel = new ArtistViewModel();
                    break;
                case "P4":
                    CurrentViewModel = new PaintingViewModel();
                    break;
                case "P5":
                    CurrentViewModel = new SculptureViewModel();
                    break;
                case "P6":
                    CurrentViewModel = new VisitViewModel();
                    break;
                // případně další stránky…
                default:
                    break;
            }
        }

    }
}
