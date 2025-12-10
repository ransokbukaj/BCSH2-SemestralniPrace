using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
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
            Users = new ObservableCollection<User>(repository.GetList());
            Roles = new ObservableCollection<Counter>(counterRep.GetRoles());
        }

        [RelayCommand]
        private void New()
        {
            SelectedUser = new User()
            {
                Role = new Counter()
            };
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedUser == null)
                return;
            if(Roles != null && SelectedUser != null)
            {
                SelectedUser.Role = Roles.FirstOrDefault(p => p.Id == SelectedUser.Role.Id);
            }

            repository.SaveItem(SelectedUser);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedUser == null || SelectedUser.Id == 0)
                return;

            repository.DeleteItem(SelectedUser.Id);
            Load();
        }


        [RelayCommand]
        private void ChangePassword()
        {
            if(SelectedUser != null)
            {
                ErrorLog = string.Empty;
                if (string.IsNullOrEmpty(NewPassword) && string.IsNullOrEmpty(NewPasswordConfirm))
                {
                    ErrorLog = "Původní heslo, nové heslo a potvrzení musí být vyplněné.";
                    return;
                }

                if (NewPassword != NewPasswordConfirm)
                {
                    ErrorLog = "Hesla se neshodují.";
                    return;
                }

                repository.ChangePassword(SelectedUser.Id, NewPassword);
                ErrorLog = "Heslo úspěšně změněno.";

            }


        }
    }
}
