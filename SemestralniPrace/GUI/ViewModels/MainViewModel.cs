using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using GUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GUI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject currentViewModel;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsLogged))]
        [NotifyPropertyChangedFor(nameof(IsAdmin))]
        private User currentUser = null;


        public bool IsLogged => CurrentUser != null;

        public bool IsAdmin => CurrentUser != null && CurrentUser.Role.Name == "Admin";

        [ObservableProperty]
        private string logButtonText;

        public MainViewModel()
        {
            currentViewModel = new HomeViewModel();
            //isLogged = false;
            //isAdmin = false;
            logButtonText = "Log in";
        }

        [RelayCommand]
        public void UpdateLog()
        {
            if (IsLogged)
            {
                CurrentViewModel = new HomeViewModel();
                UserManager.LogOut();
                CurrentUser = UserManager.CurrentUser;
                LogButtonText = "Log in";
                //IsLogged = false;
                //IsAdmin = false;
            }
            else
            {
                var vm = new LoginViewModel();
                var dialog = new LoginWindow(vm);
                bool log = (bool)dialog.ShowDialog();
                if (log)
                {
                    LogButtonText = "Log out";
                    CurrentUser = UserManager.CurrentUser;
                }


                //bool log = (bool)
                //if (log)
                //{
                //    //MessageBox.Show($"User: {UserManager.CurrentUser.Role.Name}");
                //    if(UserManager.CurrentUser.Role.Name == "Admin")
                //    {
                //        IsAdmin = true;
                //    }
                //    else
                //    {
                //        IsAdmin = false;
                //    }

                //        LogButtonText = "Log out";
                //    IsLogged = true;
                //}

            }


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
                case "SystemCatalog":
                    CurrentViewModel = new SystemCatalogViewModel();
                    break;
                case "Profil":
                    CurrentViewModel = new UserProfileViewModel();
                    break;
            }
        }
    }
}
