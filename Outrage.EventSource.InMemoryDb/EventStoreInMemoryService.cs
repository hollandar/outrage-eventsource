using Outrage.EventSource.Core;
using System.Diagnostics;

namespace Outrage.EventSource.InMemoryDb
{
    public class EventStoreInMemoryService : IEventStoreService
    {
        IDictionary<string, IList<EventContainer>> events;

        public EventStoreInMemoryService()
        {
            this.events = new Dictionary<string, IList<EventContainer>>();
        }

        public Task<long> AppendToStreamAsync(string stream, string eventType, long version, string json)
        {
            IList<EventContainer> eventList;
            if (!this.events.TryGetValue(stream, out eventList))
            {
                eventList = new List<EventContainer>();
                this.events[stream] = eventList;
            }

            eventList.Add(new EventContainer(eventType, version, json, DateTimeOffset.UtcNow));

            Debug.Assert(version == eventList.Count);
            return Task.FromResult(version);
        }

#pragma warning disable CS1998

        public async IAsyncEnumerable<EventContainer> ReadStreamAsync(string streamName)
        {
            if (this.events.TryGetValue(streamName, out var eventList))
            {
                foreach (var eventContainer in eventList) yield return eventContainer;
            }
        }

#pragma warning restore CS1998

    }
}