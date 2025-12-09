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
using System.Windows.Media;

namespace GUI.ViewModels
{
    public partial class PaintingViewModel : ObservableObject
    {
        private readonly PaintingRepository repository = new PaintingRepository();
        private readonly CounterRepository counterRep = new CounterRepository();



        [ObservableProperty]
        private ObservableCollection<Painting> paintings = new();
        [ObservableProperty]
        private ObservableCollection<Counter> techniques = new();
        [ObservableProperty]
        private ObservableCollection<Counter> bases = new();



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
            Techniques = new ObservableCollection<Counter>(counterRep.GetTechniques());
            Bases = new ObservableCollection<Counter>(counterRep.GetFoundations());
        }

        [RelayCommand]
        private void New()
        {
            SelectedPainting = new Painting()
            {
                Base = new Counter(),
                Technique = new Counter()
            };
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedPainting == null)
                return;


            if (Bases != null)
            {
                // sjednotit instanci Post podle Id
                SelectedPainting.Base = Bases.FirstOrDefault(p => p.Id == SelectedPainting.Base.Id);
            }

            if (Techniques != null)
            {
                // sjednotit instanci Post podle Id
                SelectedPainting.Technique = Techniques.FirstOrDefault(p => p.Id == SelectedPainting.Technique.Id);
            }

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
    }
}
