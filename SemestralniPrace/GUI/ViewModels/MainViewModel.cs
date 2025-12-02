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
                case "Painting":
                    CurrentViewModel = new PaintingViewModel();
                    break;
                case "Sculpture":
                    CurrentViewModel = new SculptureViewModel();
                    break;
                case "Visit":
                    CurrentViewModel = new VisitViewModel();
                    break;
                case "Sale":
                    CurrentViewModel = new SaleViewModel();
                    break;
                case "Buyer":
                    CurrentViewModel = new BuyerViewModel();
                    break;
                case "Address":
                    CurrentViewModel = new AddressViewModel();
                    break;
                case "Post":
                    CurrentViewModel = new PostViewModel();
                    break;
                case "User":
                    CurrentViewModel = new UserViewModel();
                    break;
                case "HistoryLog":
                    CurrentViewModel = new HistoryLogViewModel();
                    break;
            }
        }

    }
}
