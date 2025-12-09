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
            using (var command = ConnectionManager.Connection.CreateCommand())
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
                        uzivatelske_jmeno,
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
                            Username = reader["uzivatelske_jmeno"].ToString()                               
                        });
                    }
                }
            }
            return list;
        }
    }
}