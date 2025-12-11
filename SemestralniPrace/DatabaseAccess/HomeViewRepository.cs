using DatabaseAccess.Interface;
using Entities;
using Entities.Home;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace DatabaseAccess
{
    public class HomeViewRepository : IHomeViewRepository
    {
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

        public GaleryStatistic GetGaleryStatistic()
        {
            var stat = new GaleryStatistic();
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
                        stat = new GaleryStatistic
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

        public ArtistStatistic GetArtistStatistic(int artistId)
        {
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "p_statistiky_umelce";

                // IN parametr – ID umělce
                var pId = new OracleParameter
                {
                    ParameterName = "p_idumelec",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Input,
                    Value = artistId
                };
                command.Parameters.Add(pId);

                // OUT parametry
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

                // Vykonání procedury
                command.ExecuteNonQuery();

                // Naplnění objektu
                var result = new ArtistStatistic
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



        public MentorBranchStatistics GetMentorBranchStatics(int id)
        {
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "p_trzba_mentorske_vetve";

                // IN parametr – ID umělce
                var pId = new OracleParameter
                {
                    ParameterName = "p_idmentor",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Input,
                    Value = id
                };
                command.Parameters.Add(pId);

                // OUT parametry
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

                // Vykonání procedury
                command.ExecuteNonQuery();

                // Naplnění objektu
                var result = new MentorBranchStatistics
                {
                    ArtistsInBranch = ((OracleDecimal)amountOfArtist.Value).ToInt32(),
                    AmountofSales = ((OracleDecimal)amountOfSale.Value).ToInt32(),
                    TotalProfit = ((OracleDecimal)totalProfit.Value).ToDouble(),
                };

                return result;

            }
        }

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

                // OUT parametry
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
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(nameOfMentore);

                var amountOfArt = new OracleParameter
                {
                    ParameterName = "o_pocet_del",
                    OracleDbType = OracleDbType.Double,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(amountOfArt);

                // Vykonání procedury
                command.ExecuteNonQuery();

                // Naplnění objektu
                var result = new MostSuccesfulMentore
                {
                    ArtistId = ((OracleDecimal)idOfMentore.Value).ToInt32(),
                    ArtistName = nameOfMentore.Value.ToString(),
                    AmountOfArtPieces = ((OracleDecimal)amountOfArt.Value).ToInt32(),

                };

                return result;
            }
        }

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

                // OUT parametry
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
                    OracleDbType = OracleDbType.Date,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(isDisabled);



                // Vykonání procedury
                command.ExecuteNonQuery();

                // Naplnění objektu
                var result = new UserStatistics
                {
                    AmountOfChanges = ((OracleDecimal)amountOfChanges.Value).ToInt32(),
                    AmountOfInserts = ((OracleDecimal)amountOfInsert.Value).ToInt32(),
                    AmountOfDelete = ((OracleDecimal)amountOfDelete.Value).ToInt32(),
                    AmountOfUpdate = ((OracleDecimal)amountOfUpdate.Value).ToInt32(),
                    LastChange = (DateTime)lastChange.Value

                };

                return result;
            }
        }
    }


}
