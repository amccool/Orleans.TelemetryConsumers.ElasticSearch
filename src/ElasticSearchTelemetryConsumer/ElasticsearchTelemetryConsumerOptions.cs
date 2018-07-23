using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.TelemetryConsumers.ElasticSearch
{
    class ElasticsearchTelemetryConsumerOptions
    {
        //[Redact]
        public Uri ElasticsearchUrl { get; set; }
        public string IndexPrefix { get; set; }
    }
}
