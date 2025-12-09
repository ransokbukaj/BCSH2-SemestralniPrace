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
        public List<Visit> GetList()
        {
            var list = new List<Visit>();
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
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
            }
            return list;
        }

        public void SaveItem(Visit visit)
        {
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
                {
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

                    // Provedení procedury
                    command.ExecuteNonQuery();

                    // Commit transakce
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
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

        public void DeleteItem(int id)
        {
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
                {
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

                    // Provedení procedury
                    command.ExecuteNonQuery();

                    // Commit transakce
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
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
    }
}
