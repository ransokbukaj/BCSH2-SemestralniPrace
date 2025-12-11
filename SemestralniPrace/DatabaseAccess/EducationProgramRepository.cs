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
    public class EducationProgramRepository : IEducationProgramRepository
    {
        public List<EducationProgram> GetList()
        {
            var list = new List<EducationProgram>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        id,
                        nazev,
                        datum_od,
                        datum_do,
                        popis
                    FROM v_vzdelavaci_programy";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new EducationProgram
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["nazev"].ToString(),
                            From = Convert.ToDateTime(reader["datum_od"]),
                            To = Convert.ToDateTime(reader["datum_do"]),
                            Description = reader["popis"] == DBNull.Value ? null : reader["popis"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public void SaveItem(EducationProgram educationProgram)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_save_vzdelavaci_program";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idvzdelavaciprogram",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = educationProgram.Id == 0 ? (object)DBNull.Value : educationProgram.Id
                        };
                        command.Parameters.Add(paramId);

                        var paramNazev = new OracleParameter
                        {
                            ParameterName = "p_nazev",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = educationProgram.Name
                        };
                        command.Parameters.Add(paramNazev);

                        var paramDatumOd = new OracleParameter
                        {
                            ParameterName = "p_datumod",
                            OracleDbType = OracleDbType.Date,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = educationProgram.From
                        };
                        command.Parameters.Add(paramDatumOd);

                        var paramDatumDo = new OracleParameter
                        {
                            ParameterName = "p_datumdo",
                            OracleDbType = OracleDbType.Date,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = educationProgram.To
                        };
                        command.Parameters.Add(paramDatumDo);

                        var paramPopis = new OracleParameter
                        {
                            ParameterName = "p_popis",
                            OracleDbType = OracleDbType.Clob,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = string.IsNullOrEmpty(educationProgram.Description) ? (object)DBNull.Value : educationProgram.Description
                        };
                        command.Parameters.Add(paramPopis);

                        // Provedení procedury
                        command.ExecuteNonQuery();
                    }

                    // Commit transakce
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

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
                        command.CommandText = "p_delete_vzdelavaci_program";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idvzdelavaciprogram",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = id
                        };
                        command.Parameters.Add(paramId);

                        // Provedení procedury
                        command.ExecuteNonQuery();
                    }

                    // Commit transakce
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