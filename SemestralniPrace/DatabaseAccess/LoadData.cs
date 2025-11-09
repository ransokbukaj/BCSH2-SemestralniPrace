using DatabaseAccess.Interface;
using Entities.Data;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public class LoadData : ILoadData
    {
        private static string userID = "";
        private static string password = "";
        private static string dataSource = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=fei-sql3.upceucebny.cz)(PORT=1521)) (CONNECT_DATA=(SID=BDAS)));";
        private static string connectionString = $"User Id={userID};Password={password};{dataSource}";
        
        public static void Accsess(OracleConnection conn)
        {
            //using (OracleConnection connection = new OracleConnection(connectionString))
            using (conn)
            {
                Console.WriteLine("Připojeno k databázi!");

                using (OracleCommand cmd = conn.CreateCommand())
                {

                    //Pokusný příkaz
                    cmd.CommandText = "SELECT employee_id,first_name,last_name FROM new_emps";
                                        
                    //cmd.Parameters.Add(new OracleParameter("dept", 10));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string firstName = reader.GetString(1);
                            string lastName = reader.GetString(2);
                            Console.WriteLine($"{id} - {firstName} {lastName}");
                        }
                    }
                }
            }
        }

        public List<Adress> LoadAdresses()
        {
            throw new NotImplementedException();
        }

        public List<Artist> LoadArtists()
        {
            throw new NotImplementedException();
        }

        public List<ArtPiece> LoadArtPieces()
        {
            throw new NotImplementedException();
        }

        public List<Buyer> LoadBuyers()
        {
            throw new NotImplementedException();
        }

        public List<EducationProgram> LoadEducationPrograms()
        {
            throw new NotImplementedException();
        }

        public List<Exhibition> LoadExhibitions()
        {
            throw new NotImplementedException();
        }

        public List<Foundation> LoadFoundations()
        {
            throw new NotImplementedException();
        }

        public List<Material> LoadMaterials()
        {
            throw new NotImplementedException();
        }

        public List<Painting> LoadPaintings()
        {
            throw new NotImplementedException();
        }

        public List<PaymentMethod> LoadPaymentMethods()
        {
            throw new NotImplementedException();
        }

        public List<Post> LoadPosts()
        {
            throw new NotImplementedException();
        }

        public List<Sale> LoadSales()
        {
            throw new NotImplementedException();
        }

        public List<Sculpture> LoadSculptures()
        {
            throw new NotImplementedException();
        }

        public List<Technique> LoadTechniques()
        {
            throw new NotImplementedException();
        }

        public List<Visit> LoadVisits()
        {
            throw new NotImplementedException();
        }

        public List<VisitType> LoadVisitTypes()
        {
            throw new NotImplementedException();
        }
    }
}
