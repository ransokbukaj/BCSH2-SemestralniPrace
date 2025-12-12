using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using Entities.Home;
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
        private readonly UserRepository userRepositiry = new UserRepository();
        private readonly CounterRepository counterRepository = new CounterRepository();
        private readonly UtilityRepository utilyRepository = new UtilityRepository();

        //List pro načtení všech uživatelů
        private List<User> _allUsers = new();

        //List pro zobrazení uživatelů
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

        [ObservableProperty]
        private UserStatistics userStat;

        [ObservableProperty]
        private string searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        public UserViewModel()
        {
            Load();
        }

        partial void OnSelectedUserChanged(User value)
        {
            ErrorLog = string.Empty;
            NewPassword = string.Empty;
            NewPasswordConfirm = string.Empty;
            if(SelectedUser != null)
            {
                UserStat = utilyRepository.GetUserStatistics(SelectedUser.Id);
            }
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                _allUsers = userRepositiry.GetList();
                
                Roles = new ObservableCollection<Counter>(counterRepository.GetRoles());
                ApplyFilter();
            }, "Načtení uživatelů selhalo");
        }

        /// <summary>
        /// Metoda pro filtrování uživatelů podle jejich uživatelského jména, křestního jména, příjmeni a emailu
        /// </summary>
        private void ApplyFilter()
        {
            if (_allUsers == null)
                return;

            var text = (SearchText ?? "").Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                Users = new ObservableCollection<User>(_allUsers);
                return;
            }

            var lower = text.ToLowerInvariant();

            var filtered = _allUsers
                .Where(u =>
                    (!string.IsNullOrWhiteSpace(u.Username) && u.Username.ToLowerInvariant().Contains(lower)) ||
                    (!string.IsNullOrWhiteSpace(u.FirstName) && u.FirstName.ToLowerInvariant().Contains(lower)) ||
                    (!string.IsNullOrWhiteSpace(u.LastName) && u.LastName.ToLowerInvariant().Contains(lower)) ||
                    (!string.IsNullOrWhiteSpace(u.Email) && u.Email.ToLowerInvariant().Contains(lower)))
                .ToList();

            Users = new ObservableCollection<User>(filtered);
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
                userRepositiry.SaveItem(SelectedUser);
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
                userRepositiry.DeleteItem(SelectedUser.Id);
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

                userRepositiry.ChangePassword(SelectedUser.Id, NewPassword);
                ErrorLog = "Heslo úspěšně změněno.";

                NewPassword = string.Empty;
                NewPasswordConfirm = string.Empty;
            }, "Změna hesla selhala");
        }
    }
}