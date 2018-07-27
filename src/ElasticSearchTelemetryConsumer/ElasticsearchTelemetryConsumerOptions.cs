using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.TelemetryConsumers.ElasticSearch
{
    public class ElasticsearchTelemetryConsumerOptions
    {
        //[Redact]
        public Uri ElasticsearchUrl { get; set; }
        public string IndexPrefix { get; set; }
        public string dateFormatter { get; set; } = "yyyy-MM-dd-HH";
        public int bufferWaitSeconds { get; set; }= 1;
        public int bufferSize { get; set; } = 50;

    }
}
