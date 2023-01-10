using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventSource.Core
{
    public class EventContainer
    {
        private string eventType;
        private long version;
        private string json;
        private DateTimeOffset timestamp;

        public EventContainer(string eventType, long version, string json, DateTimeOffset timestamp)
        {
            this.eventType = eventType;
            this.version = version;
            this.json = json;
            this.timestamp = timestamp;
        }

        public string EventType { get { return eventType; } }
        public long Version { get { return version; } }
        public string Json { get { return json; } }
        public long Timestamp { get { return timestamp.ToUnixTimeMilliseconds(); } }
        public DateTimeOffset TimestampDto { get { return timestamp; } }

    }

    public interface IEventStoreService
    {
        IAsyncEnumerable<EventContainer> ReadStreamAsync(string streamName);
        Task<long> AppendToStreamAsync(string stream, string eventType, long version, string json);
    }
}
