using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {

        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string errorMessage;

        // View se na tuhle událost pověsí a okno zavře
        public event Action<bool?> RequestClose;

        public LoginViewModel()
        {
           
        }

        [RelayCommand]
        private void Login()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vyplňte uživatelské jméno i heslo.";
                return;
            }

            //Pro otestování zakomentovano -- stačí něco vyplněné pro přihlášení
            //if (!UserManager.LogIn(UserName,Password))
            //{
            //    ErrorMessage = "Neplatné uživatelské jméno nebo heslo.";
            //    return;
            //}

            
            RequestClose?.Invoke(true); 
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose?.Invoke(false);  
        }
    }
}
