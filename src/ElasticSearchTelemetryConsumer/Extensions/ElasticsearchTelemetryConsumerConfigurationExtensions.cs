﻿//using System;
//using System.Collections.Generic;
//using Orleans.Runtime.Configuration;

//namespace Orleans.Telemetry
//{
//    public static class ProviderConfigurationExtensions
//    {
//        public static void AddElasticSearchStatisticsProvider(this ClusterConfiguration config,
//            string providerName, Uri ElasticHostAddress, string ElasticIndex= "orleans_statistics", string ElasticMetricType= "metric", string ElasticCounterType = "counter")
//        {
//            if (string.IsNullOrWhiteSpace(providerName)) throw new ArgumentNullException(nameof(providerName));
//            if (string.IsNullOrWhiteSpace(ElasticIndex)) throw new ArgumentNullException(nameof(ElasticIndex));
//            if (string.IsNullOrWhiteSpace(ElasticMetricType)) throw new ArgumentNullException(nameof(ElasticMetricType));
//            if (string.IsNullOrWhiteSpace(ElasticCounterType)) throw new ArgumentNullException(nameof(ElasticCounterType));

//            var properties = new Dictionary<string, string>
//            {
//                {"ElasticHostAddress", ElasticHostAddress.ToString()},
//                {"ElasticIndex", ElasticIndex},
//                {"ElasticMetricsType", ElasticMetricType},
//                {"ElasticCounterType", ElasticCounterType},
//            };

//            config.Globals.RegisterStatisticsProvider<ElasticStatisticsProvider>(providerName, properties);
//        }

//        public static void AddElasticSearchStatisticsProvider(this ClientConfiguration config,
//            string providerName, Uri ElasticHostAddress, string ElasticIndex = "orleans_statistics", string ElasticType = "metrics")
//        {
//            if (string.IsNullOrWhiteSpace(providerName)) throw new ArgumentNullException(nameof(providerName));
//            if (string.IsNullOrWhiteSpace(ElasticIndex)) throw new ArgumentNullException(nameof(ElasticIndex));
//            if (string.IsNullOrWhiteSpace(ElasticType)) throw new ArgumentNullException(nameof(ElasticType));

//            var properties = new Dictionary<string, string>
//            {
//                {"ElasticHostAddress", ElasticHostAddress.ToString()},
//                {"ElasticIndex", ElasticIndex},
//                {"ElasticMetricsType", ElasticType},
//            };

//            config.RegisterStatisticsProvider<ElasticClientMetricsProvider>(providerName, properties);
//        }

//    }
//}

using System;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Telemetry;
using Orleans.TelemetryConsumers.ElasticSearch;

namespace Orleans.Hosting
{
    public static class ElasticsearchTelemetryConsumerConfigurationExtensions
    {
        /// <summary>
        /// Adds a metrics telemetric consumer provider of type <see cref="AITelemetryConsumer"/>.
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="uri">elasticsearch url</param>
        /// <param name="indexprefix">prefix for the index names</param>
        /// <param name="dateFormatter">date to string formatter for the index</param>
        /// <param name="bufferWaitSeconds">number of second to wait before sending if the buffer is not full</param>
        /// <param name="bufferSize">maximum number of documents to send at one time</param>
        public static ISiloHostBuilder AddElasticsearchTelemetryConsumer(this ISiloHostBuilder hostBuilder, Uri uri, string indexprefix = "orleans-telemetry-", string dateFormatter = "yyyy-MM-dd-HH", int bufferWaitSeconds = 1, int bufferSize = 50)
        {
            return hostBuilder.ConfigureServices((context, services) => ConfigureServices(context, services, uri, indexprefix, dateFormatter, bufferWaitSeconds , bufferSize));
        }

        /// <summary>
        /// Adds a metrics telemetric consumer provider of type <see cref="AITelemetryConsumer"/>.
        /// </summary>
        /// <param name="clientBuilder"></param>
        /// <param name="uri">elasticsearch url</param>
        /// <param name="indexprefix">prefix for the index names</param>
        /// <param name="dateFormatter">date to string formatter for the index</param>
        /// <param name="bufferWaitSeconds">number of second to wait before sending if the buffer is not full</param>
        /// <param name="bufferSize">maximum number of documents to send at one time</param>
        public static IClientBuilder AddElasticsearchTelemetryConsumer(this IClientBuilder clientBuilder, Uri uri, string indexprefix = "orleans-telemetry-", string dateFormatter = "yyyy-MM-dd-HH", int bufferWaitSeconds = 1, int bufferSize = 50)
        {
            return clientBuilder.ConfigureServices((context, services) => ConfigureServices(context, services, uri, indexprefix, dateFormatter, bufferWaitSeconds, bufferSize));
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services, Uri uri, string indexprefix, string dateFormatter, int bufferWaitSeconds, int bufferSize)
        {
            services.ConfigureFormatter<ElasticsearchTelemetryConsumerOptions>();
            services.Configure<TelemetryOptions>(options => options.AddConsumer<ElasticsearchTelemetryConsumer>());

            if (!string.IsNullOrWhiteSpace(indexprefix))
                services.Configure<ElasticsearchTelemetryConsumerOptions>(options =>
                {
                    options.ElasticsearchUrl = uri;
                    options.IndexPrefix = indexprefix;
                    options.dateFormatter = dateFormatter;
                    options.bufferWaitSeconds = bufferWaitSeconds;
                    options.bufferSize = bufferSize;
                });
        }
    }
}