using CommunityToolkit.Mvvm.ComponentModel;
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
        [ObservableProperty]
        private User currentUser;

        public UserProfileViewModel()
        {
            currentUser = UserManager.CurrentUser;
        }
    }
}
