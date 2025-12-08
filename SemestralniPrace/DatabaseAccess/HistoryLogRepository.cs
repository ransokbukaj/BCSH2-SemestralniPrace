using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities;

namespace DatabaseAccess
{
    public class HistoryLogRepository : IHistoryLogRepository
    {
        public List<HistoryLog> GetList()
        {
            var list = new List<HistoryLog>();
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT 
                            id,
                            datum_zmeny,
                            popis_zmeny,
                            druh_operace,
                            stare_hodnoty,
                            nove_hodnoty,
                            nazev_tabulky,
                            id_radku_tabulky,
                            id_zmeneneho_radku,
                            id_uzivatel,
                            uzivatelske_jmeno,
                            uzivatel_jmeno,
                            uzivatel_prijmeni,
                            nazev_role
                        FROM v_zaznamy_historie";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new HistoryLog
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                DateOfChange = Convert.ToDateTime(reader["datum_zmeny"]),
                                DescriptionOfChnage = reader["popis_zmeny"] == DBNull.Value ? null : reader["popis_zmeny"].ToString(),
                                TypeOfOperation = reader["druh_operace"].ToString(),
                                OldValues = reader["stare_hodnoty"] == DBNull.Value ? null : reader["stare_hodnoty"].ToString(),
                                NewValues = reader["nove_hodnoty"] == DBNull.Value ? null : reader["nove_hodnoty"].ToString(),
                                TableName = reader["nazev_tabulky"].ToString(),
                                TableRowId = Convert.ToInt32(reader["id_radku_tabulky"]),
                                EditedRowId = Convert.ToInt32(reader["id_zmeneneho_radku"]),
                                User = new User
                                {
                                    Id = Convert.ToInt32(reader["id_uzivatel"]),
                                    Username = reader["uzivatelske_jmeno"].ToString(),
                                    FirstName = reader["uzivatel_jmeno"].ToString(),
                                    LastName = reader["uzivatel_prijmeni"].ToString(),
                                    Role = new Counter
                                    {
                                        Name = reader["nazev_role"].ToString()
                                    }
                                }
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}