using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Outrage.EventSource.Cache.Dict.Options;
using Outrage.EventSource.Core;

namespace Outrage.EventSource.Cache.Dict
{
    public sealed class EntityCacheDictService : IEntityCache
    {
        private readonly IOptions<EntityCacheDictOptions> options;
        private readonly ILogger<EntityCacheDictService>? logger;
        private readonly Dictionary<object, EntityItem> _cache = new();
        private readonly SemaphoreSlim semaphore = new(1);
        public EntityCacheDictService(IOptions<EntityCacheDictOptions> options, ILogger<EntityCacheDictService>? logger)
        {
            this.options = options;
            this.logger = logger;
            logger.FastLog(LogLevel.Information, "Caching entities using an in memory dictionary.".LogFormat());
        }


        public void EvacuateCache(bool force = false)
        {
            try
            {
                semaphore.Wait();
                if (force)
                {
                    _cache.Clear();
                }
                else
                {
                    var validUntil = DateTimeOffset.UtcNow;
                    foreach (var key in _cache.Keys)
                    {
                        var value = _cache[key];
                        if (validUntil > value.Expires)
                        {
                            _cache.Remove(key);
                        }
                    }
                }

                logger.FastLog(LogLevel.Debug, "Entity cache evacuated.".LogFormat());
            }
            finally
            {
                semaphore.Release();
            }
        }


        public bool TryGetEntity(Guid id, out EntityItem entity)
        {
            if (_cache.TryGetValue(id, out var entityItem))
            {
                entity = entityItem;
                return true;
            }

            logger.FastLog(LogLevel.Debug, "Cache miss on id {}.".LogFormat(id));
            entity = EntityItem.Empty;
            return false;
        }

        public bool TryGetEntity(long id, out EntityItem entity)
        {
            if (_cache.TryGetValue(id, out var entityItem))
            {
                entity = entityItem;
                return true;
            }

            logger.FastLog(LogLevel.Debug, "Cache miss on id {}.".LogFormat(id));
            entity = EntityItem.Empty;
            return false;
        }

        public bool TryGetEntity(string id, out EntityItem entity)
        {
            if (_cache.TryGetValue(id, out var entityItem))
            {
                entity = entityItem;
                return true;
            }

            logger.FastLog(LogLevel.Debug, "Cache miss on id {}.".LogFormat(id));
            entity = EntityItem.Empty;
            return false;
        }

        public bool TryGetEntityItem(IAggregateRoot aggregateRoot, out EntityItem entity)
        {
            var id = aggregateRoot.GetAggregateRootId();
            if (_cache.TryGetValue(id, out var entityItem))
            {
                entity = entityItem;
                return true;
            }

            logger.FastLog(LogLevel.Debug, "Cache miss on id {}.".LogFormat(id));
            entity = EntityItem.Empty;
            return false;
        }

        public void UpdateCache(IAggregateRoot aggregateRoot, long version)
        {
            var id = aggregateRoot.GetAggregateRootId();
            _cache[id] = new EntityItem(aggregateRoot, version, DateTimeOffset.UtcNow.AddMicroseconds(this.options.Value.CachePeriodMinutes));
        }
    }
}
