using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace GUI.ViewModels
{
    public partial class SystemCatalogViewModel : ObservableObject
    {
        private readonly SystemCatalogRepository repository = new SystemCatalogRepository();

        [ObservableProperty]
        private SystemCatalog catalog;

        [ObservableProperty]
        private ObservableCollection<TableInfo> tables = new();

        [ObservableProperty]
        private ObservableCollection<ViewInfo> views = new();

        [ObservableProperty]
        private ObservableCollection<PrimaryKeyInfo> primaryKeys = new();

        [ObservableProperty]
        private ObservableCollection<ForeignKeyInfo> foreignKeys = new();

        [ObservableProperty]
        private ObservableCollection<IndexInfo> indexes = new();

        [ObservableProperty]
        private ObservableCollection<SequenceInfo> sequences = new();

        [ObservableProperty]
        private ObservableCollection<TriggerInfo> triggers = new();

        [ObservableProperty]
        private ObservableCollection<ProcedureInfo> procedures = new();

        [ObservableProperty]
        private TableInfo selectedTable;

        [ObservableProperty]
        private ObservableCollection<ColumnInfo> tableColumns = new();

        [ObservableProperty]
        private ViewInfo selectedView;

        [ObservableProperty]
        private string viewDefinition;

        public SystemCatalogViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            Catalog = repository.GetSystemCatalog();

            Tables = new ObservableCollection<TableInfo>(Catalog.Tables ?? Enumerable.Empty<TableInfo>());
            Views = new ObservableCollection<ViewInfo>(Catalog.Views ?? Enumerable.Empty<ViewInfo>());
            PrimaryKeys = new ObservableCollection<PrimaryKeyInfo>(Catalog.PrimaryKeys ?? Enumerable.Empty<PrimaryKeyInfo>());
            ForeignKeys = new ObservableCollection<ForeignKeyInfo>(Catalog.ForeignKeys ?? Enumerable.Empty<ForeignKeyInfo>());
            Indexes = new ObservableCollection<IndexInfo>(Catalog.Indexes ?? Enumerable.Empty<IndexInfo>());
            Sequences = new ObservableCollection<SequenceInfo>(Catalog.Sequences ?? Enumerable.Empty<SequenceInfo>());
            Triggers = new ObservableCollection<TriggerInfo>(Catalog.Triggers ?? Enumerable.Empty<TriggerInfo>());
            Procedures = new ObservableCollection<ProcedureInfo>(Catalog.Procedures ?? Enumerable.Empty<ProcedureInfo>());
        }

        partial void OnSelectedTableChanged(TableInfo value)
        {
            if (value != null)
            {
                var columns = repository.GetColumnsForTable(value.TableName);
                TableColumns = new ObservableCollection<ColumnInfo>(columns);
            }
            else
            {
                TableColumns.Clear();
            }
        }

        partial void OnSelectedViewChanged(ViewInfo value)
        {
            if (value != null)
            {
                ViewDefinition = repository.GetViewDefinition(value.ViewName);
            }
            else
            {
                ViewDefinition = null;
            }
        }
    }
}