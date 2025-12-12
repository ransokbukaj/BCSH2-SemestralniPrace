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
        private readonly UtilityRepository utilityRepository = new UtilityRepository();

        //pomocná proměná pro určení kolik nových děl se má zobrazit.
        private int amountOfNew = 10;

        [ObservableProperty]
        private ObservableCollection<AvailableExhibition> currentExhibitions = new();

        [ObservableProperty]
        private ObservableCollection<NewArtPiece> newestArtworks = new();

        //Objekt s informacemi o galerii
        [ObservableProperty]
        private GaleryStatistics stats = new();

        public HomeViewModel()
        {
            Load();
        }

        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                CurrentExhibitions = new ObservableCollection<AvailableExhibition>(utilityRepository.GetAvailableExhibitions());
                var list = utilityRepository.GetNewArtPieces().Take(amountOfNew).ToList();
                NewestArtworks = new ObservableCollection<NewArtPiece>(list);
                Stats = utilityRepository.GetGaleryStatistic();
            }, "Načtení domovské obrazovky selhalo");
        }
    }
}