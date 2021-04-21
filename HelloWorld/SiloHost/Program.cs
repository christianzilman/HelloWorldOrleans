﻿using System;
using System.Net;
using System.Threading.Tasks;
using Grains;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Polly;

namespace SiloHost
{
    public class Program
    {

        public static async Task<int> Main(string[] args)
        {
            return await RunSilo();
        }

        private static async Task<int> RunSilo()
        {
            try
            {
                await StartSilo();

                Console.WriteLine("Silo started");

                Console.WriteLine("Press enter to terminate");
                Console.ReadLine();


                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {

            var config = LoadConfig();
            var orleansConfig = GetOrleansConfig(config);

            var builder = new SiloHostBuilder()
            //Clustering information
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "dev";
                options.ServiceId = "HelloApp";
            })
            //Clustering provider
            .UseLocalhostClustering()
            //Endpoints
            .Configure<EndpointOptions>(options =>
            {
                options.SiloPort = 11111;
                options.GatewayPort = 30000;
                options.AdvertisedIPAddress = IPAddress.Loopback;
            })
            .AddAdoNetGrainStorageAsDefault(options => 
            {
                options.Invariant = orleansConfig.Invariant;
                options.ConnectionString = orleansConfig.ConnectionString;
                options.UseJsonFormat = true;
            })
            //Implementations
            .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IHello).Assembly).WithReferences())
            .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences());                 

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }

        private static IConfigurationRoot LoadConfig()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            var config = configurationBuilder.Build();
            
            return config;
        }

        private static OrleansConfig GetOrleansConfig(IConfigurationRoot config)
        {
            var orleansConfig = new OrleansConfig();
            var section = config.GetSection("OrleansConfiguration");
            section.Bind(orleansConfig);

            return orleansConfig;
        }
    }

    public class OrleansConfig
    {
        public string Invariant { get; set; }
        public string ConnectionString { get; set; }
    }
}
