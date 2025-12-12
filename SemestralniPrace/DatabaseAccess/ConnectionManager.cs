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

        //Statick� konstruktor, kter� vystvo�� spojen� s datab�z�.
        static ConnectionManager()
        {
            //Na�ten� potrebn�ch informac� k p�ipojeni ze souboru DatabaseConnectionConfig.txt
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatabaseConnectionConfig.txt");
            if (!File.Exists(configFilePath))
                throw new FileNotFoundException($"Configuration file for database connection has not been found! \nIt should be located at {configFilePath}");

            //Zpracov�n� informac� ze souboru
            string[] lines = File.ReadAllLines(configFilePath);
            if (lines.Length < 2)
                throw new Exception("The configuration file for database connection must contain two lines: user and password.");

            string user = lines[0].Trim();
            string password = lines[1].Trim();
            string dataSource = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=fei-sql3.upceucebny.cz)(PORT=1521)) (CONNECT_DATA=(SID=BDAS)));";

            //Vytvo�en� spojov�c�ho stringu.
            _connectionString = $"User Id={user};Password={password};Data Source={dataSource}";

            //Vytvo�en� a spu�t�n� spojen� s datab�z�.
            Connection = new OracleConnection(_connectionString);
            Connection.Open();
        }

        /// <summary>
        /// Metoda slou��c� k zav�en� spojen� s datab�z�.
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