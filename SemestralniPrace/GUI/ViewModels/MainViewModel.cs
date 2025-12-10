using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using GUI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private readonly UserRepository userRep = new UserRepository();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsLogged))]
        [NotifyPropertyChangedFor(nameof(IsAdmin))]
        [NotifyPropertyChangedFor(nameof(CanSimulate))]
        private User currentUser = null;

        [ObservableProperty]
        private User selectedUser;

        public bool IsLogged => CurrentUser != null;
        public bool IsAdmin => CurrentUser != null && CurrentUser.Role.Name == "Admin";
        public bool CanSimulate => IsAdmin && UserManager.isSimulating == false;

        [ObservableProperty]
        private string logButtonText;

        [ObservableProperty]
        private ObservableCollection<User> users;

        public MainViewModel()
        {
            currentViewModel = new HomeViewModel();
            logButtonText = "Log in";
        }

        partial void OnCurrentUserChanged(User value)
        {
            if (CurrentUser != null && IsAdmin)
            {
                List<User> list = userRep.GetList().Where(u => u.Id != CurrentUser.Id).ToList();
                Users = new ObservableCollection<User>(list);
            }
        }

        [RelayCommand]
        public void UpdateLog()
        {
            if (IsLogged)
            {
                if (UserManager.isSimulating)
                {
                    UserManager.EndSimulatingUser();
                    CurrentUser = UserManager.CurrentUser;
                    LogButtonText = "Log out";
                }
                else
                {
                    UserManager.LogOut();
                    CurrentViewModel = new HomeViewModel();
                    CurrentUser = UserManager.CurrentUser;
                    LogButtonText = "Log in";
                }
                
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
            }


        }
        [RelayCommand]
        private void SimulateOtherUser()
        {
            if(SelectedUser != null)
            {
                UserManager.StartSimulateUser(SelectedUser);
                CurrentUser = UserManager.CurrentUser;
                CurrentViewModel = new HomeViewModel();
                LogButtonText = "End Simulation";
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
