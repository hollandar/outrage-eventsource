using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventSource.Core
{
    public interface IAggregateRootEvent
    {

    }

    public sealed class AggregateRootEvent<TKey, TEventBase> : IAggregateRootEvent
    {
        public TKey Key { get; init; }
        public TEventBase Event { get; init; }

        public AggregateRootEvent(TKey key, TEventBase @event)
        {
            Key = key;
            Event = @event;
        }
    }
}
