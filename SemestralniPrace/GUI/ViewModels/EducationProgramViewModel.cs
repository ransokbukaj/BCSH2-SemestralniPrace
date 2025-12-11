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

namespace GUI.ViewModels
{
    public partial class EducationProgramViewModel : ObservableObject
    {

        private readonly EducationProgramRepository repository = new EducationProgramRepository();
        private readonly ExhibitionRepository exhRepo = new ExhibitionRepository();

        [ObservableProperty]
        private ObservableCollection<EducationProgram> educationPrograms = new();

        [ObservableProperty]
        private ObservableCollection<Exhibition> availableExhibitions = new();
        [ObservableProperty]
        private ObservableCollection<Exhibition> exhibitionsInProgram = new();


        [ObservableProperty]
        private EducationProgram selectedEducationProgram;

        [ObservableProperty]
        private Exhibition selectedExhibitionToRemove;
        [ObservableProperty]
        private Exhibition selectedExhibitionToAdd;

        partial void OnSelectedEducationProgramChanged(EducationProgram value)
        {
            if(SelectedEducationProgram != null)
            {
                var assignedIds = new HashSet<int>(exhRepo.GetListByProgramId(SelectedEducationProgram.Id).Select(a => a.Id));
                var coll = exhRepo.GetList().Where(a => !assignedIds.Contains(a.Id)).ToList();
                AvailableExhibitions = new ObservableCollection<Exhibition>(coll);
                ExhibitionsInProgram = new ObservableCollection<Exhibition>(exhRepo.GetListByProgramId(SelectedEducationProgram.Id));
            }
        }
        [RelayCommand]
        private void AddExhibitionToProgram()
        {
            if(SelectedEducationProgram != null && SelectedExhibitionToAdd != null)
            {
                exhRepo.AddExhibitionToProgram(SelectedExhibitionToAdd.Id, SelectedEducationProgram.Id);
                ExhibitionsInProgram.Add(SelectedExhibitionToAdd);
                AvailableExhibitions.Remove(SelectedExhibitionToAdd);

                SelectedExhibitionToAdd = AvailableExhibitions.FirstOrDefault();
            }
        }

        [RelayCommand]
        private void RemoveExhibitionFromProgram()
        {
            if(SelectedEducationProgram != null)
            {
                exhRepo.RemoveExhibitionFromProgram(SelectedExhibitionToRemove.Id);
                AvailableExhibitions.Add(SelectedExhibitionToRemove);
                ExhibitionsInProgram.Remove(SelectedExhibitionToRemove);

                SelectedExhibitionToRemove = ExhibitionsInProgram.FirstOrDefault();
            }
        }


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
