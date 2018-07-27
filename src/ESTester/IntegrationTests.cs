using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using Orleans.Streams;
using Orleans.TestingHost;
using TestExtensions;
using TestGrainInterfaces;
using Xunit;
using Xunit.Abstractions;
using Elasticsearch = ElasticsearchInside.Elasticsearch;

namespace ESTester
{

    public class IntegrationTests : OrleansTestingBase, IClassFixture<IntegrationTests.Fixture>
    //: HostedTestClusterEnsureDefaultStarted
    //: IClassFixture<IntegrationTests.Fixture>
    {
        protected const string streamProvider = "stuff";

        private readonly IntegrationTests.Fixture _fixture;
        private readonly ITestOutputHelper _output;

        public IntegrationTests(IntegrationTests.Fixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        [Fact]
        public async Task GenerateSomeActivity()
        {
            Guid streamId = Guid.NewGuid();

            IStreamProvider streamProviderBrc = this._fixture.Client.GetStreamProvider(streamProvider);
            IAsyncStream<object> messageStream =
                streamProviderBrc.GetStream<object>(streamId, streamProvider);


            for (int i = 0; i < 1000; i++)
            {
                await messageStream.OnNextAsync(new
                {
                    junk = "junk",
                    morejunk = 2,
                });

            }
        }

        [Fact]
        public async Task DelayUntilStatistics()
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
        }

        [Fact]
        public async Task WorkOnGrainLogging()
        {
            var testGrain = this._fixture.GrainFactory.GetGrain<ISimpleGrain>("sgsgdfgdg");
            await testGrain.DoSomething();

            await testGrain.DoSomethingElse();

        }




        public class Fixture : BaseTestClusterFixture
        {
            private static readonly ElasticsearchInside.Elasticsearch _elasticsearch = new ElasticsearchInside.Elasticsearch();

            //protected override void CheckPreconditionsOrThrow()
            //{
            //    base.CheckPreconditionsOrThrow();
            //}

            protected override void ConfigureTestCluster(TestClusterBuilder builder)
            {
                builder.AddSiloBuilderConfigurator<SiloConfigurator>();
                builder.AddClientBuilderConfigurator<ClientConfigurtor>();
            }

            public class SiloConfigurator : ISiloBuilderConfigurator
            {
                public void Configure(ISiloHostBuilder hostBuilder)
                {
                    hostBuilder
                        .AddSimpleMessageStreamProvider(name: streamProvider, configureOptions: (builder => { builder.FireAndForgetDelivery = false; }))
                        .AddMemoryGrainStorage("PubSubStore")
                        .AddElasticsearchTelemetryConsumer(_elasticsearch.Url)
                        .ConfigureServices(TestStartup.ConfigureServices);
                }
            }
            public class ClientConfigurtor : IClientBuilderConfigurator
            {
                public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
                {
                    clientBuilder
                        .AddSimpleMessageStreamProvider(streamProvider, configureOptions: (builder => { builder.FireAndForgetDelivery = false; }))
                        .AddElasticsearchTelemetryConsumer(_elasticsearch.Url)
                        .ConfigureServices(TestStartup.ConfigureServices);
                }
            }


            //protected override TestCluster CreateTestCluster()
            //{
            //    TimeSpan _timeout = Debugger.IsAttached ? TimeSpan.FromMinutes(5) : TimeSpan.FromSeconds(10);

            //    var options = new TestClusterOptions(1); //default = 2 nodes in cluster

            //    options.ClusterConfiguration.AddMemoryStorageProvider("PubSubStore");

            //    options.ClusterConfiguration.AddSimpleMessageStreamProvider(providerName: streamProvider,
            //        fireAndForgetDelivery: false);

            //    options.ClusterConfiguration.ApplyToAllNodes(c => c.DefaultTraceLevel = Orleans.Runtime.Severity.Error);
            //    options.ClusterConfiguration.ApplyToAllNodes(c => c.TraceToConsole = false);
            //    options.ClusterConfiguration.ApplyToAllNodes(c => c.TraceFileName = string.Empty);
            //    options.ClusterConfiguration.ApplyToAllNodes(c => c.TraceFilePattern = string.Empty);
            //    options.ClusterConfiguration.ApplyToAllNodes(c => c.StatisticsWriteLogStatisticsToTable = false);
            //    options.ClusterConfiguration.Globals.ClientDropTimeout = _timeout;
            //    options.ClusterConfiguration.UseStartupType<TestStartup>();

            //    options.ClientConfiguration.AddSimpleMessageStreamProvider(providerName: streamProvider,
            //        fireAndForgetDelivery: false);

            //    options.ClientConfiguration.DefaultTraceLevel = Orleans.Runtime.Severity.Error;
            //    options.ClientConfiguration.TraceToConsole = false;
            //    options.ClientConfiguration.TraceFileName = string.Empty;
            //    options.ClientConfiguration.ClientDropTimeout = _timeout;


            //    return new TestCluster(options);
            //}



            public class TestStartup
            {

                public static void ConfigureServices(IServiceCollection services)
                {

                    //return services.BuildServiceProvider();
                }
            }
        }

    }


}

