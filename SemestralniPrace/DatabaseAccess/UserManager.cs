using Entities;
using Entities.Data;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public static class UserManager
    {
        public static User? CurrentUser;

        static UserManager()
        {
            CurrentUser = null;
        }

        public static bool Register(string username, string password, string firstName, string Lastname, string email, string phoneNumber)
        {
            return true;
        }

        public static bool LogIn(string username, string password)
        {
            using (var connection = ConnectionManager.Connection)
            {
                string query = @"
                    SELECT
                        iduzivatel, uzivatelskejmeno, heslohash, jmeno, prijmeni, idrole 
                    FROM
                        uzivatele 
                    WHERE
                        uzivatelskejmeno = :username";

                using (var command = connection.CreateCommand())
                {
                    var param = command.CreateParameter();
                    param.ParameterName = ":username";
                    param.Value = username;

                    command.CommandText = query;
                    command.Parameters.Add(param);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader["heslohash"].ToString();

                            if (VerifyPassword(password, storedHash))
                            {
                                int userId = Convert.ToInt32(reader["iduzivatel"]);

                                CurrentUser = new User
                                {
                                    Id = userId,
                                    Username = reader["uzivatelskejmeno"].ToString(),
                                    FirstName = reader["jmeno"].ToString(),
                                    LastName = reader["prijmeni"].ToString(),
                                    Role = (Role)Convert.ToInt32(reader["idrole"])
                                };

                                SetDatabaseSessionIdentifier(connection, userId);
                                UpdateLastLoginDate(connection, userId);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        public static void LogOut()
        {
            ClearDatabaseSessionIdentifier(ConnectionManager.Connection);
            CurrentUser = null;
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
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

        private static void UpdateLastLoginDate(System.Data.IDbConnection connection, int userId)
        {
            string query = @"
                UPDATE uzivatele
                SET datumposlednihoprihlaseni = SYSDATE 
                WHERE iduzivatel = :userId";

            using (var command = connection.CreateCommand())
            {
                var param = command.CreateParameter();
                param.ParameterName = ":userId";
                param.Value = userId;

                command.CommandText = query;
                command.Parameters.Add(param);
                command.ExecuteNonQuery();
            }
        }
    }
}