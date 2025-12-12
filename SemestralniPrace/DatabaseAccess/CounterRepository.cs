using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities;

namespace DatabaseAccess
{
    public class CounterRepository : ICounterRepository
    {
        /// <summary>
        /// Metoda pro zisk číselníku všech podkladu pro malba
        /// </summary>
        /// <returns>List číselníku s podklady pro malbu.</returns>
        public List<Counter> GetFoundations()
        {
            var list = new List<Counter>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "SELECT id, nazev FROM v_podklady";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Counter
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["nazev"].ToString()
                        });
                    }
                }
            }            
            return list;
        }

        /// <summary>
        /// Metoda pro zisk číselníku všech materiálů pro sochu
        /// </summary>
        /// <returns>List číselníku s materiály pro sochu.</returns>
        public List<Counter> GetMaterials()
        {
            var list = new List<Counter>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "SELECT id, nazev FROM v_materialy";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Counter
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["nazev"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda pro získání všech číselníku s metodami platby.
        /// </summary>
        /// <returns>List číselníku s metodami platby.</returns>
        public List<Counter> GetPaymentMethods()
        {
            var list = new List<Counter>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "SELECT id, nazev FROM v_druhy_plateb";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Counter
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["nazev"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda pro zisk všech rolí.
        /// </summary>
        /// <returns>List číselníků se všemy rolemi.</returns>
        public List<Counter> GetRoles()
        {
            var list = new List<Counter>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "SELECT id, nazev FROM v_role";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Counter
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["nazev"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda pro zisk číselníků všech technik malby
        /// </summary>
        /// <returns>List číselníku technik malby.</returns>
        public List<Counter> GetTechniques()
        {
            var list = new List<Counter>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "SELECT id, nazev FROM v_techniky";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Counter
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["nazev"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Metoda pro zisk všech číselníku druhů návštěv.
        /// </summary>
        /// <returns>List číselníku druhů návštěv.</returns>
        public List<VisitType> GetVisitTypes()
        {
            var list = new List<VisitType>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = "SELECT id, nazev, cena FROM v_druhy_navstev";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new VisitType
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Name = reader["nazev"].ToString(),
                            Price = Convert.ToDecimal(reader["cena"])
                        });
                    }
                }
            }
            return list;
        }
    }
}
