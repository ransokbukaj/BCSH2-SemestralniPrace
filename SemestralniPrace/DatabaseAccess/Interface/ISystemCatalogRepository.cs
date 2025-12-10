using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    /// <summary>
    /// Rozhraní pro repository systémového katalogu
    /// </summary>
    public interface ISystemCatalogRepository
    {
        /// <summary>
        /// Získá kompletní systémový katalog databáze
        /// </summary>
        SystemCatalog GetSystemCatalog();

        /// <summary>
        /// Získá seznam všech tabulek
        /// </summary>
        List<TableInfo> GetTables();

        /// <summary>
        /// Získá sloupce pro konkrétní tabulku
        /// </summary>
        List<ColumnInfo> GetColumnsForTable(string tableName);

        /// <summary>
        /// Získá seznam primárních klíčů
        /// </summary>
        List<PrimaryKeyInfo> GetPrimaryKeys();

        /// <summary>
        /// Získá seznam cizích klíčů
        /// </summary>
        List<ForeignKeyInfo> GetForeignKeys();

        /// <summary>
        /// Získá seznam indexů
        /// </summary>
        List<IndexInfo> GetIndexes();

        /// <summary>
        /// Získá seznam pohledů
        /// </summary>
        List<ViewInfo> GetViews();

        /// <summary>
        /// Získá plnou SQL definici konkrétního pohledu
        /// </summary>
        string GetViewDefinition(string viewName);

        /// <summary>
        /// Získá seznam sekvencí
        /// </summary>
        List<SequenceInfo> GetSequences();

        /// <summary>
        /// Získá seznam triggerů
        /// </summary>
        List<TriggerInfo> GetTriggers();

        /// <summary>
        /// Získá seznam procedur a funkcí
        /// </summary>
        List<ProcedureInfo> GetProcedures();
    }
}