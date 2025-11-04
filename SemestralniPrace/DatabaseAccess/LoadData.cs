using DatabaseAccess.Interface;
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

        private static string connection = $"User Id={userID};Password={password};{dataSource}";
        
        public static void Accsess()
        {

            using (OracleConnection conn = new OracleConnection(connection))
            {
                conn.Open();
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
    }
}
