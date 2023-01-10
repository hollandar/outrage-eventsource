using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventSource.Core
{
    public static class EventSourceExtensions
    {
        public static object GetAggregateRootId(this IAggregateRoot aggregateRoot)
        {
            object id = aggregateRoot switch
            {
                IAggregateRootWithGuidKey guidEntity => guidEntity.Id,
                IAggregateRootWithLongKey longEntity => longEntity.Id,
                IAggregateRootWithStringKey stringEntity => stringEntity.Id,
                _ => throw new EntityServiceException($"{aggregateRoot.GetType().Name} does not have a key type."),
            };

            return id;
        }

        public static TId? GetAggregateRootId<TId>(this IAggregateRoot aggregateRoot)
        {
            var id = aggregateRoot.GetAggregateRootId();
            if (id is TId)
            {
                return (TId)id;
            }
            
            return default(TId);
        }
    }
}
