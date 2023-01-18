using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Outrage.EventSource.Cache.Dict;
using Outrage.EventSource.Cache.Dict.Options;
using Outrage.EventSource.Core;
using Outrage.EventSource.File;
using Outrage.EventSource.File.Options;
using Outrage.EventSource.InMemoryDb;
using System.Runtime.CompilerServices;

namespace Outrage.EventSource.Tests
{
    [TestClass]
    public class File_NoCache_Tests: TestsBase
    {
        DirectoryInfo? folder = null;

        [TestInitialize]
        public void InitializeEnvironment()
        {
            folder = Directory.CreateTempSubdirectory();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(options =>
            {
                options.AddConsole();
            });
            serviceCollection.Configure<EventStoreFileOptions>(configuration => configuration.Folder = folder.FullName);
            serviceCollection.AddSingleton<IEventStoreService, EventStoreFileService>();
            serviceCollection.AddSingleton<IEventStoreService, EventStoreInMemoryService>();
            serviceCollection.AddSingleton<IEntityService, EntityService>();

            this.serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (folder != null)
            {
                Directory.Delete(folder.FullName, true);
            }
        }

    }
}