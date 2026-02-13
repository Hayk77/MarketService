//using Microsoft.Data.SqlClient;
//using MyProject.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MyProject.Repositories
//{
//    public class UserRepository
//    {
//        private readonly string _connectionString = "Server=localhost;Database=MarketsDB;Trusted_Connection=True;TrustServerCertificate=True;";

//        public bool AddUser(User user)
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand(
//                "Insert into [dbo].[Users] (Name,Balance,Email) Values(@name,@balance,@email)", connection);
//            {
//                sql.Parameters.Add("@name", System.Data.SqlDbType.NVarChar, 100).Value = user.Name;
//                sql.Parameters.Add("@balance", System.Data.SqlDbType.Decimal, 100).Value = user.Balance;
//                sql.Parameters.Add("@email", System.Data.SqlDbType.NVarChar, 100).Value = user.Email;

//                connection.Open();
//                return sql.ExecuteNonQuery() > 0;
//            }
//        }

//        public SqlDataReader GetAllEmails()
//        {
//            SqlConnection connection = new SqlConnection(_connectionString);
//            SqlCommand sql = new SqlCommand(
//                "Select Email from dbo.Users", connection);
//            {
//                connection.Open();
//                return sql.ExecuteReader();
//            }
//        }

//        public bool BuyProductForUser(User user, Product product, int quantity)
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand(
//                "BuyProductForUser", connection);
//            {
//                sql.CommandType = System.Data.CommandType.StoredProcedure;

//                sql.Parameters.Add("@userEmail", System.Data.SqlDbType.NVarChar, 100).Value = user.Email;
//                sql.Parameters.Add("@productName", System.Data.SqlDbType.NVarChar, 100).Value = product.Name;
//                sql.Parameters.Add("@quantity", System.Data.SqlDbType.Int).Value = quantity;

//                connection.Open();
//                sql.ExecuteNonQuery();
//                return true;
//            }
//        }

//        public bool AddUserBalance(decimal newPrice, User user)
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand(
//                "Update dbo.Users Set Balance += @newPrice where Email = @email ", connection);
//            {
//                sql.Parameters.Add("@newPrice", System.Data.SqlDbType.Decimal).Value = newPrice;
//                sql.Parameters.Add("@email", System.Data.SqlDbType.NVarChar, 50).Value = user.Email;

//                connection.Open();
//                sql.ExecuteNonQuery();
//                return true;
//            }
//        }

//        public decimal GetUserBalance(User user)
//        {
//            using SqlConnection connection = new SqlConnection(_connectionString);
//            using SqlCommand sql = new SqlCommand(
//                "Select Balance from dbo.Users where Email = @email", connection);
//            {
//                sql.Parameters.Add("email", System.Data.SqlDbType.NVarChar, 50).Value = user.Email;

//                connection.Open();
//                decimal result = (decimal)sql.ExecuteScalar();
//                return result;
//            }
//        }
//    }
//}

