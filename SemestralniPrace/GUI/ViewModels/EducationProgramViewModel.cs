using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels
{
    public partial class EducationProgramViewModel : ObservableObject
    {
        private readonly EducationProgramRepository repository = new EducationProgramRepository();

        [ObservableProperty]
        private ObservableCollection<EducationProgram> educationPrograms = new();

        [ObservableProperty]
        private EducationProgram selectedEducationProgram;

        public EducationProgramViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            EducationPrograms = new ObservableCollection<EducationProgram>(repository.GetList());
        }

        [RelayCommand]
        private void New()
        {
            SelectedEducationProgram = new EducationProgram();
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedEducationProgram == null)
                return;

            repository.SaveItem(SelectedEducationProgram);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedEducationProgram == null || SelectedEducationProgram.Id == 0)
                return;

            repository.DeleteItem(SelectedEducationProgram.Id);
            Load();
        }
    }
}
