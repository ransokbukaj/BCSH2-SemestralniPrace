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

        private readonly EducationProgramRepository educationProgramRepository = new EducationProgramRepository();
        private readonly ExhibitionRepository exhibitionRepository = new ExhibitionRepository();

        //List pro načítání progrmů
        private List<EducationProgram> _allEducationPrograms = new();
        //List pro zobrazování programů
        [ObservableProperty]
        private ObservableCollection<EducationProgram> educationPrograms = new();

        //List pro zobrazení dostupných výstav
        [ObservableProperty]
        private ObservableCollection<Exhibition> availableExhibitions = new();

        //List pro zobrazení výstav v programu
        [ObservableProperty]
        private ObservableCollection<Exhibition> exhibitionsInProgram = new();

        [ObservableProperty]
        private EducationProgram selectedEducationProgram;

        [ObservableProperty]
        private Exhibition selectedExhibitionToRemove;

        [ObservableProperty]
        private Exhibition selectedExhibitionToAdd;

        [ObservableProperty]
        private string searchText;

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        partial void OnSelectedEducationProgramChanged(EducationProgram value)
        {
            if (SelectedEducationProgram != null)
            {
                ErrorHandler.SafeExecute(() =>
                {
                    var assignedIds = new HashSet<int>(exhibitionRepository.GetListByProgramId(SelectedEducationProgram.Id).Select(a => a.Id));
                    var coll = exhibitionRepository.GetList().Where(a => !assignedIds.Contains(a.Id)).ToList();
                    AvailableExhibitions = new ObservableCollection<Exhibition>(coll);
                    ExhibitionsInProgram = new ObservableCollection<Exhibition>(exhibitionRepository.GetListByProgramId(SelectedEducationProgram.Id));
                }, "Načtení výstav pro program selhalo");
            }
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
                _allEducationPrograms = educationProgramRepository.GetList();
                ApplyFilter();
            }, "Načtení vzdělávacích programů selhalo");
        }

        /// <summary>
        /// Metoda pro filtrování obsahu podle jména a popisu vzdělávacího progamu.
        /// </summary>
        private void ApplyFilter()
        {
            var text = (SearchText ?? "").Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                EducationPrograms = new ObservableCollection<EducationProgram>(_allEducationPrograms);
                return;
            }

            text = text.ToLowerInvariant();

            var filtered = _allEducationPrograms.Where(p =>
                    (!string.IsNullOrEmpty(p.Name) && p.Name.ToLowerInvariant().Contains(text)) ||
                    (!string.IsNullOrEmpty(p.Description) && p.Description.ToLowerInvariant().Contains(text))
                )
                .ToList();

            EducationPrograms = new ObservableCollection<EducationProgram>(filtered);

            if (SelectedEducationProgram != null && !filtered.Contains(SelectedEducationProgram))
                SelectedEducationProgram = filtered.FirstOrDefault();
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

                educationProgramRepository.SaveItem(SelectedEducationProgram);
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
                educationProgramRepository.DeleteItem(SelectedEducationProgram.Id);
                Load();
            }, "Smazání vzdělávacího programu selhalo");
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
                exhibitionRepository.AddExhibitionToProgram(SelectedExhibitionToAdd.Id, SelectedEducationProgram.Id);
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
                exhibitionRepository.RemoveExhibitionFromProgram(SelectedExhibitionToRemove.Id);
                AvailableExhibitions.Add(SelectedExhibitionToRemove);
                ExhibitionsInProgram.Remove(SelectedExhibitionToRemove);
                SelectedExhibitionToRemove = ExhibitionsInProgram.FirstOrDefault();
            }, "Odebrání výstavy z programu selhalo");
        }
    }
}