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
    public class BuyerRepository : IBuyerRepository
    {

        /// <summary>
        /// Metoda pro získání všech kupců z databáze.
        /// </summary>
        /// <returns>List všech kupců.</returns>
        public List<Buyer> GetList()
        {
            var list = new List<Buyer>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        id,
                        jmeno,
                        prijmeni,
                        telefonni_cislo,
                        email,
                        id_adresa,
                        ulice,
                        cislo_popisne,
                        cislo_orientacni,
                        id_posta,
                        obec,
                        psc
                    FROM v_kupci";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Buyer
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            FirstName = reader["jmeno"].ToString(),
                            LastName = reader["prijmeni"].ToString(),
                            PhoneNumber = reader["telefonni_cislo"].ToString(),
                            Email = reader["email"] == DBNull.Value ? null : reader["email"].ToString(),
                            Adress = new Address
                            {
                                Id = Convert.ToInt32(reader["id_adresa"]),
                                Street = reader["ulice"].ToString(),
                                HouseNumber = reader["cislo_popisne"].ToString(),
                                StreetNumber = reader["cislo_orientacni"] == DBNull.Value ? null : reader["cislo_orientacni"].ToString(),
                                Post = new Post
                                {
                                    Id = Convert.ToInt32(reader["id_posta"]),
                                    City = reader["obec"].ToString(),
                                    PSC = reader["psc"].ToString()
                                }
                            }
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda pro přidání nebo upravení určitého kupce.
        /// </summary>
        /// <param name="buyer">Kupec k přidání nebo upravení.</param>
        public void SaveItem(Buyer buyer)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_save_kupec";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idkupec",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = buyer.Id == 0 ? (object)DBNull.Value : buyer.Id
                        };
                        command.Parameters.Add(paramId);

                        var paramJmeno = new OracleParameter
                        {
                            ParameterName = "p_jmeno",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = buyer.FirstName
                        };
                        command.Parameters.Add(paramJmeno);

                        var paramPrijmeni = new OracleParameter
                        {
                            ParameterName = "p_prijmeni",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = buyer.LastName
                        };
                        command.Parameters.Add(paramPrijmeni);

                        var paramTelefon = new OracleParameter
                        {
                            ParameterName = "p_telefonicislo",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = buyer.PhoneNumber
                        };
                        command.Parameters.Add(paramTelefon);

                        var paramEmail = new OracleParameter
                        {
                            ParameterName = "p_email",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = string.IsNullOrEmpty(buyer.Email) ? (object)DBNull.Value : buyer.Email
                        };
                        command.Parameters.Add(paramEmail);

                        var paramIdAdresa = new OracleParameter
                        {
                            ParameterName = "p_idadresa",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = buyer.Adress.Id
                        };
                        command.Parameters.Add(paramIdAdresa);
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
        /// Metoda pro odstranění kupce.
        /// </summary>
        /// <param name="id">Id kupce k odstranění.</param>
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
                        command.CommandText = "p_delete_kupec";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idkupec",
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