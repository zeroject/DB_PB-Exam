using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
                using (var connection = new SqlConnection(_dbContext._connectionString))
                {
                    await connection.OpenAsync();
                    tasks.Add(Task.Run(() => SimulateUser(times, i, connection)));
                }
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("DDOS attack completed");
        }

        private async Task SimulateUser(int operationsCount, int User, SqlConnection connection)
        {
            for (int i = 0; i < operationsCount; i++)
            {
                var operation = _random.Next(4); // 0: read, 1: write, 2: update, 3: delete

                switch (operation)
                {
                    case 0:
                        await ReadData(connection);
                        break;
                    case 1:
                        await WriteDataAsync(connection);
                        break;
                    case 2:
                        await DeleteDataAsync(connection);
                        break;
                    case 3:
                        await UpdateDataAsync(connection);
                        break;
                }
            }
        }

        private async Task ReadData(SqlConnection connection)
        {
            try
            {
                string sql = "SELECT * FROM Users";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    await command.ExecuteReaderAsync();
                    stopwatch.Stop();
                    times.Add("Read" + _random.Next(), stopwatch.ElapsedMilliseconds);
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Failed to read from database");
                Console.WriteLine(e.Message);
            }
        }

        private async Task WriteDataAsync(SqlConnection connection)
        {
            try
            {
                string sql = "INSERT INTO Users (FirstName, LastName, Email, Password) VALUES (@FirstName, @LastName, @Email, @Password)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", "DDOS");
                    command.Parameters.AddWithValue("@LastName", "Trolololol");
                    command.Parameters.AddWithValue("@Email", "hahaha@ddos.com");
                    command.Parameters.AddWithValue("@Password", "somethign!w31");

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    stopwatch.Stop();
                    times.Add("Write" + _random.Next(), stopwatch.ElapsedMilliseconds);
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Failed to write data to database");
                Console.WriteLine(e.Message);
            }
        }

        private async Task DeleteDataAsync(SqlConnection connection)
        {
            try
            {
                string sql = "DELETE FROM Users WHERE FirstName = @FirstName";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", "DDOS");

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    stopwatch.Stop();
                    times.Add("Delete" + _random.Next(), stopwatch.ElapsedMilliseconds);
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Failed to delete data from database");
                Console.WriteLine(e.Message);
            }
        }

        private async Task UpdateDataAsync(SqlConnection connection)
        {
            try
            {
                string sql = "UPDATE Users SET LastName = @LastName WHERE FirstName = @FirstName";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@LastName", "hahahahahahaha");
                    command.Parameters.AddWithValue("@FirstName", "DDOS");

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    stopwatch.Stop();
                    times.Add("Update" + _random.Next(), stopwatch.ElapsedMilliseconds);
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Failed to update data in database");
                Console.WriteLine(e.Message);
            }
        }
    }
}
