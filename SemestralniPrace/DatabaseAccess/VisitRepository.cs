using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities;
using Oracle.ManagedDataAccess.Client;

namespace DatabaseAccess
{
    public class VisitRepository : IVisitRepository
    {
        /// <summary>
        /// Metoda pro získání všech návštev z databáze.
        /// </summary>
        /// <returns>List všech návštěv.</returns>
        public List<Visit> GetList()
        {
            var list = new List<Visit>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        id,
                        datum_navstevy,
                        id_druh_navstevy,
                        nazev_druhu_navstevy,
                        cena,
                        id_vystava,
                        nazev_vystavy
                    FROM v_navstevy";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Visit
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            DateOfVisit = Convert.ToDateTime(reader["datum_navstevy"]),
                            VisitType = new VisitType
                            {
                                Id = Convert.ToInt32(reader["id_druh_navstevy"]),
                                Name = reader["nazev_druhu_navstevy"].ToString(),
                                Price = Convert.ToDecimal(reader["cena"])
                            },
                            ExhibitionCounter = new Counter
                            {
                                Id = Convert.ToInt32(reader["id_vystava"]),
                                Name = reader["nazev_vystavy"].ToString()
                            }
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda pro přidání nebo upravení návštěvy
        /// </summary>
        /// <param name="visit">Návštěva k přidání nebo upravení</param>
        /// <param name="idExhibit">Id výstavy na které návšteva proběhla.</param>
        public void SaveItem(Visit visit, int idExhibit)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_save_navsteva";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idnavsteva",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = visit.Id == 0 ? (object)DBNull.Value : visit.Id
                        };
                        command.Parameters.Add(paramId);

                        var paramDatum = new OracleParameter
                        {
                            ParameterName = "p_datumnavstevy",
                            OracleDbType = OracleDbType.Date,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = visit.DateOfVisit
                        };
                        command.Parameters.Add(paramDatum);

                        var paramDruhNavstevy = new OracleParameter
                        {
                            ParameterName = "p_iddruhnavstevy",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = visit.VisitType.Id
                        };
                        command.Parameters.Add(paramDruhNavstevy);

                        var paramVystava = new OracleParameter
                        {
                            ParameterName = "p_idvystava",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = visit.ExhibitionCounter.Id
                        };
                        command.Parameters.Add(paramVystava);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Metoda k odstraněni návstěvy
        /// </summary>
        /// <param name="id">Id návštěvy k odstranění.</param>
        public void DeleteItem(int id)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_delete_navsteva";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idnavsteva",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = id
                        };
                        command.Parameters.Add(paramId);

                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}