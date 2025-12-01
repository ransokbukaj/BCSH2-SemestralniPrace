using Entities.Account;
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
                                CurrentUser = new User
                                {
                                    Id = Convert.ToInt32(reader["iduzivatel"]),
                                    Username = reader["uzivatelskejmeno"].ToString(),
                                    FirstName = reader["jmeno"].ToString(),
                                    LastName = reader["prijmeni"].ToString(),
                                    Role = (Role)Convert.ToInt32(reader["idrole"])
                                };
                                UpdateLastLoginDate(connection, CurrentUser.Id);
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
