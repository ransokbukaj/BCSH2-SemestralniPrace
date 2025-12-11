using DatabaseAccess.Interface;
using Entities;
using Entities.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                            Authors= reader["autori"].ToString(),
                            Type = reader["typdila"].ToString()

                        });
                    }
                }
            }
            return list;
        }
    }
}
