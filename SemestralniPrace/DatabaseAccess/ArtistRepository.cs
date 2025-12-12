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
        /// <summary>
        /// Metoda pro získání všech umělců z databáze.
        /// </summary>
        /// <returns>List všech umělců</returns>
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
                        prod_dila,
                        idmentor
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
                            SoldPieces = Convert.ToInt32(reader["prod_dila"]),
                            IdOfMentor = reader["idmentor"] == DBNull.Value
                                        ? (int?)null
                                        : Convert.ToInt32(reader["idmentor"])
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda pro uložení nebo editaci umělce.
        /// </summary>
        /// <param name="artist">Umělec k přídání nebo úpravě.</param>
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

                        var paramMentor = new OracleParameter
                        {
                            ParameterName = "p_idmentor",
                            OracleDbType = OracleDbType.Int32,
                            Direction = System.Data.ParameterDirection.Input,
                            Value = artist.IdOfMentor.HasValue
                                    ? artist.IdOfMentor.Value
                                    : (object)DBNull.Value
                        };
                        command.Parameters.Add(paramMentor);

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
        /// Metoda pro smazání umělce.
        /// </summary>
        /// <param name="id">Id umělce k odebrání.</param>
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
        /// Metoda pro získání všech umělců z databáze, kteří vytviřoli dílo s určitým id.
        /// </summary>
        /// <param name="id">Id uměleckého díla.</param>
        /// <returns>List umělců, kteří vytvořili dilo s určitým id.</returns>
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
                        popis,
                        idmentor
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
                            Description = reader["popis"] == DBNull.Value ? null : reader["popis"].ToString(),
                            IdOfMentor = reader["idmentor"] == DBNull.Value
                                        ? (int?)null
                                        : Convert.ToInt32(reader["idmentor"])

                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda pro přiřazení díla k umělci za pomoci id díla id umělce.
        /// </summary>
        /// <param name="id">Id umělce.</param>
        /// <param name="idArt">Id uměleckého díla k přidání.</param>
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
        /// Metoda pro odebrání díla od umělce za pomoci id díla a id umělce.
        /// </summary>
        /// <param name="id">Id umělce</param>
        /// <param name="idArt">Id uměleckého díla k odebrání.</param>
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
        /// Metoda pro zisk všech možných mentorů k umělci s určitým id.
        /// </summary>
        /// <param name="idOfArtist">Id umělce.</param>
        /// <returns>List umělců, kteří by mohli být mentorem pro zadaného umělce.</returns>
        public List<Artist> GetAvailableMentors(int idOfArtist)
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
                        prod_dila,
                        idmentor
                    FROM v_umelci
                    WHERE id != :artistId
                    AND (idmentor IS NULL OR idmentor != :artistId) ";

                var param = command.CreateParameter();
                param.ParameterName = "artistId";
                param.Value = idOfArtist;
                command.Parameters.Add(param);

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
                            SoldPieces = Convert.ToInt32(reader["prod_dila"]),
                            IdOfMentor = reader["idmentor"] == DBNull.Value
                                        ? (int?)null
                                        : Convert.ToInt32(reader["idmentor"])
                        });
                    }
                }
            }
            return list;
        }
    }
    
}