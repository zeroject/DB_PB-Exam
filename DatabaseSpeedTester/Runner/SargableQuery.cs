using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Runner;

public static class SargableQuery
{
    public static DbContext _dbContext;
    private static readonly Random _random;

    public static ConcurrentDictionary<string, float> times = new ConcurrentDictionary<string, float>();

    static SargableQuery()
    {
        _random = new Random();
    }

    public static ConcurrentDictionary<string, float> GetTimes()
    {
        return times;
    }

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
                    SELECT od.OrderId, i.ItemName, od.Quantity, i.ItemPrice
                    FROM OrderDetails od
                    JOIN Items i ON od.ItemId = i.ItemId
                    WHERE od.OrderId IN (
                        SELECT o.Id FROM Orders o WHERE o.CustomerId = 1
                    );
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
                string sql = @"
                    -- Insert a new customer
                    INSERT INTO Customers (FName, LName, City, Street) VALUES ('John', 'Doe', 'New York', '5th Avenue');

                    -- Get the ID of the new customer
                    DECLARE @newCustomerId INT = (SELECT MAX(Id) FROM Customers);

                    -- Insert a new order for the new customer
                    INSERT INTO Orders (CustomerId, OrderDate, OrderCost) VALUES (@newCustomerId, GETDATE(), 0);

                    -- Get the ID of the new order
                    DECLARE @newOrderId INT = (SELECT MAX(Id) FROM Orders);

                    -- Insert order details for Milk and Bread
                    INSERT INTO OrderDetails (OrderId, ItemId, Quantity)
                    SELECT @newOrderId, ItemId, 1
                    FROM Items
                    WHERE ItemName IN ('Milk', 'Bread');

                    -- Update the order cost
                    UPDATE Orders SET OrderCost = (
                        SELECT SUM(i.ItemPrice * od.Quantity)
                        FROM OrderDetails od
                        JOIN Items i ON od.ItemId = i.ItemId
                        WHERE od.OrderId = @newOrderId
                    ) WHERE Id = @newOrderId;
                ";

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
                string sql = @"
                    -- Update the price of an item
                    UPDATE Items SET ItemPrice = 1.20 WHERE ItemName = 'Milk';

                    -- Update the order cost for all orders that contain the item
                    UPDATE Orders SET OrderCost = (
                        SELECT SUM(i.ItemPrice * od.Quantity)
                        FROM OrderDetails od
                        JOIN Items i ON od.ItemId = i.ItemId
                        WHERE od.OrderId = OrderId
                    ) WHERE EXISTS (
                        SELECT * FROM OrderDetails od WHERE od.OrderId = OrderId AND od.ItemId = (
                            SELECT ItemId FROM Items WHERE ItemName = 'Milk'
                        )
                    );
                ";

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
                string sql = @"
                    -- Delete the order details of the customers with first name 'John'
                    DELETE FROM OrderDetails WHERE OrderId IN (
                        SELECT Id FROM Orders WHERE CustomerId IN (
                            SELECT Id FROM Customers WHERE FName = 'John'
                        )
                    );
                    
                    -- Delete the orders of the customers with first name 'John'
                    DELETE FROM Orders WHERE CustomerId IN (
                        SELECT Id FROM Customers WHERE FName = 'John'
                    );
                    
                    -- Delete the customers with first name 'John'
                    DELETE FROM Customers WHERE FName = 'John';
                ";

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
