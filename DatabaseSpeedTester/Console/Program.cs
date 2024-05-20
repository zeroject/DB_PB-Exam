using DatabaseConnection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace DdosConsole
{
    class Program
    {
        public static IPAddress ip;
        public static int port;
        public static string username;
        public static string password;
        public static string database;
        public static DbContext dbContext;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to the Ddos Console");
            Setup();
            Console.WriteLine("\n");
            Console.WriteLine("Connecting to the Database");
            Console.WriteLine("IP: " + ip);
            Console.WriteLine("Port: " + port);
            Console.WriteLine("Username: " + username);
            Console.WriteLine("Database: " + database);
            await DatabaseConnectAsync();
            try
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start http://localhost:5000") { CreateNoWindow = true });
            } catch (Exception)
            {
                Console.WriteLine("The browser could not be opened");
            }
            DataShower.Program.Main(args);
        }

        static async Task DatabaseConnectAsync()
        {
            //$"Server={ip},{port};Database={database};User Id={username};Password={password};TrustServerCertificate=true;"
            string connectionString = $"Server={ip},{port};Database={database};User Id={username};Password={password};TrustServerCertificate=true;";
            dbContext = new DbContext(connectionString);
            bool success = dbContext.TestConnection();
            if (success)
            {
                Console.WriteLine("Connection to the database was successful");
                Console.WriteLine("Please write the amount of users you want to attack");
                int users = int.Parse(Console.ReadLine());
                Console.WriteLine("Please write the amount of times you want to attack");
                int times = int.Parse(Console.ReadLine());
                DDOS ddos = new DDOS(dbContext);
                await ddos.DDOSAttack(users, times);
                ConcurrentDictionary<string, float> concurrentDic = ddos.GetTimes();
                Dictionary<string, float> totalTimes = concurrentDic.ToDictionary(p => p.Key, p => p.Value);
                float averageRead = 0;
                float averageWrite = 0;
                float averageDelete = 0;
                float averageUpdate = 0;
                float averageJoin = 0;
                int numberOfReads = 0;
                int numberOfWrites = 0;
                int numberOfDeletes = 0;
                int numberOfUpdates = 0;
                int numberOfJoins = 0;

                foreach (var key in totalTimes.Keys)
                {
                    if (key.Contains("Read")) 
                    {
                        averageRead += totalTimes[key];
                        numberOfReads++;
                    }
                    if (key.Contains("Write"))
                    {
                        averageWrite += totalTimes[key];
                        numberOfWrites++;
                    }
                    if (key.Contains("Delete"))
                    {
                        averageDelete += totalTimes[key];
                        numberOfDeletes++;
                    }
                    if (key.Contains("Update"))
                    {
                        averageUpdate += totalTimes[key];
                        numberOfUpdates++;
                    }
                    if (key.Contains("Join"))
                    {
                        averageJoin += totalTimes[key];
                        numberOfJoins++;
                    }
                }

                // Serialize data to JSON
                var jsonData = new
                {
                    TotalTimes = totalTimes,
                    AverageRead = averageRead / numberOfReads,
                    AverageWrite = averageWrite / numberOfWrites,
                    AverageDelete = averageDelete / numberOfDeletes,
                    AverageUpdate = averageUpdate / numberOfUpdates,
                    AverageJoin = averageJoin / numberOfJoins
                    
                };
                string jsonString = JsonSerializer.Serialize(jsonData);
                string tempFile = Path.Combine("C:\\Users\\Tiger\\Downloads", "data.json");
                await File.WriteAllTextAsync(tempFile, jsonString);
            }
            else
            {
                Console.WriteLine("Connection to the database failed");
                Console.WriteLine("Please check the connection settings and try again");
                Setup();
                await DatabaseConnectAsync();
            }
        }

        static void Setup()
        {
            Console.WriteLine("Please write the IP Address for the database");
            string ipInput = Console.ReadLine();
            try
            {
                ip = IPAddress.Parse(ipInput);
            }
            catch (Exception)
            {
                Console.WriteLine("The IP Address is not valid");
                Setup();
            }
            Console.WriteLine("Please write the port for the database if empty standard port 1433 will be used");
            string portInput = Console.ReadLine();
            port = 0;
            try
            {
                port = int.Parse(portInput);
            }
            catch (Exception)
            {
                if (portInput != "")
                {
                    Console.WriteLine("The port is not valid");
                    Setup();
                }
            }
            if (port == 0)
            {
                Console.WriteLine("Standard port 1433 will be used");
                port = 1433;
            }
            Console.WriteLine("Please write the username for the database");
            username = Console.ReadLine();
            Console.WriteLine("Please write the password for the database");
            password = GetPassword();
            Console.WriteLine("\n");
            Console.WriteLine("Please write the database name");
            database = Console.ReadLine();
        }

        static string GetPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, (password.Length - 1));
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            return password;
        }
    }

    public class DataItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
