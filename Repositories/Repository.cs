using Microsoft.Data.SqlClient;
using MyProject.Interfaces;
using MyProject.Models;
using System.Data;

namespace MyProject.Repositories;

public class Repository : IRepository
{
    private readonly string _connectionString = "Server=localhost;Database=MarketsDB;Trusted_Connection=True;TrustServerCertificate=True;";
    private string GetTableName<T>()
    {
        if (typeof(T) == typeof(User)) return "Users";
        if (typeof(T) == typeof(Product)) return "Products";
        if (typeof(T) == typeof(Market)) return "Market";
        if (typeof(T) == typeof(Order)) return "Orders";
        if (typeof(T) == typeof(OrderItem)) return "OrderItems";
        if (typeof(T) == typeof(AdminSettings)) return "AdminSettings";

        throw new Exception("Table mapping not found for " + typeof(T).Name);
    }

    public IEnumerable<T> GetAll<T>() where T : class
    {
        string tableName = GetTableName<T>();

        using SqlConnection connection = new(_connectionString);
        using SqlCommand sql = new($"SELECT * FROM {tableName}", connection);
        {
            connection.Open();
            using SqlDataReader reader = sql.ExecuteReader();

            while (reader.Read())
            {
                T obj = Activator.CreateInstance<T>();

                foreach (var prop in typeof(T).GetProperties())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                        prop.SetValue(obj, reader[prop.Name]);
                }

                yield return obj;
            }
        }
    }

    public T Get<T>(Func<T, bool> predicate) where T : class, new()
    {
        return GetAll<T>().FirstOrDefault(predicate);
    }

    public void Add<T>(T entity) where T : class
    {
        string tableName = GetTableName<T>();
        var props = typeof(T).GetProperties().Where(p => p.Name != "Id");

        string columns = string.Join(",", props.Select(p => p.Name));
        string values = string.Join(",", props.Select(p => "@" + p.Name));

        using SqlConnection connection = new(_connectionString);
        using SqlCommand sql = new(
            $"INSERT INTO {tableName} ({columns}) VALUES ({values})", connection);
        {
            foreach (var prop in props)
                sql.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(entity) ?? DBNull.Value);

            connection.Open();
            sql.ExecuteNonQuery();
        }
    }

    public void Update<T>(T entity) where T : class
    {
        string tableName = GetTableName<T>();
        var props = typeof(T).GetProperties();

        var setClause = string.Join(",",
            props.Where(p => p.Name != "Id")
                 .Select(p => $"{p.Name}=@{p.Name}"));

        using SqlConnection connection = new(_connectionString);
        using SqlCommand sql = new(
            $"UPDATE {tableName} SET {setClause} WHERE Id=@Id", connection);
        {
            foreach (var prop in props)
            {
                sql.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(entity) ?? DBNull.Value);
            }
            connection.Open();
            sql.ExecuteNonQuery();
        }
    }

    public void Delete<T>(T entity) where T : class
    {
        var idProp = typeof(T).GetProperty("Id");
        if (idProp == null)
            throw new Exception("Entity must have Id");

        string tableName = GetTableName<T>();

        using SqlConnection connection = new(_connectionString);
        using SqlCommand sql = new(
            $"DELETE FROM {tableName} WHERE Id=@Id", connection);
        {
            sql.Parameters.AddWithValue("@Id", idProp.GetValue(entity));
            connection.Open();
            sql.ExecuteNonQuery();
        }
    }

    public (byte[] hash, byte[] salt) GetAdminPassword()
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        using SqlCommand sql = new SqlCommand(
            "SELECT PasswordHash, PasswordSalt FROM AdminSettings WHERE Id = 1", connection);
        {
            connection.Open();
            using SqlDataReader reader = sql.ExecuteReader();

            if (!reader.Read())
            {
                throw new Exception("Admin not initialized");
            }

            byte[] storedHash = (byte[])reader["PasswordHash"];
            byte[] storedSalt = (byte[])reader["PasswordSalt"];

            return (storedHash, storedSalt);
        }
    }

    public bool BuyProductForUser(User user, Product product, int quantity)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        using SqlCommand sql = new SqlCommand(
            "BuyProductForUser", connection);
        {
            sql.CommandType = System.Data.CommandType.StoredProcedure;

            sql.Parameters.Add("@userEmail", System.Data.SqlDbType.NVarChar, 100).Value = user.Email;
            sql.Parameters.Add("@productName", System.Data.SqlDbType.NVarChar, 100).Value = product.Name;
            sql.Parameters.Add("@quantity", System.Data.SqlDbType.Int).Value = quantity;

            connection.Open();
            sql.ExecuteNonQuery();
            return true;
        }
    }

    public bool BuyProductForMarket(Product product)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        using SqlCommand sql = new SqlCommand(
            "BuyProductForMarket", connection);
        {
            sql.CommandType = CommandType.StoredProcedure;

            sql.Parameters.Add("@productname", System.Data.SqlDbType.NVarChar, 50).Value = product.Name;
            sql.Parameters.Add("@price", System.Data.SqlDbType.Decimal).Value = product.Price;
            sql.Parameters.Add("@quantity", System.Data.SqlDbType.Int).Value = product.Quantity;

            connection.Open();
            sql.ExecuteNonQuery();
            return true;
        }
    }

    public IEnumerable<OrderDetailsDto> GetOrderDetails()
    {
        List<OrderDetailsDto> result = new List<OrderDetailsDto>();

        using SqlConnection connection = new SqlConnection(_connectionString);
        using SqlCommand sql = new SqlCommand(@"
        select p.[Name] as ProductName,
               oi.quantity as Quantity,
               oi.price as Price,
               o.totalPrice as TotalPrice,
               u.[Name] as UserName,
               u.Email,
               o.CreatedAt
        from dbo.orderitems as oi
        join dbo.products as p on oi.productId = p.Id
        join dbo.orders as o on oi.orderId = o.id
        join dbo.users as u on o.userId = u.id", connection);
        {
            connection.Open();
            using SqlDataReader reader = sql.ExecuteReader();

            while (reader.Read())
            {
                result.Add(new OrderDetailsDto
                {
                    ProductName = (string)reader["ProductName"],
                    Quantity = (int)reader["Quantity"],
                    Price = (decimal)reader["Price"],
                    TotalPrice = (decimal)reader["TotalPrice"],
                    UserName = (string)reader["UserName"],
                    Email = (string)reader["Email"],
                    CreatedAt = (DateTime)reader["CreatedAt"]
                });
            }
        }
        return result;
    }
}
