using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnection
{
    public class DDOS
    {
        private readonly DbContext _dbContext;
        private Random _random;
        public Dictionary<string, float> times = new Dictionary<string, float>();

        public DDOS(DbContext dbContext)
        {
            _dbContext = dbContext;
            _random = new Random();
        }

        public Dictionary<string, float> GetTimes()
        {
            return times;
        }

        public async Task DDOSAttack(int Users, int times)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < Users; i++)
            {
                tasks.Add(Task.Run(() => SimulateUser(times, i)));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("DDOS attack completed");
        }

        private async Task SimulateUser(int operationsCount, int User)
        {
            for (int i = 0; i < operationsCount; i++)
            {
                var operation = _random.Next(5);

                switch (operation)
                {
                    case 0:
                        await ReadData();
                        break;
                    case 1:
                        await WriteDataAsync();
                        break;
                    case 2:
                        await DeleteDataAsync();
                        break;
                    case 3:
                        await UpdateDataAsync();
                        break;
                    case 4:
                        await JoinTabels();
                        break;
                }
            }
        }

        private async Task ReadData()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (var connection = new SqlConnection(_dbContext._connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "SELECT * FROM Customers";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        await command.ExecuteReaderAsync();
                    }
                }
                stopwatch.Stop();
                times.Add("Read" + _random.Next(), stopwatch.ElapsedMilliseconds);
            }
            catch (SqlException e)
            {
                Console.WriteLine("Failed to read from database");
                Console.WriteLine(e.Message);
            }
        }

        private async Task WriteDataAsync()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (var connection = new SqlConnection(_dbContext._connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "INSERT INTO Customers (FName, LName, City, Street) VALUES (@FName, @LName, @City, @Street)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FName", "DDOS");
                        command.Parameters.AddWithValue("@LName", "Trolololol");
                        command.Parameters.AddWithValue("@City", "hahaha@ddos.com");
                        command.Parameters.AddWithValue("@Street", "somethign!w31");

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                    }
                }
                stopwatch.Stop();
                times.Add("Write" + _random.Next(), stopwatch.ElapsedMilliseconds);
            }
            catch (SqlException e)
            {
                Console.WriteLine("Failed to write data to database");
                Console.WriteLine(e.Message);
            }
        }

        private async Task DeleteDataAsync()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (var connection = new SqlConnection(_dbContext._connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "DELETE FROM Customers WHERE FName = @FName";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FName", "DDOS");

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                    }
                }
                stopwatch.Stop();
                times.Add("Delete" + _random.Next(), stopwatch.ElapsedMilliseconds);
            }
            catch (SqlException e)
            {
                Console.WriteLine("Failed to delete data from database");
                Console.WriteLine(e.Message);
            }
        }

        private async Task UpdateDataAsync()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (var connection = new SqlConnection(_dbContext._connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "UPDATE Customers SET LName = @LName WHERE FName = @FName";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@LName", "hahahahahahaha");
                        command.Parameters.AddWithValue("@FName", "DDOS");

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                    }
                }
                stopwatch.Stop();
                times.Add("Update" + _random.Next(), stopwatch.ElapsedMilliseconds);
            }
            catch (SqlException e)
            {
                Console.WriteLine("Failed to update data in database");
                Console.WriteLine(e.Message);
            }
        }

        private async Task JoinTabels()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (var connection = new SqlConnection(_dbContext._connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "SELECT * FROM Customers INNER JOIN Orders ON Customers.CustomerID = Orders.CustomerID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        await command.ExecuteReaderAsync();
                    }
                }
                stopwatch.Stop();
                times.Add("Update" + _random.Next(), stopwatch.ElapsedMilliseconds);
            }
            catch (SqlException e)
            {
                Console.WriteLine("Failed to Join data in database");
                Console.WriteLine(e.Message);
            }
        }
    }
}
