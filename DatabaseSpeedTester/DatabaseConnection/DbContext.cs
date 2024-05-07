using Microsoft.Data.SqlClient;

namespace DatabaseConnection
{
    public class DbContext
    {
        public readonly string _connectionString;
        public DbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool TestConnection()
        {
            Console.WriteLine("Testing connection to database");
            Console.WriteLine($"Connection string: {_connectionString}");
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection successful");
                    return true;
                }
                catch (SqlException e)
                {
                    Console.WriteLine("Connection failed");
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }
    }
}
