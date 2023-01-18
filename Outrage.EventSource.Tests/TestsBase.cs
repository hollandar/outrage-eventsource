using Entity_Person.PersonEvents;
using Example;
using Microsoft.Extensions.DependencyInjection;
using Outrage.EventSource.Core;

namespace Outrage.EventSource.Tests
{
    public abstract class TestsBase
    {
        protected IServiceProvider? serviceProvider = null;

        protected void EvacuateCache()
        {
            var entityCache = this.serviceProvider?.GetService<IEntityCache>();
            entityCache?.EvacuateCache(true);
        }

        [TestMethod]
        public void EntityServiceAvailable()
        {
            var entityService = this.serviceProvider?.GetService<IEntityService>();
            Assert.IsNotNull(entityService, "Entity service was not properly created.");

        }

        [TestMethod]
        public async Task PersistentId()
        {
            var entityService = serviceProvider!.GetService<IEntityService>();
            var personId = Guid.NewGuid();
            var person = await entityService!.GetEntity<Person, PersonEventBase>(personId);
            var person2 = await entityService.GetEntity<Person, PersonEventBase>(personId);

            Assert.AreEqual(person.Id, person2.Id);
        }

        protected async Task<Person> CreateTestPerson(IEntityService entityService, Guid id)
        {
            var personId = Guid.NewGuid();
            var person = await entityService.GetEntity<Person, PersonEventBase>(id);

            await entityService.Apply(person, new CreatePersonEvent
            {
                Name = "Jon",
                EmailAddress = "jon@example.com",
                MobilePhone = "0400111222"
            });

            return person;
        }

        [TestMethod]
        public async Task ApplyEvent()
        {
            var entityService = serviceProvider!.GetService<IEntityService>();
            var personId = Guid.NewGuid();
            var person = await CreateTestPerson(entityService!, personId);

            EvacuateCache();

            var another = await entityService!.GetEntity<Person, PersonEventBase>(personId);
            Assert.AreEqual(person.Name, another.Name);
            Assert.AreEqual("Jon", another.Name);
            Assert.AreEqual(person.EmailAddress, another.EmailAddress);
            Assert.AreEqual("jon@example.com", another.EmailAddress);
            Assert.AreEqual(person.MobilePhone, another.MobilePhone);
            Assert.AreEqual("0400111222", another.MobilePhone);
        }

        [TestMethod]
        public async Task ApplyEvents()
        {
            var entityService = serviceProvider!.GetService<IEntityService>();
            var personId = Guid.NewGuid();
            var person = await CreateTestPerson(entityService!, personId);

            await entityService!.Apply(person, new AddPostalAddress()
            {
                Id = Guid.NewGuid(),
                City = "Someplace"
            });
            EvacuateCache();

            var another = await entityService!.GetEntity<Person, PersonEventBase>(personId);
            Assert.AreEqual(1, another.PostalAddresses.Count);
            Assert.AreEqual("Someplace", another.PostalAddresses.First().City);
        }

        [TestMethod]
        public async Task ApplyMany()
        {
            var entityService = serviceProvider!.GetService<IEntityService>();
            var personId = Guid.NewGuid();
            var person = await CreateTestPerson(entityService!, personId);

            var random = new Random();
            int lastAge = -1;
            for (int i = 0; i < 100; i++)
            {
                lastAge = random.Next(0, 99);
                await entityService!.Apply(person, new SetAgeEvent()
                {
                    Age = lastAge
                });
            }
            EvacuateCache();

            var another = await entityService!.GetEntity<Person, PersonEventBase>(personId);
            Assert.AreEqual(lastAge, another.Age);
        }

    }
}