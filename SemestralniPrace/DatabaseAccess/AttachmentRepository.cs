using DatabaseAccess.Interface;
using Entities;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public class AttachmentRepository : IAttachmentRepository
    {
        public List<Attachment> GetList()
        {
            throw new NotImplementedException();
        }

        public List<Attachment> GetListByArtPieceId(int id)
        {
            var list = new List<Attachment>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        idpriloha,
                        soubor,
                        typsouboru,
                        nazevsouboru
                   
                    FROM prilohy
                    WHERE idumeleckedilo = :umeleckeId
                    ORDER BY nazevsouboru";

                var paramId = new OracleParameter
                {
                    ParameterName = "umeleckeId",
                    OracleDbType = OracleDbType.Int32,
                    Value = id
                };
                command.Parameters.Add(paramId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        
                        list.Add(new Attachment
                        {
                            Id = Convert.ToInt32(reader["idpriloha"]),
                            File = (byte[])reader["soubor"],
                            FileType = reader["typsouboru"].ToString(),
                            FileName = reader["nazevsouboru"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public void SaveItem(Attachment attachment, int artId)
        {
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "p_save_priloha";

                var paramFile = new OracleParameter
                {
                    ParameterName = "p_soubor",
                    OracleDbType = OracleDbType.Blob,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = attachment.File
                };
                command.Parameters.Add(paramFile);

                var paramType = new OracleParameter
                {
                    ParameterName = "p_typsouboru",
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = attachment.FileType
                };
                command.Parameters.Add(paramType);

                var paramName = new OracleParameter
                {
                    ParameterName = "p_nazevsouboru",
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = attachment.FileName
                };
                command.Parameters.Add(paramName);


                var paramArtId = new OracleParameter
                {
                    ParameterName = "p_idumeleckedilo",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = artId
                };
                command.Parameters.Add(paramArtId);

                var paramAttId = new OracleParameter
                {
                    ParameterName = "p_idpriloha",
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = attachment.Id == 0 ? (object)DBNull.Value : attachment.Id
                };
                command.Parameters.Add(paramAttId);

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
            throw new NotImplementedException();
        }

      
    }
}
