using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    internal static class ConnectionManager
    {
        private static string _connectionString;

        static ConnectionManager()
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatabaseConnectionConfig.txt");
            if (!File.Exists(configFilePath))
                throw new FileNotFoundException($"Configuration file for database connection has not been found! \nIt should be located at {configFilePath}");

            string[] lines = File.ReadAllLines(configFilePath);
            if (lines.Length < 2)
                throw new Exception("The configuration file for database connection must contain two lines: user and password.");

            string user = lines[0].Trim();
            string password = lines[1].Trim();
            string dataSource = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=fei-sql3.upceucebny.cz)(PORT=1521)) (CONNECT_DATA=(SID=BDAS)));";

            _connectionString = $"User Id={user};Password={password};Data Source={dataSource}";
        }

        public static OracleConnection GetConnection()
        {
            return new OracleConnection(_connectionString);
        }
    }
}
