﻿using DbUp;
using System;
using System.Linq;
using System.Reflection;

namespace DbMigration
{
    class Program
    {
        static int Main(string[] args)
        {
            var connectionString =
       args.FirstOrDefault()
       ?? "Server=localhost;Uid=root;Pwd=123456;Persist Security Info= true;Database=orleanshelloworld;SslMode=none;";

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
