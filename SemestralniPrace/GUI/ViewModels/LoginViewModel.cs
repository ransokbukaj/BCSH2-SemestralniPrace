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
        private readonly UserRepository _userRepository;

        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string errorMessage;

        public User LoggedUser { get; private set; }

        // View se na tuhle událost pověsí a okno zavře
        public event Action<bool?> RequestClose;

        public LoginViewModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
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

            // Tohle si upravíš podle svého UserRepository
            //var user = _userRepository.ValidateUser(UserName, Password);

            //if (user == null)
            //{
            //    ErrorMessage = "Neplatné uživatelské jméno nebo heslo.";
            //    return;
            //}

            //LoggedUser = user;
            RequestClose?.Invoke(true);   // signál: přihlášení OK → zavřít okno (DialogResult = true)
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose?.Invoke(false);  // signál: ruším → zavřít okno (DialogResult = false)
        }
    }
}
