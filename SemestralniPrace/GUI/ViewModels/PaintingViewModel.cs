using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities.Data;

using DatabaseAccess;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels
{

    public partial class PaintingViewModel : ObservableObject
    {
        private readonly PaintingRepository repository = new PaintingRepository();

        [ObservableProperty]
        private ObservableCollection<Painting> paintings = new();

        [ObservableProperty]
        private Painting selectedPainting;

        public PaintingViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            Paintings = new ObservableCollection<Painting>(repository.GetList());
        }

        [RelayCommand]
        private void New()
        {
            SelectedPainting = new Painting();
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedPainting == null)
                return;

            repository.SaveItem(SelectedPainting);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedPainting == null || SelectedPainting.Id == 0)
                return;

            repository.DeleteItem(SelectedPainting.Id);
            Load();
        }

    public class PaintingViewModel : ObservableObject
    {
        private PaintingRepository paintingRepository;
        private CounterRepository counterRepository;

    }
}
