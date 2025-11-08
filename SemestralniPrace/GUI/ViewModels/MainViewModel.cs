using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject currentViewModel;


        public MainViewModel()
        {
            currentViewModel = new HomeViewModel();
        }


        [RelayCommand]
        public void UpdateView(object parameter)
        {
            switch (parameter.ToString())
            {
                case "Home":
                    CurrentViewModel = new HomeViewModel();
                    break;
                case "P1":
                    CurrentViewModel = new P1ViewModel();
                    break;
                case "P2":
                    CurrentViewModel = new P2ViewModel();
                    break;
                case "P3":
                    CurrentViewModel = new P3ViewModel();
                    break;
                // případně další stránky…
                default:
                    break;
            }
        }

    }
}
