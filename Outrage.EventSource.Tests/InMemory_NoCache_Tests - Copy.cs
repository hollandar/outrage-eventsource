using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Outrage.EventSource.Cache.Dict;
using Outrage.EventSource.Cache.Dict.Options;
using Outrage.EventSource.Core;
using Outrage.EventSource.InMemoryDb;

namespace Outrage.EventSource.Tests
{
    [TestClass]
    public class InMemory_NoCache_Tests: TestsBase
    {
        [TestInitialize]
        public void InitializeEnvironment()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(options =>
            {
                options.AddConsole();
            });

            serviceCollection.AddSingleton<IEventStoreService, EventStoreInMemoryService>();
            serviceCollection.AddSingleton<IEntityService, EntityService>();

            this.serviceProvider = serviceCollection.BuildServiceProvider();
        }

    }
}