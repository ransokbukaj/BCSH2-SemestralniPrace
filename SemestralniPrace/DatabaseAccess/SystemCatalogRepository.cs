using System;
using System.Collections.Generic;
using Entities;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace DatabaseAccess
{
    /// <summary>
    /// Repository pro práci se systémovým katalogem databáze
    /// </summary>
    public class SystemCatalogRepository
    {
        /// <summary>
        /// Získá kompletní systémový katalog databáze
        /// </summary>
        public SystemCatalog GetSystemCatalog()
        {
            var catalog = new SystemCatalog
            {
                GeneratedDate = DateTime.Now,
                DatabaseUser = GetCurrentUser()
            };

            catalog.Tables = GetTables();
            catalog.Views = GetViews();
            catalog.PrimaryKeys = GetPrimaryKeys();
            catalog.ForeignKeys = GetForeignKeys();
            catalog.Indexes = GetIndexes();
            catalog.Sequences = GetSequences();
            catalog.Triggers = GetTriggers();
            catalog.Procedures = GetProcedures();

            return catalog;
        }

        /// <summary>
        /// Získá seznam tabulek
        /// </summary>
        public List<TableInfo> GetTables()
        {
            var tables = new List<TableInfo>();

            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "BEGIN :cursor := f_get_tables_info(); END;";
                command.CommandType = System.Data.CommandType.Text;

                var cursorParam = new OracleParameter("cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                command.Parameters.Add(cursorParam);

                command.ExecuteNonQuery();

                using (var reader = ((OracleRefCursor)cursorParam.Value).GetDataReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(new TableInfo
                        {
                            TableName = reader["NAZEV_TABULKY"]?.ToString(),
                            Description = reader["POPIS"] == DBNull.Value ? null : reader["POPIS"]?.ToString(),
                            ColumnCount = Convert.ToInt32(reader["POCET_SLOUPCU"]),
                            ForeignKeyCount = Convert.ToInt32(reader["POCET_CIZICH_KLICU"]),
                            EstimatedRowCount = reader["ODHADOVANY_POCET_RADKU"] == DBNull.Value
                                ? null
                                : (int?)Convert.ToInt32(reader["ODHADOVANY_POCET_RADKU"]),
                            SizeMB = reader["VELIKOST_MB"] == DBNull.Value
                                ? null
                                : (decimal?)Convert.ToDecimal(reader["VELIKOST_MB"])
                        });
                    }
                }
            }

            return tables;
        }

        /// <summary>
        /// Získá sloupce pro konkrétní tabulku
        /// </summary>
        public List<ColumnInfo> GetColumnsForTable(string tableName)
        {
            var columns = new List<ColumnInfo>();

            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "BEGIN :cursor := f_get_columns_info(:tableName); END;";
                command.CommandType = System.Data.CommandType.Text;

                var cursorParam = new OracleParameter("cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                command.Parameters.Add(cursorParam);

                var param = new OracleParameter("tableName", OracleDbType.Varchar2, tableName, System.Data.ParameterDirection.Input);
                command.Parameters.Add(param);

                command.ExecuteNonQuery();

                using (var reader = ((OracleRefCursor)cursorParam.Value).GetDataReader())
                {
                    while (reader.Read())
                    {
                        columns.Add(new ColumnInfo
                        {
                            Order = Convert.ToInt32(reader["PORADI"]),
                            ColumnName = reader["NAZEV_SLOUPCE"]?.ToString(),
                            DataType = reader["DATOVY_TYP"]?.ToString(),
                            Nullable = reader["NULLABLE"]?.ToString(),
                            DefaultValue = reader["VYCHOZI_HODNOTA"] == DBNull.Value
                                ? null
                                : reader["VYCHOZI_HODNOTA"]?.ToString(),
                            Description = reader["POPIS"] == DBNull.Value
                                ? null
                                : reader["POPIS"]?.ToString(),
                            IsPrimaryKey = reader["JE_PRIMARNI_KLIC"]?.ToString(),
                            IsForeignKey = reader["JE_CIZI_KLIC"]?.ToString()
                        });
                    }
                }
            }

            return columns;
        }

        /// <summary>
        /// Získá seznam primárních klíčů
        /// </summary>
        public List<PrimaryKeyInfo> GetPrimaryKeys()
        {
            var primaryKeys = new List<PrimaryKeyInfo>();

            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "BEGIN :cursor := f_get_primary_keys(); END;";
                command.CommandType = System.Data.CommandType.Text;

                var cursorParam = new OracleParameter("cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                command.Parameters.Add(cursorParam);

                command.ExecuteNonQuery();

                using (var reader = ((OracleRefCursor)cursorParam.Value).GetDataReader())
                {
                    while (reader.Read())
                    {
                        primaryKeys.Add(new PrimaryKeyInfo
                        {
                            TableName = reader["NAZEV_TABULKY"]?.ToString(),
                            ConstraintName = reader["NAZEV_OMEZENI"]?.ToString(),
                            Columns = reader["SLOUPCE"]?.ToString()
                        });
                    }
                }
            }

            return primaryKeys;
        }

        /// <summary>
        /// Získá seznam cizích klíčů
        /// </summary>
        public List<ForeignKeyInfo> GetForeignKeys()
        {
            var foreignKeys = new List<ForeignKeyInfo>();

            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "BEGIN :cursor := f_get_foreign_keys(); END;";
                command.CommandType = System.Data.CommandType.Text;

                var cursorParam = new OracleParameter("cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                command.Parameters.Add(cursorParam);

                command.ExecuteNonQuery();

                using (var reader = ((OracleRefCursor)cursorParam.Value).GetDataReader())
                {
                    while (reader.Read())
                    {
                        foreignKeys.Add(new ForeignKeyInfo
                        {
                            SourceTable = reader["TABULKA_ZDROJ"]?.ToString(),
                            ConstraintName = reader["NAZEV_OMEZENI"]?.ToString(),
                            SourceColumns = reader["SLOUPCE_ZDROJ"]?.ToString(),
                            TargetTable = reader["TABULKA_CIL"]?.ToString(),
                            TargetColumns = reader["SLOUPCE_CIL"]?.ToString(),
                            DeleteRule = reader["PRAVIDLO_MAZANI"]?.ToString()
                        });
                    }
                }
            }

            return foreignKeys;
        }

        /// <summary>
        /// Získá seznam indexů
        /// </summary>
        public List<IndexInfo> GetIndexes()
        {
            var indexes = new List<IndexInfo>();

            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "BEGIN :cursor := f_get_indexes_info(); END;";
                command.CommandType = System.Data.CommandType.Text;

                var cursorParam = new OracleParameter("cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                command.Parameters.Add(cursorParam);

                command.ExecuteNonQuery();

                using (var reader = ((OracleRefCursor)cursorParam.Value).GetDataReader())
                {
                    while (reader.Read())
                    {
                        indexes.Add(new IndexInfo
                        {
                            TableName = reader["NAZEV_TABULKY"]?.ToString(),
                            IndexName = reader["NAZEV_INDEXU"]?.ToString(),
                            IndexType = reader["TYP_INDEXU"]?.ToString(),
                            Columns = reader["SLOUPCE"]?.ToString(),
                            RowCount = reader["POCET_RADKU"] == DBNull.Value
                                ? null
                                : (int?)Convert.ToInt32(reader["POCET_RADKU"]),
                            DistinctKeys = reader["POCET_UNIKALTNICH_HODNOT"] == DBNull.Value
                                ? null
                                : (int?)Convert.ToInt32(reader["POCET_UNIKALTNICH_HODNOT"]),
                            Status = reader["STAV"]?.ToString()
                        });
                    }
                }
            }

            return indexes;
        }

        /// <summary>
        /// Získá seznam pohledů
        /// </summary>
        public List<ViewInfo> GetViews()
        {
            var views = new List<ViewInfo>();

            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "BEGIN :cursor := f_get_views_info(); END;";
                command.CommandType = System.Data.CommandType.Text;

                var cursorParam = new OracleParameter("cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                command.Parameters.Add(cursorParam);

                command.ExecuteNonQuery();

                using (var reader = ((OracleRefCursor)cursorParam.Value).GetDataReader())
                {
                    while (reader.Read())
                    {
                        views.Add(new ViewInfo
                        {
                            ViewName = reader["NAZEV_POHLEDU"]?.ToString(),
                            Description = reader["POPIS"] == DBNull.Value
                                ? null
                                : reader["POPIS"]?.ToString(),
                            ColumnCount = Convert.ToInt32(reader["POCET_SLOUPCU"]),
                            Definition = null  // Vždy NULL kvůli LONG datatype omezení
                        });
                    }
                }
            }

            return views;
        }

        /// <summary>
        /// Získá seznam sekvencí
        /// </summary>
        public List<SequenceInfo> GetSequences()
        {
            var sequences = new List<SequenceInfo>();

            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "BEGIN :cursor := f_get_sequences_info(); END;";
                command.CommandType = System.Data.CommandType.Text;

                var cursorParam = new OracleParameter("cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                command.Parameters.Add(cursorParam);

                command.ExecuteNonQuery();

                using (var reader = ((OracleRefCursor)cursorParam.Value).GetDataReader())
                {
                    while (reader.Read())
                    {
                        sequences.Add(new SequenceInfo
                        {
                            SequenceName = reader["NAZEV_SEKVENCE"]?.ToString(),
                            MinValue = SafeConvertToLong(reader["MINIMALNI_HODNOTA"]),
                            MaxValue = SafeConvertToLong(reader["MAXIMALNI_HODNOTA"]),
                            IncrementBy = reader["KROK"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["KROK"]),
                            LastValue = SafeConvertToLong(reader["POSLEDNI_HODNOTA"]),
                            Cache = reader["CACHE"]?.ToString(),
                            IsCyclic = reader["CYKLICKY"]?.ToString(),
                            IsOrdered = reader["SERAZENY"]?.ToString()
                        });
                    }
                }
            }

            return sequences;
        }

        /// <summary>
        /// Bezpečně převede hodnotu na long, pokud je v rozsahu, jinak vrátí null
        /// </summary>
        private long? SafeConvertToLong(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            try
            {
                decimal decimalValue = Convert.ToDecimal(value);

                // Zkontroluj, jestli se hodnota vejde do long
                if (decimalValue > long.MaxValue || decimalValue < long.MinValue)
                    return null; // Hodnota je příliš velká/malá

                return (long)decimalValue;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Získá seznam triggerů
        /// </summary>
        public List<TriggerInfo> GetTriggers()
        {
            var triggers = new List<TriggerInfo>();

            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "BEGIN :cursor := f_get_triggers_info(); END;";
                command.CommandType = System.Data.CommandType.Text;

                var cursorParam = new OracleParameter("cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                command.Parameters.Add(cursorParam);

                command.ExecuteNonQuery();

                using (var reader = ((OracleRefCursor)cursorParam.Value).GetDataReader())
                {
                    while (reader.Read())
                    {
                        triggers.Add(new TriggerInfo
                        {
                            TriggerName = reader["NAZEV_TRIGGERU"]?.ToString(),
                            TableName = reader["NAZEV_TABULKY"]?.ToString(),
                            Event = reader["UDALOST"]?.ToString(),
                            TriggerType = reader["TYP"]?.ToString(),
                            Status = reader["STAV"]?.ToString()
                        });
                    }
                }
            }

            return triggers;
        }

        /// <summary>
        /// Získá seznam procedur a funkcí
        /// </summary>
        public List<ProcedureInfo> GetProcedures()
        {
            var procedures = new List<ProcedureInfo>();

            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "BEGIN :cursor := f_get_procedures_info(); END;";
                command.CommandType = System.Data.CommandType.Text;

                var cursorParam = new OracleParameter("cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                command.Parameters.Add(cursorParam);

                command.ExecuteNonQuery();

                using (var reader = ((OracleRefCursor)cursorParam.Value).GetDataReader())
                {
                    while (reader.Read())
                    {
                        procedures.Add(new ProcedureInfo
                        {
                            Name = reader["NAZEV"]?.ToString(),
                            Type = reader["TYP"]?.ToString(),
                            Status = reader["STAV"]?.ToString(),
                            ParameterCount = reader["POCET_PARAMETRU"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["POCET_PARAMETRU"]),
                            CreatedDate = reader["DATUM_VYTVORENI"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(reader["DATUM_VYTVORENI"]),
                            LastModified = reader["POSLEDNI_ZMENA"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(reader["POSLEDNI_ZMENA"])
                        });
                    }
                }
            }

            return procedures;
        }

        /// <summary>
        /// Získá plnou SQL definici konkrétního pohledu
        /// </summary>
        /// <param name="viewName">Název pohledu</param>
        /// <returns>SQL definice pohledu nebo null pokud pohled neexistuje</returns>
        public string GetViewDefinition(string viewName)
        {
            if (string.IsNullOrWhiteSpace(viewName))
                return "Nebyl vybrán žádný pohled.";

            try
            {
                using (var command = ConnectionManager.Connection.CreateCommand())
                {
                    // Pro čtení LONG musíme nastavit InitialLONGFetchSize
                    command.CommandText = "SELECT text FROM user_views WHERE view_name = :viewName";
                    command.Parameters.Add(new OracleParameter("viewName", viewName.ToUpper()));

                    // Nastavení velikosti pro čtení LONG - důležité pro velké definice
                    command.InitialLONGFetchSize = -1; // -1 znamená načíst celý LONG

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.IsDBNull(0))
                                return $"Pohled '{viewName}' nemá definici.";

                            // Čtení LONG jako string
                            string definition = reader.GetString(0);

                            if (string.IsNullOrWhiteSpace(definition))
                                return $"Pohled '{viewName}' má prázdnou definici.";

                            return definition;
                        }
                        else
                        {
                            return $"Pohled '{viewName}' nebyl nalezen.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Chyba při načítání definice pohledu '{viewName}':\n{ex.Message}";
            }
        }

        /// <summary>
        /// Získá jméno aktuálního databázového uživatele
        /// </summary>
        private string GetCurrentUser()
        {
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "SELECT USER FROM DUAL";
                return command.ExecuteScalar()?.ToString();
            }
        }
    }
}