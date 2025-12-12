using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using GUI.Helpers;
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

        // Údálost pro zavření dialogu
        public event Action<bool?> RequestClose;

        public LoginViewModel() { }

        [RelayCommand]
        private void Login()
        {
            LoginError = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                LoginError = "Vyplňte uživatelské jméno i heslo.";
                return;
            }

            ErrorHandler.SafeExecute(() =>
            {
                if (!UserManager.LogIn(Username, Password))
                {
                    LoginError = "Neplatné uživatelské jméno nebo heslo.";
                    return;
                }

                RequestClose?.Invoke(true);
            }, "Přihlášení selhalo. Zkontrolujte připojení k databázi.");
        }

        [RelayCommand]
        private void Register()
        {
            RegisterError = "";

            // Validace uživatelského jména a hesla
            if (string.IsNullOrWhiteSpace(RegUsername))
            {
                RegisterError = "Vyplňte uživatelské jméno.";
                return;
            }

            if (string.IsNullOrWhiteSpace(RegPassword))
            {
                RegisterError = "Vyplňte heslo.";
                return;
            }

            // Kontrola shody hesel
            if (RegPassword != RegPasswordConfirm)
            {
                RegisterError = "Hesla se neshodují.";
                return;
            }

            // Validace emailu (pokud je vyplněn)
            if (!string.IsNullOrWhiteSpace(RegEmail) && !RegEmail.Contains("@"))
            {
                RegisterError = "Email není ve správném formátu.";
                return;
            }

            ErrorHandler.SafeExecute(() =>
            {
                if (!UserManager.Register(RegUsername, RegPassword, RegFirstName, RegLastName, RegEmail, RegPhone))
                {
                    RegisterError = "Registrace se nezdařila. Uživatelské jméno může být již použito.";
                    return;
                }

                if (!UserManager.LogIn(RegUsername, RegPassword))
                {
                    RegisterError = "Nastala chyba při přihlašování nového účtu.";
                    return;
                }

                RequestClose?.Invoke(true);
            }, "Registrace selhala.");
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose?.Invoke(false);  
        }
    }
}
