using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using GUI.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels
{
    public partial class UserViewModel : ObservableObject
    {
        private readonly UserRepository repository = new UserRepository();
        private readonly CounterRepository counterRep = new CounterRepository();

        [ObservableProperty]
        private ObservableCollection<User> users = new();

        [ObservableProperty]
        private ObservableCollection<Counter> roles = new();

        [ObservableProperty]
        private User selectedUser;

        [ObservableProperty]
        private string newPassword;

        [ObservableProperty]
        private string newPasswordConfirm;

        [ObservableProperty]
        private string errorLog;

        public UserViewModel()
        {
            Load();
        }

        partial void OnSelectedUserChanged(User value)
        {
            ErrorLog = string.Empty;
            NewPassword = string.Empty;
            NewPasswordConfirm = string.Empty;
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                Users = new ObservableCollection<User>(repository.GetList());
                Roles = new ObservableCollection<Counter>(counterRep.GetRoles());
            }, "Načtení uživatelů selhalo");
        }

        [RelayCommand]
        private void New()
        {
            SelectedUser = new User()
            {
                Role = new Counter()
                {
                    Id = 2
                }
            };
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedUser == null)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                if (string.IsNullOrWhiteSpace(SelectedUser.Username))
                {
                    ErrorHandler.ShowError("Validační chyba", "Uživatelské jméno nesmí být prázdné");
                    return;
                }

                if (string.IsNullOrWhiteSpace(SelectedUser.FirstName))
                {
                    ErrorHandler.ShowError("Validační chyba", "Jméno nesmí být prázdné");
                    return;
                }

                if (string.IsNullOrWhiteSpace(SelectedUser.LastName))
                {
                    ErrorHandler.ShowError("Validační chyba", "Příjmení nesmí být prázdné");
                    return;
                }

                if (SelectedUser.Role == null || SelectedUser.Role.Id == 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Musíte vybrat roli");
                    return;
                }

                if (Roles != null && SelectedUser != null)
                {
                    SelectedUser.Role = Roles.FirstOrDefault(p => p.Id == SelectedUser.Role.Id);
                }

                SelectedUser.Password = NewPassword;
                repository.SaveItem(SelectedUser);
                Load();
            }, "Uložení uživatele selhalo.");
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedUser == null || SelectedUser.Id == 0)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                repository.DeleteItem(SelectedUser.Id);
                Load();
            }, "Smazání uživatele selhalo");
        }

        [RelayCommand]
        private void ChangePassword()
        {
            if (SelectedUser == null || SelectedUser.Id == 0)
            {
                ErrorLog = "Vyberte uživatele";
                return;
            }

            ErrorHandler.SafeExecute(() =>
            {
                ErrorLog = string.Empty;

                if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(NewPasswordConfirm))
                {
                    ErrorLog = "Nové heslo a potvrzení musí být vyplněné.";
                    return;
                }

                if (NewPassword != NewPasswordConfirm)
                {
                    ErrorLog = "Hesla se neshodují.";
                    return;
                }

                repository.ChangePassword(SelectedUser.Id, NewPassword);
                ErrorLog = "Heslo úspěšně změněno.";

                NewPassword = string.Empty;
                NewPasswordConfirm = string.Empty;
            }, "Změna hesla selhala");
        }
    }
}