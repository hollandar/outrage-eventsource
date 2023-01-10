using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventSource.Core
{
    public interface IEntityService
    {
        ValueTask<TEntity> GetEntity<TEntity, TEventBase>(Guid id) where TEntity : IAggregateRootWithGuidKey, IAggregateRoot<TEventBase>, new();
        ValueTask<TEntity> GetEntity<TEntity, TEventBase>(long id) where TEntity : IAggregateRootWithLongKey, IAggregateRoot<TEventBase>, new();
        ValueTask<TEntity> GetEntity<TEntity, TEventBase>(string id) where TEntity : IAggregateRootWithStringKey, IAggregateRoot<TEventBase>, new();
        Task<IAggregateRoot<TEventBase>> Apply<TEventBase>(IAggregateRoot<TEventBase> aggregateRoot, IEnumerable<TEventBase> events);
        Task<IAggregateRoot<TEventBase>> Apply<TEventBase>(IAggregateRoot<TEventBase> aggregateRoot, TEventBase @event);
    }
}
