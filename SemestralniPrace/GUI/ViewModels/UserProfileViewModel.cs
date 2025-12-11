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

<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
        [RelayCommand]
        private void ChangePassword()
        {
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

                repo.ChangePassword(CurrentUser.Id, NewPassword);
                ErrorLog = "Heslo úspěšně změněno.";

                NewPassword = string.Empty;
                NewPasswordConfirm = string.Empty;
            }, "Změna hesla selhala");
        }

        [RelayCommand]
        private void SaveProfile()
        {
            if (CurrentUser == null)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                if (string.IsNullOrWhiteSpace(CurrentUser.Username))
                {
                    ErrorHandler.ShowError("Validační chyba", "Uživatelské jméno nesmí být prázdné");
                    return;
                }

                if (string.IsNullOrWhiteSpace(CurrentUser.FirstName))
                {
                    ErrorHandler.ShowError("Validační chyba", "Jméno nesmí být prázdné");
                    return;
                }

                if (string.IsNullOrWhiteSpace(CurrentUser.LastName))
                {
                    ErrorHandler.ShowError("Validační chyba", "Příjmení nesmí být prázdné");
                    return;
                }

                // Email validace (pokud je vyplněný)
                if (!string.IsNullOrWhiteSpace(CurrentUser.Email) && !CurrentUser.Email.Contains("@"))
                {
                    ErrorHandler.ShowError("Validační chyba", "Email není ve správném formátu");
                    return;
                }

                repo.SaveItem(CurrentUser);
                ErrorLog = "Profil úspěšně uložen.";
            }, "Uložení profilu selhalo. Uživatelské jméno může být již obsazené.");
        }
    }
}
