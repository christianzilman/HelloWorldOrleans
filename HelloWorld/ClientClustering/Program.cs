using System;
using System.Threading.Tasks;
using Interfaces;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Microsoft.Extensions.Logging;
using Polly;

namespace ClientClustering
{
    class Program
    {
        static void Main(string[] args)
        {
            var v = RunMainAsync();
            Console.ReadKey();
        }

        //static async Task<int> RunMainAsync()
        static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = StartClient())
                {
                    //Guid helloId = new Guid("{2349992C-860A-4EDA-9590-000000000006}");
                    //var grain = client.GetGrain<IHello>(helloId);
                    RequestContext.Set("traceId", Guid.NewGuid());

                    //var grain = client.GetGrain<IHello>(0);
                    //var response = grain.SayHello("Good morning");

                    //RequestContext.Set("traceId", Guid.NewGuid());
                    //var grain1 = client.GetGrain<IHello>(1);                   
                    //var response1 = grain1.SayHello("Good afternoon");


                    //var response2 = grain1.SayHello("Good midday");


                    //var r = response.Result;
                    //Console.WriteLine(response.Result);


                    //var grain = client.GetGrain<IGreetingsGrain>(0);
                    //await grain.SendGreetings("Hello 2");

                    //var grain1 = client.GetGrain<IGreetingsGrain>(0);
                    //await grain1.SendGreetings("Morning 2");

                    //var grain2 = client.GetGrain<IGreetingsGrain>(0);
                    //await grain1.SendGreetings("Afternoon 2");

                    int reps = 60;
                    int counter = 0;

                    do
                    {
                        var grain = client.GetGrain<IHello>(counter);
                        var response = grain.SayHello("Good morning");

                        counter++;                        

                    } while (counter < reps);


                    Console.WriteLine($"Client is initialized: {client.IsInitialized}");
                }

                return 0;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                return -1;
            }
        }


        static IClusterClient StartClient()
        {

            return Policy<IClusterClient>
                .Handle<Exception>()
                .Or<OrleansMessageRejectionException>()
                .WaitAndRetry(2, retryAttempt =>
                {
                    Console.WriteLine($"Attempt {retryAttempt}. Waiting 10 seconds");
                    return TimeSpan.FromSeconds(10); // Wait 10 seconds
                })
                .Execute(() =>
                {
                    var client = new ClientBuilder()
                    // Clustering information 
                    .Configure<ClusterOptions>(options =>
                    {

                        options.ClusterId = "dev";
                        options.ServiceId = "HelloApp";

                    })
                    // Clustering provider
                    .UseAdoNetClustering(options => {
                        options.Invariant = "MySql.Data.MySqlClient";
                        options.ConnectionString = "Server=localhost;Uid=root;Pwd=123456;Persist Security Info= true;Database=orleanshelloworld;SslMode=none;";
                    })
                    .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IHello).Assembly))
                    //.ConfigureLogging(logging => logging.AddConsole())
                    .Build();

                    client.Connect().GetAwaiter().GetResult();

                    Console.WriteLine("Client connected");

                    return client;
                });
        }
        //static async Task<IClusterClient> StartClient()
        //{
        //    var client = new ClientBuilder()

        //        // Clustering information 
        //        .Configure<ClusterOptions>(options =>
        //        {

        //            options.ClusterId = "dev";
        //            options.ServiceId = "HelloApp";

        //        })

        //         // Clustering provider
        //         .UseLocalhostClustering()
        //         .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IHello).Assembly))
        //         .Build();

        //    await client.Connect();
        //    Console.WriteLine("Client connected");

        //    return client;
        //}
    }
}

