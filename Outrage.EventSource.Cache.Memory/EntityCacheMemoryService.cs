using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Outrage.EventSource.Cache.Memory.Options;
using Outrage.EventSource.Core;
using System.Runtime.ExceptionServices;

namespace Outrage.EventSource.Cache.Memory
{
    public sealed class EntityCacheMemoryService : IEntityCache
    {
        private CancellationTokenSource expiryCancellationToken = new CancellationTokenSource();
        private readonly IMemoryCache memoryCache;
        private readonly IOptions<EntityCacheMemoryOptions> options;
        private readonly ILogger<EntityCacheMemoryService>? logger;

        public EntityCacheMemoryService(IMemoryCache memoryCache, IOptions<EntityCacheMemoryOptions> options, ILogger<EntityCacheMemoryService>? logger)
        {
            this.memoryCache = memoryCache;
            this.options = options;
            this.logger = logger;
            logger.FastLog(LogLevel.Information, "Caching entities using in memory caching for cache period {} min.".LogFormat(this.options.Value.CachePeriodMinutes));

        }


        public void EvacuateCache(bool force = false)
        {
            if (!expiryCancellationToken.IsCancellationRequested && expiryCancellationToken.Token.CanBeCanceled)
            {
                expiryCancellationToken.Cancel();
                expiryCancellationToken.Dispose();
            }

            expiryCancellationToken = new CancellationTokenSource();
        }


        public bool TryGetEntityInternal(object id, out EntityItem entity)
        {
            entity = EntityItem.Empty;
            if (this.memoryCache.TryGetValue<EntityItem>(id, out var cacheEntry))
            {
                if (cacheEntry is not null)
                {
                    entity = cacheEntry;
                    return true;
                }
                return false;
            }

            return false;
        }

        public bool TryGetEntity(Guid id, out EntityItem entity)
        {
            return TryGetEntityInternal(id, out entity);
        }

        public bool TryGetEntity(long id, out EntityItem entity)
        {
            return TryGetEntityInternal(id, out entity);
        }

        public bool TryGetEntity(string id, out EntityItem entity)
        {
            return TryGetEntityInternal(id, out entity);
        }

        public bool TryGetEntityItem(IAggregateRoot aggregateRoot, out EntityItem entity)
        {
            entity = EntityItem.Empty;
            var key = aggregateRoot.GetAggregateRootId();
            return TryGetEntityInternal(key, out entity);
        }


        public void UpdateCache(IAggregateRoot aggregateRoot, long version)
        {
            var memoryOptions = new MemoryCacheEntryOptions();
            memoryOptions.SetPriority(CacheItemPriority.Normal);
            memoryOptions.AddExpirationToken(new CancellationChangeToken(expiryCancellationToken.Token));
            memoryOptions.SetAbsoluteExpiration(
                DateTimeOffset.UtcNow.AddMinutes(this.options.Value.CachePeriodMinutes)
            );
            var key = aggregateRoot.GetAggregateRootId();

            memoryCache.Set(key, aggregateRoot, memoryOptions);
        }
    }
}