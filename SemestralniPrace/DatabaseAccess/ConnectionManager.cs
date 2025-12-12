using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public static class ConnectionManager
    {


        private static string _connectionString;

        public static OracleConnection Connection { get; private set; }

        //Statický konstruktor, který vystvoøí spojení s databází.
        static ConnectionManager()
        {
            //Naètení potrebných informací k pøipojeni ze souboru DatabaseConnectionConfig.txt
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatabaseConnectionConfig.txt");
            if (!File.Exists(configFilePath))
                throw new FileNotFoundException($"Configuration file for database connection has not been found! \nIt should be located at {configFilePath}");

            //Zpracování informací ze souboru
            string[] lines = File.ReadAllLines(configFilePath);
            if (lines.Length < 2)
                throw new Exception("The configuration file for database connection must contain two lines: user and password.");

            string user = lines[0].Trim();
            string password = lines[1].Trim();
            string dataSource = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=fei-sql3.upceucebny.cz)(PORT=1521)) (CONNECT_DATA=(SID=BDAS)));";

            //Vytvoøení spojovácího stringu.
            _connectionString = $"User Id={user};Password={password};Data Source={dataSource}";

            //Vytvoøení a spuštìní spojení s databází.
            Connection = new OracleConnection(_connectionString);
            Connection.Open();
        }

        /// <summary>
        /// Metoda sloužící k zavøení spojení s databází.
        /// </summary>
        public static void CloseConnection()
        {
            if (Connection != null)
            {
                Connection.Close();
                Connection.Dispose();
                Connection = null;
            }
        }
    }
}