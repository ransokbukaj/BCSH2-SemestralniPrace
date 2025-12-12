using DatabaseAccess.Interface;
using Entities;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public class ArtPieceRepository : IArtPieceRepository
    {
        public List<ArtPiece> GetList()
        {
            var list = new List<ArtPiece>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        id_dilo,
                        dilo_nazev,
                        dilo_popis,
                        typ_dila,
                        datum_zverejneni,
                        vyska,
                        sirka,
                        id_prodej,
                        id_vystava
                    FROM v_umelecka_dila";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ArtPiece
                        {
                            Id = Convert.ToInt32(reader["id_dilo"]),
                            Name = reader["dilo_nazev"].ToString(),
                            Description = reader["dilo_popis"] == DBNull.Value ? null : reader["dilo_popis"].ToString(),
                            PublishedDate = Convert.ToDateTime(reader["datum_zverejneni"]),
                            Height = Convert.ToDouble(reader["vyska"]),
                            Width = Convert.ToDouble(reader["sirka"]),
                            ExhibitionId = reader["id_vystava"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_vystava"]),
                            SaleId = reader["id_prodej"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_prodej"]),
                            Type = reader["typ_dila"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public void SaveItem(ArtPiece piece)
        {
            throw new NotImplementedException();
        }


        public void DeleteItem(int id)
        {
            throw new NotImplementedException();
        }

        public List<ArtPiece> GetListByArtistId(int artistId)
        {
            var list = new List<ArtPiece>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        id_dilo,
                        dilo_nazev,
                        typ_dila,
                        datum_zverejneni,
                        popis,
                        vyska,
                        sirka,
                        id_prodej,
                        id_vystava,
                        id_umelec
                    FROM v_dila_umelci
                    WHERE id_umelec = :artistId
                    ORDER BY dilo_nazev";

                var paramId = new OracleParameter
                {
                    ParameterName = "artistId",
                    OracleDbType = OracleDbType.Int32,
                    Value = artistId
                };
                command.Parameters.Add(paramId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ArtPiece
                        {
                            Id = Convert.ToInt32(reader["id_dilo"]),
                            Name = reader["dilo_nazev"].ToString(),
                            Description = reader["popis"] == DBNull.Value ? null : reader["popis"].ToString(),
                            PublishedDate = Convert.ToDateTime(reader["datum_zverejneni"]),
                            Height = Convert.ToDouble(reader["vyska"]),
                            Width = Convert.ToDouble(reader["sirka"]),
                            Type = reader["typ_dila"].ToString()

                        });
                    }
                }
            }
            return list;
        }

        public List<ArtPiece> GetListByExhibitionId(int exhibitionId)
        {
            var list = new List<ArtPiece>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        idumeleckedilo,
                        nazev,
                        popis,
                        datumzverejneni,
                        vyska,
                        sirka,
                        idprodej,
                        idvystava,
                        typdila
                    FROM umelecka_dila
                    WHERE idvystava = :exhibitionId
                    ORDER BY nazev";

                var paramId = new OracleParameter
                {
                    ParameterName = "exhibitionId",
                    OracleDbType = OracleDbType.Int32,
                    Value = exhibitionId
                };
                command.Parameters.Add(paramId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ArtPiece
                        {
                            Id = Convert.ToInt32(reader["idumeleckedilo"]),
                            Name = reader["nazev"].ToString(),
                            Description = reader["popis"] == DBNull.Value ? null : reader["popis"].ToString(),
                            PublishedDate = Convert.ToDateTime(reader["datumzverejneni"]),
                            Height = Convert.ToDouble(reader["vyska"]),
                            Width = Convert.ToDouble(reader["sirka"]),
                            ExhibitionId = reader["idvystava"] == DBNull.Value ? 0 : Convert.ToInt32(reader["idvystava"]),
                            SaleId = reader["idprodej"] == DBNull.Value ? 0 : Convert.ToInt32(reader["idprodej"]),
                            Type = reader["typdila"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<ArtPiece> GetListInStorage()
        {
            var list = new List<ArtPiece>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        idumeleckedilo,
                        nazev,
                        popis,
                        datumzverejneni,
                        vyska,
                        sirka,
                        idprodej,
                        idvystava,
                        typdila
                    FROM umelecka_dila
                    WHERE idvystava IS NULL 
                    ORDER BY nazev";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ArtPiece
                        {
                            Id = Convert.ToInt32(reader["idumeleckedilo"]),
                            Name = reader["nazev"].ToString(),
                            Description = reader["popis"] == DBNull.Value ? null : reader["popis"].ToString(),
                            PublishedDate = Convert.ToDateTime(reader["datumzverejneni"]),
                            Height = Convert.ToDouble(reader["vyska"]),
                            Width = Convert.ToDouble(reader["sirka"]),
                            ExhibitionId = 0,
                            SaleId = 0,
                            Type = reader["typdila"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<ArtPiece> GetListBySaleId(int saleId)
        {
            var list = new List<ArtPiece>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        idumeleckedilo,
                        nazev,
                        popis,
                        datumzverejneni,
                        vyska,
                        sirka,
                        idprodej,
                        idvystava,
                        typdila
                    FROM umelecka_dila
                    WHERE idprodej = :saleId
                    ORDER BY nazev";

                var paramId = new OracleParameter
                {
                    ParameterName = "saleId",
                    OracleDbType = OracleDbType.Int32,
                    Value = saleId
                };
                command.Parameters.Add(paramId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ArtPiece
                        {
                            Id = Convert.ToInt32(reader["idumeleckedilo"]),
                            Name = reader["nazev"].ToString(),
                            Description = reader["popis"] == DBNull.Value ? null : reader["popis"].ToString(),
                            PublishedDate = Convert.ToDateTime(reader["datumzverejneni"]),
                            Height = Convert.ToDouble(reader["vyska"]),
                            Width = Convert.ToDouble(reader["sirka"]),
                            ExhibitionId = reader["idvystava"] == DBNull.Value ? 0 : Convert.ToInt32(reader["idvystava"]),
                            SaleId = Convert.ToInt32(reader["idprodej"]),
                            Type = reader["typdila"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<ArtPiece> GetListUnsold()
        {
            var list = new List<ArtPiece>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        idumeleckedilo,
                        nazev,
                        popis,
                        datumzverejneni,
                        vyska,
                        sirka,
                        idprodej,
                        idvystava,
                        typdila
                    FROM umelecka_dila
                    WHERE idprodej is NULL
                    ORDER BY nazev";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ArtPiece
                        {
                            Id = Convert.ToInt32(reader["idumeleckedilo"]),
                            Name = reader["nazev"].ToString(),
                            Description = reader["popis"] == DBNull.Value ? null : reader["popis"].ToString(),
                            PublishedDate = Convert.ToDateTime(reader["datumzverejneni"]),
                            Height = Convert.ToDouble(reader["vyska"]),
                            Width = Convert.ToDouble(reader["sirka"]),
                            ExhibitionId = reader["idvystava"] == DBNull.Value ? 0 : Convert.ToInt32(reader["idvystava"]),
                            SaleId = reader["idprodej"] == DBNull.Value ? 0 : Convert.ToInt32(reader["idprodej"]),
                            Type = reader["typdila"].ToString()
                        });
                    }
                }
            }
            return list;
        }




        public void AddArtPieceToExhibition(int idArtpiece, int idExhibition)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_pridat_dilo_na_vystavu";

                        var paramIdArt = new OracleParameter
                        {
                            ParameterName = "p_idumeleckedilo",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = idArtpiece
                        };
                        command.Parameters.Add(paramIdArt);

                        var paramIdExhib = new OracleParameter
                        {
                            ParameterName = "p_idvystava",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = idExhibition
                        };
                        command.Parameters.Add(paramIdExhib);

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

        public void RemoveArtPieceFromExhibition(int idArtpiece, int idExhibition)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_odeber_dilo_z_vystavy";

                        var paramIdArt = new OracleParameter
                        {
                            ParameterName = "p_idumeleckedilo",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = idArtpiece
                        };
                        command.Parameters.Add(paramIdArt);

                        var paramIdExhib = new OracleParameter
                        {
                            ParameterName = "p_idvystava",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = idExhibition
                        };
                        command.Parameters.Add(paramIdExhib);

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

        public void AddArtPieceToSale(int idArtPiece, int idSale)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_pridat_dilo_do_prodeje";

                        var paramIdArt = new OracleParameter
                        {
                            ParameterName = "p_idumeleckedilo",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = idArtPiece
                        };
                        command.Parameters.Add(paramIdArt);

                        var paramIdSale = new OracleParameter
                        {
                            ParameterName = "p_idprodej",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = idSale
                        };
                        command.Parameters.Add(paramIdSale);

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

        public void RemoveArtPieceFromSale(int idArtPiece)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_odebrat_dilo_z_prodeje";

                        var paramIdArt = new OracleParameter
                        {
                            ParameterName = "p_idumeleckedilo",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = idArtPiece
                        };
                        command.Parameters.Add(paramIdArt);

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