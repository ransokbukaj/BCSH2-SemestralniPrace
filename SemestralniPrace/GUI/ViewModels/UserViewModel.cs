using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities.Account;

using DatabaseAccess;

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

        [ObservableProperty]
        private ObservableCollection<User> users = new();

        [ObservableProperty]
        private User selectedUser;

        public UserViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            Users = new ObservableCollection<User>(repository.GetList());
        }

        [RelayCommand]
        private void New()
        {
            SelectedUser = new User();
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedUser == null)
                return;

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

    public class UserViewModel : ObservableObject
    {
        private UserRepository userRepository;
        private CounterRepository counterRepository;

    }
}
