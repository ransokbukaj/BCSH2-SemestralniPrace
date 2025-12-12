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
    public class PaintingRepository : IPaintingRepository
    {
        /// <summary>
        /// Metoda pro získání všech maleb z databáze
        /// </summary>
        /// <returns>List všech maleb</returns>
        public List<Painting> GetList()
        {
            var list = new List<Painting>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        id,
                        nazev,
                        popis,
                        datum_zverejneni,
                        vyska,
                        sirka,
                        id_prodej,
                        id_vystava,
                        id_podklad,
                        nazev_podkladu,
                        id_technika,
                        nazev_techniky
                    FROM v_obrazy";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Painting
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["nazev"].ToString(),
                            Description = reader["popis"] == DBNull.Value ? null : reader["popis"].ToString(),
                            PublishedDate = Convert.ToDateTime(reader["datum_zverejneni"]),
                            Height = Convert.ToDouble(reader["vyska"]),
                            Width = Convert.ToDouble(reader["sirka"]),
                            SaleId = reader["id_prodej"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_prodej"]),
                            ExhibitionId = reader["id_vystava"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_vystava"]),
                            Base = new Counter
                            {
                                Id = Convert.ToInt32(reader["id_podklad"]),
                                Name = reader["nazev_podkladu"].ToString()
                            },
                            Technique = new Counter
                            {
                                Id = Convert.ToInt32(reader["id_technika"]),
                                Name = reader["nazev_techniky"].ToString()
                            }
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda pro přidání nebo úpravu malby
        /// </summary>
        /// <param name="painting">Malba k přidání nebo úpravě</param>
        public void SaveItem(Painting painting)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_save_obraz";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idumeleckedilo",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = painting.Id == 0 ? (object)DBNull.Value : painting.Id
                        };
                        command.Parameters.Add(paramId);

                        var paramNazev = new OracleParameter
                        {
                            ParameterName = "p_nazev",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = painting.Name
                        };
                        command.Parameters.Add(paramNazev);

                        var paramPopis = new OracleParameter
                        {
                            ParameterName = "p_popis",
                            OracleDbType = OracleDbType.Clob,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = string.IsNullOrEmpty(painting.Description) ? (object)DBNull.Value : painting.Description
                        };
                        command.Parameters.Add(paramPopis);

                        var paramDatum = new OracleParameter
                        {
                            ParameterName = "p_datumzverejneni",
                            OracleDbType = OracleDbType.Date,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = painting.PublishedDate
                        };
                        command.Parameters.Add(paramDatum);

                        var paramVyska = new OracleParameter
                        {
                            ParameterName = "p_vyska",
                            OracleDbType = OracleDbType.Decimal,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = painting.Height
                        };
                        command.Parameters.Add(paramVyska);

                        var paramSirka = new OracleParameter
                        {
                            ParameterName = "p_sirka",
                            OracleDbType = OracleDbType.Decimal,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = painting.Width
                        };
                        command.Parameters.Add(paramSirka);

                        var paramProdej = new OracleParameter
                        {
                            ParameterName = "p_idprodej",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = painting.SaleId == 0 ? (object)DBNull.Value : painting.SaleId
                        };
                        command.Parameters.Add(paramProdej);

                        var paramVystava = new OracleParameter
                        {
                            ParameterName = "p_idvystava",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = painting.ExhibitionId == 0 ? (object)DBNull.Value : painting.ExhibitionId
                        };
                        command.Parameters.Add(paramVystava);

                        var paramPodklad = new OracleParameter
                        {
                            ParameterName = "p_idpodklad",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = painting.Base.Id
                        };
                        command.Parameters.Add(paramPodklad);

                        var paramTechnika = new OracleParameter
                        {
                            ParameterName = "p_idtechnika",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = painting.Technique.Id
                        };
                        command.Parameters.Add(paramTechnika);

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
        /// Metoda pro odstranění určité malby
        /// </summary>
        /// <param name="id">Id malby k odstranení.</param>
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
                        command.CommandText = "p_delete_obraz";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idumeleckedilo",
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