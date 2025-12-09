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
        public List<Counter> GetFoundations()
        {
            var list = new List<Counter>();
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
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
            }
            return list;
        }

        public List<Counter> GetMaterials()
        {
            var list = new List<Counter>();
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
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
            }
            return list;
        }

        public List<Counter> GetPaymentMethods()
        {
            var list = new List<Counter>();
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
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
            }
            return list;
        }

        public List<Counter> GetRoles()
        {
            var list = new List<Counter>();
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
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
            }
            return list;
        }

        public List<Counter> GetTechniques()
        {
            var list = new List<Counter>();
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
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
            }
            return list;
        }

        public List<VisitType> GetVisitTypes()
        {
            var list = new List<VisitType>();
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
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
            }
            return list;
        }
        public List<Counter> GetExhibitionCounters()
        {
            var list = new List<Counter>();
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT id, nazev FROM v_vystavy_jako_ciselniky";
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
            }
            return list;
        }

        public List<Counter> GetArtPieceCounters()
        {
            var list = new List<Counter>();
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT id, nazev FROM v_umelecka_dila_jako_ciselniky";
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
            }
            return list;
        }
    }
}
