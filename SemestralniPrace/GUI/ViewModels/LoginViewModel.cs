using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GUI.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {

        // ---------- LOGIN ----------
        [ObservableProperty] private string username;
        [ObservableProperty] private string password;
        [ObservableProperty] private string loginError;

        // ---------- REGISTRACE ----------
        [ObservableProperty] private string regUsername;
        [ObservableProperty] private string regPassword;
        [ObservableProperty] private string regPasswordConfirm;
        [ObservableProperty] private string regFirstName;
        [ObservableProperty] private string regLastName;
        [ObservableProperty] private string regEmail;
        [ObservableProperty] private string regPhone;
        [ObservableProperty] private string registerError;

        // View se na tuhle událost pověsí a okno zavře
        public event Action<bool?> RequestClose;
        
     

        public LoginViewModel()
        {
           
        }


        [RelayCommand]
        private void Login()
        {
            LoginError = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                LoginError = "Vyplňte uživatelské jméno i heslo.";
                return;
            }

            //Pro otestování zakomentovano-- stačí něco vyplněné pro přihlášení
            if (!UserManager.LogIn(Username, Password))
            {
                LoginError = "Neplatné uživatelské jméno nebo heslo.";
                return;
            }


            RequestClose?.Invoke(true); 
        }

        // ---------- REGISTRACE COMMAND ----------
        [RelayCommand]
        private void Register()
        {
            RegisterError = "";
            //MessageBox.Show($"Heslo: {regPassword}, Znovu: {RegPasswordConfirm}");
            


            if (string.IsNullOrWhiteSpace(RegUsername) ||
                string.IsNullOrWhiteSpace(RegPassword))
            {
                RegisterError = "Vyplňte uživatelské jméno a heslo.";
                return;
            }

            if (RegPassword != RegPasswordConfirm)
            {
                RegisterError = "Hesla se neshodují.";
                return;
            }


            if (!UserManager.Register(RegUsername, RegPassword, RegFirstName, RegLastName, RegEmail, RegPhone))
            {
                RegisterError = "Neplatné uživatelské jméno nebo heslo.";
                return;
            }

            RequestClose?.Invoke(true);
        }


        [RelayCommand]
        private void Cancel()
        {
            RequestClose?.Invoke(false);  
        }
    }
}
