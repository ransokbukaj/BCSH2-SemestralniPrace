using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace DatabaseAccess
{
    public class AddressRepository : IAddressRepository
    {
        public List<Address> GetList()
        {
            var list = new List<Address>();
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
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
            }
            return list;
        }

        public void SaveItem(Address address)
        {
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
                {
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
                    command.CommandText = "p_delete_adresa";

                    var paramId = new OracleParameter
                    {
                        ParameterName = "p_idadresa",
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
