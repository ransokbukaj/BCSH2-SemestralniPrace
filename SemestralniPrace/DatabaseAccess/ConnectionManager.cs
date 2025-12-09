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
        private static OracleConnection _connection;
        private static readonly object _lock = new object();
        private static int? _currentUserId = null;

        public static OracleConnection Connection
        {
            get
            {
                lock (_lock)
                {
                    // Pokud připojení neexistuje nebo je uzavřené, vytvoř nové
                    if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
                    {
                        // Dispose starého připojení, pokud existuje
                        if (_connection != null)
                        {
                            try
                            {
                                _connection.Dispose();
                            }
                            catch { }
                        }

                        // Vytvoř nové připojení
                        _connection = new OracleConnection(_connectionString);
                        _connection.Open();

                        // Obnov session identifikátor, pokud byl nastaven
                        if (_currentUserId.HasValue)
                        {
                            SetDatabaseSessionIdentifier(_connection, _currentUserId.Value);
                        }
                    }

                    return _connection;
                }
            }
        }

        public static void SetCurrentUser(int userId)
        {
            lock (_lock)
            {
                _currentUserId = userId;

                // Nastav session identifikátor pro aktuální připojení
                if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
                {
                    SetDatabaseSessionIdentifier(_connection, userId);
                }
            }
        }

        public static void ClearCurrentUser()
        {
            lock (_lock)
            {
                _currentUserId = null;

                // Vyčisti session identifikátor z databáze
                if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
                {
                    ClearDatabaseSessionIdentifier(_connection);
                }
            }
        }

        private static void SetDatabaseSessionIdentifier(System.Data.IDbConnection connection, int userId)
        {
            string query = @"
                BEGIN
                    DBMS_SESSION.SET_IDENTIFIER(:userId);
                END;";

            using (var command = connection.CreateCommand())
            {
                var param = command.CreateParameter();
                param.ParameterName = ":userId";
                param.Value = userId.ToString();

                command.CommandText = query;
                command.Parameters.Add(param);
                command.ExecuteNonQuery();
            }
        }

        private static void ClearDatabaseSessionIdentifier(System.Data.IDbConnection connection)
        {
            string query = @"
                BEGIN
                    DBMS_SESSION.CLEAR_IDENTIFIER();
                END;";

            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }

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

            // Inicializuj připojení
            _connection = new OracleConnection(_connectionString);
            _connection.Open();
        }

        public static void CloseConnection()
        {
            lock (_lock)
            {
                if (_connection != null)
                {
                    try
                    {
                        _connection.Close();
                        _connection.Dispose();
                    }
                    catch { }
                    finally
                    {
                        _connection = null;
                    }
                }
            }
        }
    }
}