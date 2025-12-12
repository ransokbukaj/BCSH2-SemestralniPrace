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
    public class PostRepository : IPostRepository
    {
        /// <summary>
        /// Metoda pro získání všech pošt z databáze
        /// </summary>
        /// <returns>List všech pošt.</returns>
        public List<Post> GetList()
        {
            var list = new List<Post>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        id,
                        obec,
                        psc
                    FROM v_posty";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Post
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            City = reader["obec"].ToString(),
                            PSC = reader["psc"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda pro uložení nebo úpravu posty
        /// </summary>
        /// <param name="post">Pošta k přidání nebo úložení.</param>
        public void SaveItem(Post post)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_save_posta";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idposta",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = post.Id == 0 ? (object)DBNull.Value : post.Id
                        };
                        command.Parameters.Add(paramId);

                        var paramObec = new OracleParameter
                        {
                            ParameterName = "p_obec",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = post.City
                        };
                        command.Parameters.Add(paramObec);

                        var paramPsc = new OracleParameter
                        {
                            ParameterName = "p_psc",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = post.PSC
                        };
                        command.Parameters.Add(paramPsc);

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
        /// Metoda pro odstranění určité pošty.
        /// </summary>
        /// <param name="id">Id pošty k odstranění.</param>
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
                        command.CommandText = "p_delete_posta";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idposta",
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