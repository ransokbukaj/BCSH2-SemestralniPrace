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
    public class SculptureRepository : ISculptureRepository
    {
        public List<Sculpture> GetList()
        {
            var list = new List<Sculpture>();
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
                        hloubka,
                        hmotnost,
                        id_material,
                        nazev_materialu
                    FROM v_sochy";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Sculpture
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["nazev"].ToString(),
                            Description = reader["popis"] == DBNull.Value ? null : reader["popis"].ToString(),
                            PublishedDate = Convert.ToDateTime(reader["datum_zverejneni"]),
                            Height = Convert.ToDouble(reader["vyska"]),
                            Width = Convert.ToDouble(reader["sirka"]),
                            SaleId = reader["id_prodej"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_prodej"]),
                            ExhibitionId = reader["id_vystava"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_vystava"]),
                            Depth = Convert.ToDouble(reader["hloubka"]),
                            Weight = Convert.ToDouble(reader["hmotnost"]),
                            Material = new Counter
                            {
                                Id = Convert.ToInt32(reader["id_material"]),
                                Name = reader["nazev_materialu"].ToString()
                            }
                        });
                    }
                }
            }
            return list;
        }

        public void SaveItem(Sculpture sculpture)
        {
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "p_save_socha";

                var paramId = new OracleParameter
                {
                    ParameterName = "p_idumeleckedilo",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = sculpture.Id == 0 ? (object)DBNull.Value : sculpture.Id
                };
                command.Parameters.Add(paramId);

                var paramNazev = new OracleParameter
                {
                    ParameterName = "p_nazev",
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = sculpture.Name
                };
                command.Parameters.Add(paramNazev);

                var paramPopis = new OracleParameter
                {
                    ParameterName = "p_popis",
                    OracleDbType = OracleDbType.Clob,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = string.IsNullOrEmpty(sculpture.Description) ? (object)DBNull.Value : sculpture.Description
                };
                command.Parameters.Add(paramPopis);

                var paramDatum = new OracleParameter
                {
                    ParameterName = "p_datumzverejneni",
                    OracleDbType = OracleDbType.Date,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = sculpture.PublishedDate
                };
                command.Parameters.Add(paramDatum);

                var paramVyska = new OracleParameter
                {
                    ParameterName = "p_vyska",
                    OracleDbType = OracleDbType.Decimal,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = sculpture.Height
                };
                command.Parameters.Add(paramVyska);

                var paramSirka = new OracleParameter
                {
                    ParameterName = "p_sirka",
                    OracleDbType = OracleDbType.Decimal,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = sculpture.Width
                };
                command.Parameters.Add(paramSirka);

                var paramProdej = new OracleParameter
                {
                    ParameterName = "p_idprodej",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = sculpture.SaleId == 0 ? (object)DBNull.Value : sculpture.SaleId
                };
                command.Parameters.Add(paramProdej);

                var paramVystava = new OracleParameter
                {
                    ParameterName = "p_idvystava",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = sculpture.ExhibitionId == 0 ? (object)DBNull.Value : sculpture.ExhibitionId
                };
                command.Parameters.Add(paramVystava);

                var paramHloubka = new OracleParameter
                {
                    ParameterName = "p_hloubka",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = sculpture.Depth
                };
                command.Parameters.Add(paramHloubka);

                var paramHmotnost = new OracleParameter
                {
                    ParameterName = "p_hmotnost",
                    OracleDbType = OracleDbType.Decimal,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = sculpture.Weight
                };
                command.Parameters.Add(paramHmotnost);

                var paramMaterial = new OracleParameter
                {
                    ParameterName = "p_idmaterial",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = sculpture.Material.Id
                };
                command.Parameters.Add(paramMaterial);

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
                command.CommandText = "p_delete_socha";

                var paramId = new OracleParameter
                {
                    ParameterName = "p_idumeleckedilo",
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
