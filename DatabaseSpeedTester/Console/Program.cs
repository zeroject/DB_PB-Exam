using System;
using System.Net;

namespace DdosConsole
{
    class Program
    {
        public static IPAddress ip;
        public static int port;
        public static string username;
        public static string password;
        public static string database;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Ddos Console");
            Setup();
            Console.WriteLine("\n");
            Console.WriteLine("Connecting to the Database");
            Console.WriteLine("IP: " + ip);
            Console.WriteLine("Port: " + port);
            Console.WriteLine("Username: " + username);
            Console.WriteLine("Database: " + database);
        }

        static void Setup()
        {
            Console.WriteLine("Please write the IP Adress for the database");
            string ipInput = Console.ReadLine();
            try
            {
                ip = IPAddress.Parse(ipInput);
            } catch (Exception e)
            {
                Console.WriteLine("The IP Adress is not valid");
                Setup();
            }
            Console.WriteLine("Please write the port for the database if empty standart port 1433 will be used");
            string portInput = Console.ReadLine();
            port = 0;
            try
            {
                port = int.Parse(portInput);
            } catch (Exception e)
            {
                if (portInput != "")
                {
                    Console.WriteLine("The port is not valid");
                    Setup();
                }
            }
            if (port == 0)
            {
                Console.WriteLine("Standart port 1433 will be used");
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
}