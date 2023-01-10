using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Outrage.EventSource.Core
{

    public abstract class EventSerializer<TEventBase>
    {
        public abstract TEventBase? Deserialize(string json);
        public abstract string Serialize(TEventBase @event);
        public abstract string EventType { get; }
        public abstract bool CanSerialize<T>();
        public abstract bool CanSerialize(Type type);
        public abstract Type GetEventType();
    }

    public class EventSerializer<TEventBase, TEventType> : EventSerializer<TEventBase> where TEventType : TEventBase
    {
        private string eventType;

        public EventSerializer(string? eventType = null)
        {
            this.eventType = eventType ?? typeof(TEventType).Name;
        }

        public override string EventType => eventType;

        public override TEventBase? Deserialize(string json)
        {
            return JsonSerializer.Deserialize<TEventType>(json);
        }

        public override string Serialize(TEventBase @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException();
            }

            var eventType = (TEventType)@event;
            return JsonSerializer.Serialize<TEventType>((TEventType)@event);
        }

        public override bool CanSerialize<T>()
        {
            return CanSerialize(typeof(T));
        }

        public override bool CanSerialize(Type type)
        {
            return type == typeof(TEventType);
        }

        public override Type GetEventType()
        {
            return typeof(TEventType);
        }
    }
}
