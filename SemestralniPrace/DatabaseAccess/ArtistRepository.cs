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
    public class ArtistRepository : IArtistRepository
    {
        public List<Artist> GetList()
        {
            var list = new List<Artist>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        id,
                        jmeno,
                        prijmeni,
                        datum_narozeni,
                        datum_umrti,
                        popis,
                        prum_cena,
                        prod_dila
                    FROM v_umelci";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Artist
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            FirstName = reader["jmeno"].ToString(),
                            LastName = reader["prijmeni"].ToString(),
                            DateOfBirth = Convert.ToDateTime(reader["datum_narozeni"]),
                            DateOfDeath = reader["datum_umrti"] == DBNull.Value
                                ? DateTime.MinValue
                                : Convert.ToDateTime(reader["datum_umrti"]),
                            Description = reader["popis"] == DBNull.Value ? null : reader["popis"].ToString(),
                            AvgPrice = Convert.ToDouble(reader["prum_cena"]),
                            SoldPieces = Convert.ToInt32(reader["prod_dila"])
                        });
                    }
                }
            }
            return list;
        }

        public void SaveItem(Artist artist)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_save_umelec";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idumelec",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = artist.Id == 0 ? (object)DBNull.Value : artist.Id
                        };
                        command.Parameters.Add(paramId);

                        var paramJmeno = new OracleParameter
                        {
                            ParameterName = "p_jmeno",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = artist.FirstName
                        };
                        command.Parameters.Add(paramJmeno);

                        var paramPrijmeni = new OracleParameter
                        {
                            ParameterName = "p_prijmeni",
                            OracleDbType = OracleDbType.Varchar2,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = artist.LastName
                        };
                        command.Parameters.Add(paramPrijmeni);

                        var paramDatumNarozeni = new OracleParameter
                        {
                            ParameterName = "p_datumnarozeni",
                            OracleDbType = OracleDbType.Date,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = artist.DateOfBirth
                        };
                        command.Parameters.Add(paramDatumNarozeni);

                        var paramDatumUmrti = new OracleParameter
                        {
                            ParameterName = "p_datumumrti",
                            OracleDbType = OracleDbType.Date,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = artist.DateOfDeath == DateTime.MinValue ? (object)DBNull.Value : artist.DateOfDeath
                        };
                        command.Parameters.Add(paramDatumUmrti);

                        var paramPopis = new OracleParameter
                        {
                            ParameterName = "p_popis",
                            OracleDbType = OracleDbType.Clob,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = string.IsNullOrEmpty(artist.Description) ? (object)DBNull.Value : artist.Description
                        };
                        command.Parameters.Add(paramPopis);

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
                        command.CommandText = "p_delete_umelec";

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idumelec",
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
        public List<Artist> GetListByArtPieceId(int id)
        {
            var list = new List<Artist>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        id,
                        jmeno,
                        prijmeni,
                        datum_narozeni,
                        datum_umrti,
                        popis
                    FROM v_umelci_dila
                    WHERE idumeleckedilo = :idumldila";

                var paramId = new OracleParameter
                {
                    ParameterName = "idumldila",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = id
                };
                command.Parameters.Add(paramId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Artist
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            FirstName = reader["jmeno"].ToString(),
                            LastName = reader["prijmeni"].ToString(),
                            DateOfBirth = Convert.ToDateTime(reader["datum_narozeni"]),
                            DateOfDeath = reader["datum_umrti"] == DBNull.Value
                                ? DateTime.MinValue
                                : Convert.ToDateTime(reader["datum_umrti"]),
                            Description = reader["popis"] == DBNull.Value ? null : reader["popis"].ToString()

                        });
                    }
                }
            }
            return list;
        }

        public void AddArtistToArtPiece(int id, int idArt)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_pridat_umelece_k_dilu";

                        var paramArtId = new OracleParameter
                        {
                            ParameterName = "p_idumeleckedilo",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = idArt
                        };
                        command.Parameters.Add(paramArtId);

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idumelec",
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

        public void RemoveArtistFromArtPiece(int id, int idArt)
        {
            using (var transaction = ConnectionManager.Connection.BeginTransaction())
            {
                try
                {
                    using (var command = ConnectionManager.Connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "p_odebrat_umelece_od_dila";

                        var paramArtId = new OracleParameter
                        {
                            ParameterName = "p_idumeleckedilo",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = idArt
                        };
                        command.Parameters.Add(paramArtId);

                        var paramId = new OracleParameter
                        {
                            ParameterName = "p_idumelec",
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