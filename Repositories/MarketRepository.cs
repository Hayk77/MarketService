//using MyProject.Interfaces;
//using MyProject.Models;
//using Microsoft.Data.SqlClient;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MyProject.Repositories
//{
//    public class MarketRepository : IRepository
//    {
//        private readonly string _connectionString = "Server=localhost;Database=MarketsDB;Trusted_Connection=True;TrustServerCertificate=True;";

//        public (byte[] hash, byte[] salt) GetAdminPassword()
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand(
//                "SELECT PasswordHash, PasswordSalt FROM AdminSettings WHERE Id = 1", connection);
//            {
//                connection.Open();
//                using SqlDataReader reader = sql.ExecuteReader();

//                if (!reader.Read())
//                {
//                    throw new Exception("Admin not initialized");
//                }

//                byte[] storedHash = (byte[])reader["PasswordHash"];
//                byte[] storedSalt = (byte[])reader["PasswordSalt"];

//                return (storedHash, storedSalt);
//            }
//        }

//        public string GetMarketName()
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand(
//                "Select name from dbo.Market where id = 1", connection);
//            {
//                connection.Open();
//                string name = (string)sql.ExecuteScalar();
//                return name;
//            }
//        }



//        public decimal GetMarketBalance()
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand(
//                "Select Balance from Market where id = 1", connection);
//            {
//                connection.Open();
//                decimal balance = (decimal)sql.ExecuteScalar();
//                return balance;
//            }
//        }

//        public int GetAllProductsQuantity()
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand(
//                "Select Sum(Quantity) from dbo.Products", connection);
//            {
//                connection.Open();
//                int result = (int)sql.ExecuteScalar();
//                return result;
//            }
//        }

//        public decimal GetProductsSellIncome()
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand(
//                "Select Sum(TotalPrice) from dbo.Orders", connection);
//            {
//                connection.Open();
//                decimal income = (decimal)sql.ExecuteScalar();
//                return income;
//            }
//        }

//        public IEnumerable<Product> GetAllProducts()
//        {
//            List<Product> products = new List<Product>();

//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand(
//                "Select * from dbo.Products Where IsActive = 1", connection);
//            {
//                connection.Open();
//                SqlDataReader reader = sql.ExecuteReader();

//                while (reader.Read())
//                {
//                    products.Add(new Product()
//                    {
//                        Name = (string)reader["Name"],
//                        Price = (decimal)reader["Price"],
//                        Quantity = (int)reader["Quantity"]
//                    });
//                }
//                return products;
//            }
//        }

//        public bool UpdateProductPrice(string productName, decimal newPrice)
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand(
//                "Update dbo.Products Set price = @newPrice Where Name = @name", connection);
//            {
//                sql.Parameters.Add("@newPrice", System.Data.SqlDbType.Decimal).Value = newPrice;
//                sql.Parameters.Add("@name", System.Data.SqlDbType.NVarChar).Value = productName;

//                connection.Open();
//                return sql.ExecuteNonQuery() > 0;
//            }
//        }

//        public bool DeleteProduct(string productName)
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand(
//                "Update dbo.Products Set IsActive = 0 Where Name = @productName", connection);
//            {
//                sql.Parameters.Add("@productName", System.Data.SqlDbType.NVarChar).Value = productName;

//                connection.Open();
//                return sql.ExecuteNonQuery() > 0;
//            }
//        }
//        public bool BuyProductForMarket(Product product)
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand(
//                "BuyProductForMarket", connection);
//            {
//                sql.CommandType = CommandType.StoredProcedure;

//                sql.Parameters.Add("@productname", System.Data.SqlDbType.NVarChar, 50).Value = product.Name;
//                sql.Parameters.Add("@price", System.Data.SqlDbType.Decimal).Value = product.Price;
//                sql.Parameters.Add("@quantity", System.Data.SqlDbType.Int).Value = product.Quantity;

//                connection.Open();
//                sql.ExecuteNonQuery();
//                return true;
//            }
//        }

//        public IEnumerable<T> GetAll<T>() where T : class
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand("", connection); 
//            {
//                connection.Open();

//                if (typeof(T) == typeof(Product))
//                {
//                    sql.CommandText = "SELECT * FROM dbo.Products";
//                    List<Product> products = new List<Product>();
//                    using SqlDataReader reader = sql.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        products.Add(new Product()
//                        {
//                            Name = (string)reader["Name"],
//                            Price = (decimal)reader["Price"],
//                            Quantity = (int)reader["Quantity"],
//                            IsActive = (bool)reader["IsActive"]
//                        });
//                    }
//                    return products as IEnumerable<T>;
//                }
//                else if (typeof(T) == typeof(Market))
//                {
//                    sql.CommandText = "SELECT * FROM dbo.Market";
//                    List<Market> markets = new List<Market>();
//                    using SqlDataReader reader = sql.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        markets.Add(new Market()
//                        {
//                            Id = (int)reader["Id"],
//                            Name = (string)reader["Name"],
//                            Balance = (decimal)reader["Balance"]
//                        });
//                    }
//                    return markets as IEnumerable<T>;
//                }
//                else if (typeof(T) == typeof(Order))
//                {
//                    sql.CommandText = "SELECT * FROM dbo.Orders";
//                    List<Order> orders = new List<Order>();
//                    using SqlDataReader reader = sql.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        orders.Add(new Order()
//                        {
//                            Id = (int)reader["Id"],
//                            TotalPrice = (decimal)reader["TotalPrice"]
//                        });
//                    }
//                    return orders as IEnumerable<T>;
//                }
//                else
//                {
//                    return new List<T>();
//                }
//            }
//        }

//        public void Add<T>(T entity) where T : class
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand("", connection);
//            {
//                connection.Open();

//                if (entity is Product product)
//                {
//                    sql.CommandText = "INSERT INTO dbo.Products (Name, Price, Quantity, IsActive) VALUES (@name, @price, @quantity, @isActive)";
//                    sql.Parameters.AddWithValue("@name", product.Name);
//                    sql.Parameters.AddWithValue("@price", product.Price);
//                    sql.Parameters.AddWithValue("@quantity", product.Quantity);
//                    sql.Parameters.AddWithValue("@isActive", product.IsActive);
//                    sql.ExecuteNonQuery();
//                }
//                else if (entity is Market market)
//                {
//                    sql.CommandText = "UPDATE dbo.Market SET Name=@name, Balance=@balance WHERE Id=@id";
//                    sql.Parameters.AddWithValue("@name", market.Name);
//                    sql.Parameters.AddWithValue("@balance", market.Balance);
//                    sql.Parameters.AddWithValue("@id", market.Id);
//                    sql.ExecuteNonQuery();
//                }
//            }
//        }

//        public void Update<T>(T entity) where T : class
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand("", connection);
//            {
//                connection.Open();

//                if (entity is Product product)
//                {
//                    sql.CommandText = "UPDATE dbo.Products SET Price=@price, Quantity=@quantity, IsActive=@isActive WHERE Name=@name";
//                    sql.Parameters.AddWithValue("@name", product.Name);
//                    sql.Parameters.AddWithValue("@price", product.Price);
//                    sql.Parameters.AddWithValue("@quantity", product.Quantity);
//                    sql.Parameters.AddWithValue("@isActive", product.IsActive);
//                    sql.ExecuteNonQuery();
//                }
//                else if (entity is Market market)
//                {
//                    sql.CommandText = "UPDATE dbo.Market SET Name=@name, Balance=@balance WHERE Id=@id";
//                    sql.Parameters.AddWithValue("@name", market.Name);
//                    sql.Parameters.AddWithValue("@balance", market.Balance);
//                    sql.Parameters.AddWithValue("@id", market.Id);
//                    sql.ExecuteNonQuery();
//                }
//            }
//        }

//        public T Get<T>(Func<T, bool> predicate) where T : class, new()
//        {
//            List<T> list = new List<T>();
//            string tableName = typeof(T).Name; 
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand($"SELECT * FROM {tableName}", connection);
//            {
//                connection.Open();
//                SqlDataReader reader = sql.ExecuteReader();
//                while (reader.Read())
//                {
//                    T obj = new T();
//                    foreach (var prop in typeof(T).GetProperties())
//                    {
//                        if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
//                            prop.SetValue(obj, reader[prop.Name]);
//                    }
//                    list.Add(obj);
//                }
//            }
//            return list.FirstOrDefault(predicate);
//        }

//        public void Delete<T>(T entity) where T : class
//        {
//            string tableName = typeof(T).Name;
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand("", connection);
//            {
//                connection.Open();

//                var idProp = typeof(T).GetProperty("Id");
//                if (idProp == null)
//                    throw new Exception("Entity must have an Id property for Delete operation.");

//                var idValue = idProp.GetValue(entity);
//                sql.CommandText = $"DELETE FROM {tableName} WHERE Id = @id";
//                sql.Parameters.AddWithValue("@id", idValue);

//                sql.ExecuteNonQuery();
//            }
//        }

//        public bool BuyProductForUser(User user, Product product, int quantity)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
