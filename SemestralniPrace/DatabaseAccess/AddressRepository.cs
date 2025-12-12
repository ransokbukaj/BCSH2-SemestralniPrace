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
    public class AddressRepository : IAddressRepository
    {
        /// <summary>
        /// Metoda pro zisk všech adres nacházejicích se v databázi.
        /// </summary>
        /// <returns>List všech adres</returns>
        public List<Address> GetList()
        {
            var list = new List<Address>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        id,
                        ulice,
                        cislo_popisne,
                        cislo_orientacni,
                        id_posta,
                        obec,
                        psc
                    FROM v_adresy";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Address
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Street = reader["ulice"].ToString(),
                            HouseNumber = reader["cislo_popisne"].ToString(),
                            StreetNumber = reader["cislo_orientacni"] == DBNull.Value ? null : reader["cislo_orientacni"].ToString(),
                            Post = new Post
                            {
                                Id = Convert.ToInt32(reader["id_posta"]),
                                City = reader["obec"].ToString(),
                                PSC = reader["psc"].ToString()
                            }
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda pro uložení a editaci adres.
        /// </summary>
        /// <param name="address">Adresa k přidání nebo úpravě.</param>
        public void SaveItem(Address address)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_save_adresa";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idadresa",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = address.Id == 0 ? (object)DBNull.Value : address.Id
                        };
                        command.Parameters.Add(paramId);

                        var paramUlice = new OracleParameter
                        {
                            ParameterName = "p_ulice",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = address.Street
                        };
                        command.Parameters.Add(paramUlice);

                        var paramCisloPopisne = new OracleParameter
                        {
                            ParameterName = "p_cislopopisne",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = address.HouseNumber
                        };
                        command.Parameters.Add(paramCisloPopisne);

                        var paramCisloOrientacni = new OracleParameter
                        {
                            ParameterName = "p_cisloorientacni",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = string.IsNullOrEmpty(address.StreetNumber) ? (object)DBNull.Value : address.StreetNumber
                        };
                        command.Parameters.Add(paramCisloOrientacni);

                        var paramIdPosta = new OracleParameter
                        {
                            ParameterName = "p_idposta",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = address.Post.Id
                        };
                        command.Parameters.Add(paramIdPosta);
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
        /// Metoda pro smazání adresy z databáze.
        /// </summary>
        /// <param name="id">Id adresy k odebrání.</param>
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
                        command.CommandText = "p_delete_adresa";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idadresa",
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
