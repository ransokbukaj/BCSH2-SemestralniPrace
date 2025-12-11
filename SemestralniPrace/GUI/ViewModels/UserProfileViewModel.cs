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
    public partial class UserProfileViewModel : ObservableObject
    {
        private readonly UserRepository repo = new UserRepository();

        [ObservableProperty]
        private User currentUser;


        [ObservableProperty]
        private string newPassword;
        [ObservableProperty]
        private string newPasswordConfirm;

        [ObservableProperty]
        private string errorLog;

        public UserProfileViewModel()
        {
            currentUser = UserManager.CurrentUser;
        }


        [RelayCommand]
        private void ChangePassword()
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

            repo.ChangePassword(CurrentUser.Id,NewPassword);
            ErrorLog = "Heslo úspěšně změněno.";
        }
    }
}
