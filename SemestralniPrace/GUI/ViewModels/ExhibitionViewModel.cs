using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels
{
    public partial class ExhibitionViewModel : ObservableObject
    {

        private readonly ExhibitionRepository repository = new ExhibitionRepository();
        private readonly ArtPieceRepository artRepo = new ArtPieceRepository();

        [ObservableProperty]
        private ObservableCollection<Exhibition> exhibitions = new();

        [ObservableProperty]
        private ObservableCollection<ArtPiece> availableArtPieces = new();

        [ObservableProperty]
        private ObservableCollection<ArtPiece> exhibitionArtPieces = new();



        [ObservableProperty]
        private Exhibition selectedExhibition;

        [ObservableProperty]
        private ArtPiece selectedArtPieceToRemove;

        [ObservableProperty]
        private ArtPiece selectedArtPieceToAdd;

        partial void OnSelectedExhibitionChanged(Exhibition value)
        {
            if(SelectedExhibition != null)
            {
                ExhibitionArtPieces = new ObservableCollection<ArtPiece>(artRepo.GetListByExhibitionId(SelectedExhibition.Id));
                AvailableArtPieces = new ObservableCollection<ArtPiece>(artRepo.GetListInStorage());
            }
        }
        [RelayCommand]
        private void AddArtPiece()
        {

        }
        [RelayCommand]
        private void RemoveArtPiece()
        {

        }





        public ExhibitionViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            Exhibitions = new ObservableCollection<Exhibition>(repository.GetList());
        }

        [RelayCommand]
        private void New()
        {
            SelectedExhibition = new Exhibition();
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedExhibition == null)
                return;

            repository.SaveItem(SelectedExhibition);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedExhibition == null || SelectedExhibition.Id == 0)
                return;

            repository.DeleteItem(SelectedExhibition.Id);
            Load();
        }
    }
}
