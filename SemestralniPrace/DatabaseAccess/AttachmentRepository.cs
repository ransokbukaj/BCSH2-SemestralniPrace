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
        /// <summary>
        /// Metoda pro získání všech příloh k určitému uměleckému dílu.
        /// </summary>
        /// <param name="id">Id uměleckého díla.</param>
        /// <returns>List příloh patřících k určítému dílu.</returns>
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

        /// <summary>
        /// Metoda pro uložení přílohy k učitému uměleckému dílu.
        /// </summary>
        /// <param name="attachment">Příloha k danému dílu</param>
        /// <param name="artId">Id uměleckého díla.</param>
        public void SaveItem(Attachment attachment, int artId)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
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
        /// Metoda pro odtránění přílohy.
        /// </summary>
        /// <param name="id">    </param>
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
                        command.CommandText = "p_delete_priloha";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idpriloha",
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