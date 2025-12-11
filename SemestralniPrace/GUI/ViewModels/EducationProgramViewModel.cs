using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using GUI.Helpers;
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
            if (SelectedEducationProgram != null)
            {
                ErrorHandler.SafeExecute(() =>
                {
                    var assignedIds = new HashSet<int>(exhRepo.GetListByProgramId(SelectedEducationProgram.Id).Select(a => a.Id));
                    var coll = exhRepo.GetList().Where(a => !assignedIds.Contains(a.Id)).ToList();
                    AvailableExhibitions = new ObservableCollection<Exhibition>(coll);
                    ExhibitionsInProgram = new ObservableCollection<Exhibition>(exhRepo.GetListByProgramId(SelectedEducationProgram.Id));
                }, "Načtení výstav pro program selhalo");
            }
        }

        [RelayCommand]
        private void AddExhibitionToProgram()
        {
            if (SelectedEducationProgram == null || SelectedEducationProgram.Id == 0)
            {
                ErrorHandler.ShowError("Chyba", "Nejprve uložte vzdělávací program");
                return;
            }

            if (SelectedExhibitionToAdd == null)
            {
                ErrorHandler.ShowError("Chyba", "Vyberte výstavu k přidání");
                return;
            }

            ErrorHandler.SafeExecute(() =>
            {
                exhRepo.AddExhibitionToProgram(SelectedExhibitionToAdd.Id, SelectedEducationProgram.Id);
                ExhibitionsInProgram.Add(SelectedExhibitionToAdd);
                AvailableExhibitions.Remove(SelectedExhibitionToAdd);
                SelectedExhibitionToAdd = AvailableExhibitions.FirstOrDefault();
            }, "Přidání výstavy do programu selhalo");
        }

        [RelayCommand]
        private void RemoveExhibitionFromProgram()
        {
            if (SelectedEducationProgram == null || SelectedEducationProgram.Id == 0)
            {
                ErrorHandler.ShowError("Chyba", "Nejprve uložte vzdělávací program");
                return;
            }

            if (SelectedExhibitionToRemove == null)
            {
                ErrorHandler.ShowError("Chyba", "Vyberte výstavu k odebrání");
                return;
            }

            ErrorHandler.SafeExecute(() =>
            {
                exhRepo.RemoveExhibitionFromProgram(SelectedExhibitionToRemove.Id);
                AvailableExhibitions.Add(SelectedExhibitionToRemove);
                ExhibitionsInProgram.Remove(SelectedExhibitionToRemove);
                SelectedExhibitionToRemove = ExhibitionsInProgram.FirstOrDefault();
            }, "Odebrání výstavy z programu selhalo");
        }

        public EducationProgramViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                EducationPrograms = new ObservableCollection<EducationProgram>(repository.GetList());
            }, "Načtení vzdělávacích programů selhalo");
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

            ErrorHandler.SafeExecute(() =>
            {
                if (string.IsNullOrWhiteSpace(SelectedEducationProgram.Name))
                {
                    ErrorHandler.ShowError("Validační chyba", "Název programu nesmí být prázdný");
                    return;
                }

                if (string.IsNullOrWhiteSpace(SelectedEducationProgram.Description))
                {
                    ErrorHandler.ShowError("Validační chyba", "Popis programu nesmí být prázdný");
                    return;
                }

                if (SelectedEducationProgram.From > SelectedEducationProgram.To)
                {
                    ErrorHandler.ShowError("Validační chyba", "Datum začátku musí být před datem konce");
                    return;
                }

                repository.SaveItem(SelectedEducationProgram);
                Load();
            }, "Uložení vzdělávacího programu selhalo");
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedEducationProgram == null || SelectedEducationProgram.Id == 0)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                repository.DeleteItem(SelectedEducationProgram.Id);
                Load();
            }, "Smazání vzdělávacího programu selhalo");
        }
    }
}