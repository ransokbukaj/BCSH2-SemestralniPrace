using DatabaseAccess.Interface;
using Entities;
using Entities.Home;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace DatabaseAccess
{
    public class UtilityRepository : IUtilityRepository
    {
        /// <summary>
        /// Metoda pro zisk všech stále dostupných výstav
        /// </summary>
        /// <returns>List všech stále dostupných výstav </returns>
        public List<AvailableExhibition> GetAvailableExhibitions()
        {
            var list = new List<AvailableExhibition>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        idvystava,
                        nazev,
                        datumod,
                        datumdo,
                        popis,
                        nazev_programu
                    FROM v_aktualni_vystavy";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AvailableExhibition
                        {
                            Id = Convert.ToInt32(reader["idvystava"]),
                            Name = reader["nazev"].ToString(),
                            From = Convert.ToDateTime(reader["datumod"]),
                            To = Convert.ToDateTime(reader["datumdo"]),
                            Description = reader["popis"] == DBNull.Value ? null : reader["popis"].ToString(),
                            EducationProgramName = reader["nazev_programu"].ToString()

                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda pro získání GaleryStaticstic, objektu obsahujicí data o galerii.
        /// </summary>
        /// <returns>Objekt GaleryStatistics</returns>
        public GaleryStatistics GetGaleryStatistic()
        {
            var stat = new GaleryStatistics();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        pocet_umelecky_del,
                        pocet_obrazu,
                        pocet_soch,
                        pocet_vystav,
                        pocet_vzdelavacich_programu,
                        pocet_umelcu,
                        pocet_navstevniku,
                        trzba_za_prodej
                    FROM v_statistiky_galerie";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stat = new GaleryStatistics
                        {
                            ArtPieceAmount = Convert.ToInt32(reader["pocet_umelecky_del"]),
                            PaintingAmount = Convert.ToInt32(reader["pocet_obrazu"]),
                            SculptureAmount = Convert.ToInt32(reader["pocet_soch"]),
                            ExhibitonAmount = Convert.ToInt32(reader["pocet_vystav"]),
                            EducationProgramAmount = Convert.ToInt32(reader["pocet_vzdelavacich_programu"]),
                            ArtistAmount = Convert.ToInt32(reader["pocet_umelcu"]),
                            VisitorAmount = Convert.ToInt32(reader["pocet_navstevniku"]),
                            Sales = Convert.ToDouble(reader["trzba_za_prodej"])

                        };
                    }
                }
            }
            return stat;
        }

        /// <summary>
        /// Metoda pro získání nejnovějších uměleckých děl.
        /// </summary>
        /// <returns>List nejnovějších děl</returns>
        public List<NewArtPiece> GetNewArtPieces()
        {
            var list = new List<NewArtPiece>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        idumeleckedilo,
                        nazev,
                        datumzverejneni,
                        autori,
                        typdila
                    FROM v_nejnovejsi_um_dila";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new NewArtPiece
                        {
                            Id = Convert.ToInt32(reader["idumeleckedilo"]),
                            Name = reader["nazev"].ToString(),
                            PublishedDate = Convert.ToDateTime(reader["datumzverejneni"]),
                            Authors = reader["autori"].ToString(),
                            Type = reader["typdila"].ToString()

                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda pro získání objektu ArtistStatistic, který obsahuje informace o umělci.
        /// </summary>
        /// <param name="artistId">Id umělce od kterého cheme získat ArtistStatistic</param>
        /// <returns>Objekt ArtistStatistic</returns>
        public ArtistStatistics GetArtistStatistic(int artistId)
        {
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "p_statistiky_umelce";


                var pId = new OracleParameter
                {
                    ParameterName = "p_idumelec",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Input,
                    Value = artistId
                };
                command.Parameters.Add(pId);

  
                var pPocetDel = new OracleParameter
                {
                    ParameterName = "o_pocet_del",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(pPocetDel);

                var pPocetProdanych = new OracleParameter
                {
                    ParameterName = "o_pocet_prodanych_del",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(pPocetProdanych);

                var pTrzba = new OracleParameter
                {
                    ParameterName = "o_trzba_celkem",
                    OracleDbType = OracleDbType.Double,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(pTrzba);

                var pMin = new OracleParameter
                {
                    ParameterName = "o_cena_min",
                    OracleDbType = OracleDbType.Double,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(pMin);

                var pMax = new OracleParameter
                {
                    ParameterName = "o_cena_max",
                    OracleDbType = OracleDbType.Double,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(pMax);

                var pAvg = new OracleParameter
                {
                    ParameterName = "o_cena_prumer",
                    OracleDbType = OracleDbType.Double,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(pAvg);


                command.ExecuteNonQuery();
                var result = new ArtistStatistics
                {
                    AmounArtPiece = ((OracleDecimal)pPocetDel.Value).ToInt32(),
                    AmountSold = ((OracleDecimal)pPocetProdanych.Value).ToInt32(),
                    FullProfit = ((OracleDecimal)pTrzba.Value).ToDouble(),
                    MinProfit = ((OracleDecimal)pMin.Value).ToDouble(),
                    MaxProfit = ((OracleDecimal)pMax.Value).ToDouble(),
                    AvgProfit = ((OracleDecimal)pAvg.Value).ToDouble()
                };

                return result;
            }
        }

        /// <summary>
        /// Metoda pro získání objektu MentorBranchStatistic, obsahujího informace o linii umělců.
        /// </summary>
        /// <param name="id">Id uživate, který je mentorem linie.</param>
        /// <returns>Objekt MentorBranchStatistics</returns>
        public MentorBranchStatistics GetMentorBranchStatics(int id)
        {
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "p_trzba_mentorske_vetve";

                var pId = new OracleParameter
                {
                    ParameterName = "p_idmentor",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Input,
                    Value = id
                };
                command.Parameters.Add(pId);

                var amountOfArtist = new OracleParameter
                {
                    ParameterName = "o_pocet_umelcu",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(amountOfArtist);

                var amountOfSale = new OracleParameter
                {
                    ParameterName = "o_pocet_prodeju",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(amountOfSale);

                var totalProfit = new OracleParameter
                {
                    ParameterName = "o_trzba_celkem",
                    OracleDbType = OracleDbType.Double,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(totalProfit);

                command.ExecuteNonQuery();

                var result = new MentorBranchStatistics
                {
                    ArtistsInBranch = ((OracleDecimal)amountOfArtist.Value).ToInt32(),
                    AmountofSales = ((OracleDecimal)amountOfSale.Value).ToInt32(),
                    TotalProfit = ((OracleDecimal)totalProfit.Value).ToDouble(),
                };

                return result;

            }
        }

        /// <summary>
        /// Metoda pro získání MostSuccesfulMentore, který má informace o nejúspěšnějším studentovy v linii.
        /// </summary>
        /// <param name="artistId">Id mentora od kterého chceme získat nejúspěšnějšího studenta linie. </param>
        /// <returns>Objekt MostSuccesfulMentore</returns>
        public MostSuccesfulMentore GetMostSuccesfulMentore(int artistId)
        {
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "p_nejuspesnejsi_potomek";

                var idOfMentor = new OracleParameter
                {
                    ParameterName = "p_idmentor",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Input,
                    Value = artistId
                };
                command.Parameters.Add(idOfMentor);

                var idOfMentore = new OracleParameter
                {
                    ParameterName = "o_idpotomek",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(idOfMentore);

                var nameOfMentore = new OracleParameter
                {
                    ParameterName = "o_jmenopotomek",
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = ParameterDirection.Output,
                    Size = 5000
                };
                command.Parameters.Add(nameOfMentore);

                var amountOfArt = new OracleParameter
                {
                    ParameterName = "o_pocet_del",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(amountOfArt);

                command.ExecuteNonQuery();

                int? potomekId = null;
                if (idOfMentore.Value != DBNull.Value)
                {
                    var od = (OracleDecimal)idOfMentore.Value;
                    potomekId = od.IsNull ? (int?)null : od.ToInt32();
                }

                string? potomekJmeno = null;
                if (nameOfMentore.Value != DBNull.Value)
                {
                    var os = (OracleString)nameOfMentore.Value;
                    potomekJmeno = os.IsNull ? null : os.Value;
                }

                int? pocetDel = null;
                if (amountOfArt.Value != DBNull.Value)
                {
                    var od = (OracleDecimal)amountOfArt.Value;
                    pocetDel = od.IsNull ? (int?)null : od.ToInt32();
                }

                var result = new MostSuccesfulMentore
                {
                    ArtistId = potomekId ?? 0,          
                    ArtistName = potomekJmeno ,
                    AmountOfArtPieces = pocetDel
                };

                return result;
            }
        }

        /// <summary>
        /// Metoda pro získáni informací o umělce za pomocí objektu UserStatistics.
        /// </summary>
        /// <param name="userId">Id uživatele od kterého cheme UserStatistics</param>
        /// <returns></returns>
        public UserStatistics GetUserStatistics(int userId)
        {
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "p_aktivita_uzivatele";

                var pId = new OracleParameter
                {
                    ParameterName = "p_iduzivatel",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Input,
                    Value = userId
                };
                command.Parameters.Add(pId);

                var amountOfChanges = new OracleParameter
                {
                    ParameterName = "o_pocet_zmen",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(amountOfChanges);

                var amountOfInsert = new OracleParameter
                {
                    ParameterName = "o_pocet_insertu",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(amountOfInsert);

                var amountOfUpdate = new OracleParameter
                {
                    ParameterName = "o_pocet_updatu",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(amountOfUpdate);

                var amountOfDelete = new OracleParameter
                {
                    ParameterName = "o_pocet_deletu",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(amountOfDelete);

                var lastChange = new OracleParameter
                {
                    ParameterName = "o_posledni_zmena",
                    OracleDbType = OracleDbType.Date,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(lastChange);

                var isDisabled = new OracleParameter
                {
                    ParameterName = "o_deaktivovan",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(isDisabled);

                command.ExecuteNonQuery();

                var oLast = (OracleDate)lastChange.Value;
                
                var result = new UserStatistics
                {
                    AmountOfChanges = ((OracleDecimal)amountOfChanges.Value).ToInt32(),
                    AmountOfInsert = ((OracleDecimal)amountOfInsert.Value).ToInt32(),
                    AmountOfDelete = ((OracleDecimal)amountOfDelete.Value).ToInt32(),
                    AmountOfUpdate = ((OracleDecimal)amountOfUpdate.Value).ToInt32(),

                    LastChange = oLast.IsNull ? DateTime.MinValue : oLast.Value

                };

                return result;
            }
        }
    }


}
