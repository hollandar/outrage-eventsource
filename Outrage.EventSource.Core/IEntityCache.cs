using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outrage.EventSource.Core
{
    public class EntityItem
    {
        readonly IAggregateRoot? aggregateRoot;

        public EntityItem(IAggregateRoot? entity, long version, DateTimeOffset? expires = null)
        {
            aggregateRoot = entity;
            Expires = expires ?? DateTimeOffset.UtcNow.AddMinutes(30);
            Version = version;
        }

        public IAggregateRoot Entity
        {
            get
            {
                if (aggregateRoot is null)
                {
                    throw new NullReferenceException(nameof(Entity));
                }

                if (Expires < DateTimeOffset.UtcNow)
                {
                    throw new EntityExpiredException();
                }

                return aggregateRoot;
            }
        }
        public DateTimeOffset Expires { get; set; }
        public long Version { get; set; }

        public static EntityItem Empty = new EntityItem(null, -1, DateTimeOffset.MinValue);
    }


    public interface IEntityCache
    {
        bool TryGetEntityItem(IAggregateRoot aggregateRoot, out EntityItem entity);
        bool TryGetEntity(Guid id, out EntityItem entity);
        bool TryGetEntity(long id, out EntityItem entity);
        bool TryGetEntity(string id, out EntityItem entity);
        void EvacuateCache(bool force = false);
        void UpdateCache(IAggregateRoot aggregateRoot, long version);
    }
}
