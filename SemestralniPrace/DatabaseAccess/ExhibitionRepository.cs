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
    public class ExhibitionRepository : IExhibitionRepository
    {
        public List<Exhibition> GetList()
        {
            var list = new List<Exhibition>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        id,
                        nazev,
                        datum_od,
                        datum_do,
                        popis,
                        id_vzdelavaci_program
                    FROM v_vystavy";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Exhibition
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["nazev"].ToString(),
                            From = Convert.ToDateTime(reader["datum_od"]),
                            To = Convert.ToDateTime(reader["datum_do"]),
                            Description = reader["popis"] == DBNull.Value ? null : reader["popis"].ToString(),
                            EducationProgramId = reader["id_vzdelavaci_program"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["id_vzdelavaci_program"])
                        });
                    }
                }
            }
            return list;
        }

        public void SaveItem(Exhibition exhibition)
        {
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "p_save_vystava";

                var paramId = new OracleParameter
                {
                    ParameterName = "p_idvystava",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = exhibition.Id == 0 ? (object)DBNull.Value : exhibition.Id
                };
                command.Parameters.Add(paramId);

                var paramNazev = new OracleParameter
                {
                    ParameterName = "p_nazev",
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = exhibition.Name
                };
                command.Parameters.Add(paramNazev);

                var paramDatumOd = new OracleParameter
                {
                    ParameterName = "p_datumod",
                    OracleDbType = OracleDbType.Date,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = exhibition.From
                };
                command.Parameters.Add(paramDatumOd);

                var paramDatumDo = new OracleParameter
                {
                    ParameterName = "p_datumdo",
                    OracleDbType = OracleDbType.Date,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = exhibition.To
                };
                command.Parameters.Add(paramDatumDo);

                var paramPopis = new OracleParameter
                {
                    ParameterName = "p_popis",
                    OracleDbType = OracleDbType.Clob,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = string.IsNullOrEmpty(exhibition.Description) ? (object)DBNull.Value : exhibition.Description
                };
                command.Parameters.Add(paramPopis);

                var paramProgram = new OracleParameter
                {
                    ParameterName = "p_idvzdelavaciprogram",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = exhibition.EducationProgramId == 0 ? (object)DBNull.Value : exhibition.EducationProgramId
                };
                command.Parameters.Add(paramProgram);

                // Provedení procedury
                command.ExecuteNonQuery();

                // Commit transakce
                using (var transaction = ConnectionManager.Connection.BeginTransaction())
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

        public void DeleteItem(int id)
        {
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "p_delete_vystava";

                var paramId = new OracleParameter
                {
                    ParameterName = "p_idvystava",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = id
                };
                command.Parameters.Add(paramId);

                // Provedení procedury
                command.ExecuteNonQuery();

                // Commit transakce
                using (var transaction = ConnectionManager.Connection.BeginTransaction())
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
