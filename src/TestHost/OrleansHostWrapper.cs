using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;
using Orleans.Telemetry;

namespace TestHost
{
    internal class OrleansHostWrapper : IDisposable
    {
        private ISiloHost siloHost;
        private ElasticsearchInside.Elasticsearch elasticsearch;

        /// <summary>
        /// start primary
        /// </summary>
        public OrleansHostWrapper()
        {
            var clusterConfig = ClusterConfiguration.LocalhostPrimarySilo();

            // add providers to the legacy configuration object.
            //clusterConfig.AddMemoryStorageProvider();

        elasticsearch = new ElasticsearchInside.Elasticsearch();

            ///
            /// see https://elk-docker.readthedocs.io/
            /// for an easy way to run a ELK stack via docker
            /// 
            /// or on windows this docker-compose.yml file
            /// https://gist.github.com/jeoffman/91082bfe7d30ae2f74c07fac7db5e53b
            /// and run docker-compose.exe in the same dir

            //var elasticSearchURL = new Uri("http://elasticsearch:9200");
            var elasticSearchURL = elasticsearch.Url;

            //var esTeleM = new ElasticsearchTelemetryConsumer(elasticSearchURL, "orleans-telemetry");
            //LogManager.TelemetryConsumers.Add(esTeleM);
            //LogManager.LogConsumers.Add(esTeleM);


            var builder = new SiloHostBuilder()
                .UseConfiguration(clusterConfig)
                // Add assemblies to scan for grains and serializers.
                // For more info read the Application Parts section
                //.ConfigureApplicationParts(parts =>
                //    parts.AddApplicationPart(typeof(HelloGrain).Assembly)
                //        .WithReferences())
                // Configure logging with any logging framework that supports Microsoft.Extensions.Logging.
                // In this particular case it logs using the Microsoft.Extensions.Logging.Console package.
                .ConfigureLogging(logging => logging.AddConsole())
                .AddElasticsearchTelemetryConsumer(elasticsearch.Url, "alex-orleans");

            siloHost = builder.Build();

            //siloHost = new SiloHost("primary", clusterConfig);
        }



        public bool Run()
        {
            var ok = false;

            try
            {
                siloHost.StartAsync().ConfigureAwait(false);
                //siloHost.InitializeOrleansSilo();

                //ok = siloHost.StartOrleansSilo();
                //if (!ok)
                //    throw new SystemException(string.Format("Failed to start Orleans silo '{0}' as a {1} node.",
                //        siloHost.Name, siloHost.Type));
            }
            catch (Exception exc)
            {
                //siloHost.ReportStartupError(exc);
                //var msg = string.Format("{0}:\n{1}\n{2}", exc.GetType().FullName, exc.Message, exc.StackTrace);
                Console.WriteLine(exc);
                throw;
            }

            return ok;
        }

        public bool Stop()
        {
            var ok = false;

            try
            {
                siloHost.StopAsync().ConfigureAwait(false);
            }
            catch (Exception exc)
            {
                //siloHost.ReportStartupError(exc);
                //var msg = $"{exc.GetType().FullName}:\n{exc.Message}\n{exc.StackTrace}";
                Console.WriteLine(exc);
                throw;
            }

            return ok;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool dispose)
        {
            elasticsearch?.Dispose();
            siloHost.Dispose();
            siloHost = null;
        }
    }
}
