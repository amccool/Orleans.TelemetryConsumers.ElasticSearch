using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Orleans;

namespace TestHost
{

    class Program
    {

        static readonly ManualResetEvent _siloStopped = new ManualResetEvent(false);

        static ISiloHost silo;
        static bool siloStopping = false;
        static readonly object syncLock = new object();

        static void Main(string[] args)
        {

            SetupApplicationShutdown();

            silo = CreateSilo();
            silo.StartAsync().Wait();

            // Wait for the silo to completely shutdown before exiting. 
            _siloStopped.WaitOne();
        }

        static void SetupApplicationShutdown()
        {
            // Capture the user pressing Ctrl+C
            Console.CancelKeyPress += (s, a) => {
                // Prevent the application from crashing ungracefully.
                a.Cancel = true;
                // Don't allow the following code to repeat if the user presses Ctrl+C repeatedly.
                lock (syncLock)
                {
                    if (!siloStopping)
                    {
                        siloStopping = true;
                        Task.Run(StopSilo).Ignore();
                    }
                }
                // Event handler execution exits immediately, leaving the silo shutdown running on a background thread,
                // but the app doesn't crash because a.Cancel has been set = true
            };
        }

        static ISiloHost CreateSilo()
        {
            return new SiloHostBuilder()
                .UseLocalhostClustering()
                //.ClusterOptions>(options => options.ClusterId = "MyTestCluster")
                // Prevent the silo from automatically stopping itself when the cancel key is pressed.
                .Configure<ProcessExitHandlingOptions>(options => options.FastKillOnProcessExit = false)
                //.UseDevelopmentClustering(options => options.PrimarySiloEndpoint = new IPEndPoint(IPAddress.Loopback, 11111))
                .ConfigureLogging(b => b.SetMinimumLevel(LogLevel.Debug).AddConsole())
                .AddElasticsearchTelemetryConsumer(new Uri("http://smellycat01.devint.dev-r5ead.net:9200"), "alex-orleans")
                .Build();
        }

        static async Task StopSilo()
        {
            await silo.StopAsync();
            _siloStopped.Set();
        }
    }
}