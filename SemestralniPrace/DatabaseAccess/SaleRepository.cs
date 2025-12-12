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
    public class SaleRepository : ISaleRepository
    {
        /// <summary>
        /// Metod pro získání všech prodejů z databáze.
        /// </summary>
        /// <returns>List všech prodejů</returns>
        public List<Sale> GetList()
        {
            var list = new List<Sale>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        id,
                        cena,
                        datum_prodeje,
                        cislo_karty,
                        cislo_uctu,
                        id_druh_platby,
                        nazev_druhu_platby,
                        id_kupec,
                        kupec_jmeno,
                        kupec_prijmeni
                    FROM v_prodeje";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Sale
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Price = Convert.ToDecimal(reader["cena"]),
                            DateOfSale = Convert.ToDateTime(reader["datum_prodeje"]),
                            CardNumber = reader["cislo_karty"] == DBNull.Value ? null : reader["cislo_karty"].ToString(),
                            AccountNumber = reader["cislo_uctu"] == DBNull.Value ? null : reader["cislo_uctu"].ToString(),
                            TypeOfPayment = new Counter
                            {
                                Id = Convert.ToInt32(reader["id_druh_platby"]),
                                Name = reader["nazev_druhu_platby"].ToString()
                            },
                            Buyer = new Buyer
                            {
                                Id = Convert.ToInt32(reader["id_kupec"]),
                                FirstName = reader["kupec_jmeno"].ToString(),
                                LastName = reader["kupec_prijmeni"].ToString()
                            }
                        });
                    }
                }
            }
            return list;
        }



        /// <summary>
        /// Metoda pro přidání nebo upravení prodeje.
        /// </summary>
        /// <param name="sale">Prodej pro přidání nebo úpravu.</param>
        public void SaveItem(Sale sale)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_save_prodej";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idprodej",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = sale.Id == 0 ? (object)DBNull.Value : sale.Id
                        };
                        command.Parameters.Add(paramId);

                        var paramCena = new OracleParameter
                        {
                            ParameterName = "p_cena",
                            OracleDbType = OracleDbType.Decimal,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = sale.Price
                        };
                        command.Parameters.Add(paramCena);

                        var paramDatum = new OracleParameter
                        {
                            ParameterName = "p_datumprodeje",
                            OracleDbType = OracleDbType.Date,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = sale.DateOfSale
                        };
                        command.Parameters.Add(paramDatum);

                        var paramCisloKarty = new OracleParameter
                        {
                            ParameterName = "p_cislokarty",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = string.IsNullOrEmpty(sale.CardNumber) ? (object)DBNull.Value : sale.CardNumber
                        };
                        command.Parameters.Add(paramCisloKarty);

                        var paramCisloUctu = new OracleParameter
                        {
                            ParameterName = "p_cislouctu",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = string.IsNullOrEmpty(sale.AccountNumber) ? (object)DBNull.Value : sale.AccountNumber
                        };
                        command.Parameters.Add(paramCisloUctu);

                        var paramDruhPlatby = new OracleParameter
                        {
                            ParameterName = "p_iddruhplatby",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = sale.TypeOfPayment.Id
                        };
                        command.Parameters.Add(paramDruhPlatby);

                        var paramKupec = new OracleParameter
                        {
                            ParameterName = "p_idkupec",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = sale.Buyer.Id
                        };
                        command.Parameters.Add(paramKupec);

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
        /// Metoda pro odstranění určitého prodeje.
        /// </summary>
        /// <param name="id">Id prodeje k odstranění.</param>
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
                        command.CommandText = "p_delete_prodej";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idprodej",
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