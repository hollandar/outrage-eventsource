using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orleans.EventSourcing.EventStoreDb.Options
{
    public class EventStoreOptions
    {
        public string Uri { get; set; } = "esdb+discover://localhost:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000";
    }
}
