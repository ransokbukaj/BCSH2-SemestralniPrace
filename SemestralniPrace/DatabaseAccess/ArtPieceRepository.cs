using DatabaseAccess.Interface;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    internal class ArtPieceRepository : IArtPieceRepository
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
                        


                    FROM v_vystavy";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ArtPiece
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["nazev"].ToString(),



                           
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
    }
}
