using CommunityToolkit.Mvvm.ComponentModel;
using DatabaseAccess;
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
    public partial class HomeViewModel : ObservableObject
    {
        private readonly HomeViewRepository repo = new HomeViewRepository();

        private int amountOfNew = 10;

        [ObservableProperty]
        private ObservableCollection<AvailableExhibition> currentExhibitions = new();

        [ObservableProperty]
        private ObservableCollection<NewArtPiece> newestArtworks = new();

        [ObservableProperty]
        private GaleryStatistic stats = new();

        public HomeViewModel()
        {
            Load();
        }

        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                CurrentExhibitions = new ObservableCollection<AvailableExhibition>(repo.GetAvailableExhibitions());
                var list = repo.GetNewArtPieces().Take(amountOfNew).ToList();
                NewestArtworks = new ObservableCollection<NewArtPiece>(list);
                Stats = repo.GetGaleryStatistic();
            }, "Načtení domovské obrazovky selhalo");
        }
    }
}