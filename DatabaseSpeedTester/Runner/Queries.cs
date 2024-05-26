using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Runner;

public static class Queries
{
    public static DbContext _dbContext;
    private static readonly Random _random;

    public static ConcurrentDictionary<string, float> times = new ConcurrentDictionary<string, float>();

    static Queries()
    {
        _random = new Random();
    }

    public static ConcurrentDictionary<string, float> GetTimes()
    {
        return times;
    }

    //Non-SargableQueries

    public static async Task GetAllFromSpecificStreetNonSargable()
    {
        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var connection = new SqlConnection(_dbContext._connectionString))
            {
                await connection.OpenAsync();
                string sql = "SELECT * FROM Customers  WHERE LOWER(Street) = LOWER('Minegade');";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    await command.ExecuteReaderAsync();
                }
            }
            stopwatch.Stop();
            times.TryAdd("GetAllFromSpecificStreetNonSargable" + _random.Next(), stopwatch.ElapsedMilliseconds);
        }
        catch (SqlException e)
        {
            Console.WriteLine("Failed to read from database");
            Console.WriteLine(e.Message);
        }
    }

    public static async Task GetOrderDetailsFromCustomerNonSargable()
    {
        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var connection = new SqlConnection(_dbContext._connectionString))
            {
                await connection.OpenAsync();
                string sql = @"
                    BEGIN TRANSACTION;

                    SELECT od.OrderId, i.ItemName, od.Quantity, i.ItemPrice
                    FROM OrderDetails od 
                    JOIN Items i ON od.ItemId = i.ItemId
                    WHERE od.OrderId IN (
                    SELECT o.Id FROM Orders o  WHERE o.CustomerId = 2 AND o.OrderDate >= DATEADD(day, -30, GETDATE())
                    );

                    COMMIT TRANSACTION;
                ";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    await command.ExecuteReaderAsync();
                }
            }
            stopwatch.Stop();
            times.TryAdd("GetOrderDetailsFromCustomerNonSargable" + _random.Next(), stopwatch.ElapsedMilliseconds);
        }
        catch (SqlException e)
        {
            Console.WriteLine("Failed to read from database");
            Console.WriteLine(e.Message);
        }
    }

    //SargableQueries

    public static async Task GetAllFromSpecificStreet()
    {
        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var connection = new SqlConnection(_dbContext._connectionString))
            {
                await connection.OpenAsync();
                string sql = "SELECT * FROM Customers WHERE Street = 'Fjerkræevej';";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    await command.ExecuteReaderAsync();
                }
            }
            stopwatch.Stop();
            times.TryAdd("GetAllFromSpecificStreet" + _random.Next(), stopwatch.ElapsedMilliseconds);
        }
        catch (SqlException e)
        {
            Console.WriteLine("Failed to read from database");
            Console.WriteLine(e.Message);
        }
    }

    public static async Task GetOrderDetailsFromCustomer()
    {
        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var connection = new SqlConnection(_dbContext._connectionString))
            {
                await connection.OpenAsync();
                string sql = @"
                    BEGIN TRANSACTION;

                    SELECT od.OrderId, i.ItemName, od.Quantity, i.ItemPrice
                    FROM OrderDetails od
                    JOIN Items i ON od.ItemId = i.ItemId
                    WHERE od.OrderId IN (
                        SELECT o.Id FROM Orders o WHERE o.CustomerId = 1
                    );

                    COMMIT TRANSACTION;
                ";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    await command.ExecuteReaderAsync();
                }
            }
            stopwatch.Stop();
            times.TryAdd("GetOrderDetailsFromCustomer" + _random.Next(), stopwatch.ElapsedMilliseconds);
        }
        catch (SqlException e)
        {
            Console.WriteLine("Failed to read from database");
            Console.WriteLine(e.Message);
        }
    }

    public static async Task CreateNewCustomerWithOrderOfMilkAndBread()
    {
        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var connection = new SqlConnection(_dbContext._connectionString))
            {
                await connection.OpenAsync();
                string sql = "INSERT INTO Customers (FName, LName, City, Street) VALUES ('Andrew', 'Doe', 'Odense', 'Bægeralle');";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                }
            }
            stopwatch.Stop();
            times.TryAdd("CreateNewCustomerWithOrderOfMilkAndBread" + _random.Next(), stopwatch.ElapsedMilliseconds);
        }
        catch (SqlException e)
        {
            Console.WriteLine("Failed to write data to database");
            Console.WriteLine(e.Message);
        }
    }

    public static async Task UpdatePriceOfItem()
    {
        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var connection = new SqlConnection(_dbContext._connectionString))
            {
                await connection.OpenAsync();
                string sql = "UPDATE Items SET ItemPrice = 1.20 WHERE ItemName = 'Milk';";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                }
            }
            stopwatch.Stop();
            times.TryAdd("UpdatePriceOfItem" + _random.Next(), stopwatch.ElapsedMilliseconds);
        }
        catch (SqlException e)
        {
            Console.WriteLine("Failed to update data in database");
            Console.WriteLine(e.Message);
        }
    }

    public static async Task DeleteLastInsertCustomer()
    {
        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var connection = new SqlConnection(_dbContext._connectionString))
            {
                await connection.OpenAsync();
                string sql = "DELETE FROM Customers WHERE Fname = 'Andrew' And LName = 'Doe';";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                }
            }
            stopwatch.Stop();
            times.TryAdd("DeleteLastInsertCustomer" + _random.Next(), stopwatch.ElapsedMilliseconds);
        }
        catch (SqlException e)
        {
            Console.WriteLine("Failed to delete data from database");
            Console.WriteLine(e.Message);
        }
    }
}
