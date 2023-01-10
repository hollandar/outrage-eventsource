# Outrage.EventSource

A framework for build entities from event source logs, and to apply events to entities and store those events into a log of your choice.

In memory caching of entities is provided by:
 * Outrage.EventSource.Cache.Dict - In memory dictionary caching, with automatic cache evacuations.
 * Outrage.EventSource.Cache.Memory - In memory caching using Microsoft.Extensions.MemoryCache.

Log storage is provided by:
 * Outrage.EventSource.File - A file based storage mechanism that appends logs to a binary file.
 * Outrage.EventSource.EventStoreDb - A storage mechanism that integrates with EventStoreDb.

Storage providers should be considered an immutable collection of events.

## DDD

EventSource supports entity modelling around an aggregate root object keyed using a Guid, an Int64 for a String.
Below is an example of a Person aggregated entity, keyed by a Guid.

The aggregate root must implemented Serializers, a collection of event serializers with a specific name each, which provide instructions about serialization and deserialization of each event supported.

The aggregate root must also implement a distinct Apply method that is able to replay the event against the aggregate.

``` c#
public class Person : IAggregateRoot<PersonEventBase>, IAggregateRootWithGuidKey
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public List<Address> PostalAddresses { get; set; } = new();
    public List<Address> StreetAddresses { get; set; } = new();

    public List<EventSerializer<PersonEventBase>> Serializers
    {
        get
        {
            return new List<EventSerializer<PersonEventBase>>()
            {
                new EventSerializer<PersonEventBase, CreatePersonEvent>("CreatePersonEvent"),
                new EventSerializer<PersonEventBase, AddPostalAddress>("AddPostalAddress"),
                new EventSerializer<PersonEventBase, AddStreetAddress>("AddStreetAddress"),
                new EventSerializer<PersonEventBase, UpdateEmailAddress>("UpdateEmailAddress"),
                new EventSerializer<PersonEventBase, UpdateMobilePhone>("UpdateMobilePhone"),
            };
        }
    }

    public void Apply(CreatePersonEvent @event)
    {
        this.Name = @event.Name;
        this.MobilePhone = @event.MobilePhone;
        this.EmailAddress = @event.EmailAddress;
    }

    public void Apply(AddPostalAddress @event)
    {
        if (!this.PostalAddresses.Where(r => r.Id == @event.Id).Any())
        {
            var address = new Address()
            {
                Id = @event.Id,
                StreetNo = @event.StreetNo,
                StreetName = @event.StreetName,
                City = @event.City,
                State = @event.State,
                PostalCode = @event.PostalCode
            };
            this.PostalAddresses.Add(address);
        }
    }

    public void Apply(AddStreetAddress @event)
    {
        if (!this.StreetAddresses.Where(r => r.Id == @event.Id).Any())
        {
            var address = new Address()
            {
                Id = @event.Id,
                StreetNo = @event.StreetNo,
                StreetName = @event.StreetName,
                City = @event.City,
                State = @event.State,
                PostalCode = @event.PostalCode
            };
        }
    }

    public void Apply(RemovePostalAddress @event)
    {
        var address = this.PostalAddresses.Where(r => r.Id == @event.AddressId).SingleOrDefault();
        if (address is not null)
        {
            this.PostalAddresses.Remove(address);
        }
    }

    public void Apply(RemoveStreetAddress @event)
    {
        var address = this.StreetAddresses.Where(r => r.Id == @event.AddressId).SingleOrDefault();
        if (address is not null)
        {
            this.PostalAddresses.Remove(address);
        }
    }

    public void Apply(UpdateEmailAddress @event)
    {
        this.EmailAddress = @event.EmailAddress;
    }

    public void Apply(UpdateMobilePhone @event)
    {
        this.MobilePhone = @event.MobilePhone;
    }
}
```

In practice you should implement units tests for each apply method, and test for proper application of the event to the aggregate.

Subordinate models are plain old C# objects that are created and modified by the aggregate root Apply methods.
```c#
public class Address
{
    public Guid Id { get; set; }
    public int StreetNo { get; set; } = 0;
    public string StreetName { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}
```

Events themselves must have a common base class for the aggregate, in this case PersonEventBase.  They are not otherwise special except that they must be serializable by EventSerializer.  The default implementation uses System.Text.Json.

```c#
public abstract class PersonEventBase
{
}

public class CreatePersonEvent: PersonEventBase
{
    public string Name { get; set; } = String.Empty;
    public string MobilePhone { get; set; } = String.Empty;
    public string EmailAddress { get; set; } = String.Empty;
}
```

## Writing entities, and reading them

You can create an initial entity and apply events to it.  The initial creation is simply calling new() on your entity.  You should not assume an entity gets any data other than the defaults specified in the default constructor.

Events are then used to apply changes to your aggregate, the first event is usually the initial event, in the case above CreatePersonEvent is the initial event and should be applied to the object immediately after creation.

The framework supports dependency injection frameworks, the example uses Microsoft.Extensions.DependencyInjection by default.  To build an initial version of an entity, do the following:

```c#

var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging(options => {
    options.AddConsole();
});
serviceCollection.AddMemoryCache();
serviceCollection.AddSingleton<IEntityCache, EntityCacheMemoryService>();
serviceCollection.AddSingleton<IEventStoreService, EventStoreDbService>();
serviceCollection.AddSingleton<EntityService>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var entityService = serviceProvider.GetService<EntityService>();
var entityCache = serviceProvider.GetService<IEntityCache>();

var personId = Guid.NewGuid();
{
    var entity = await entityService.GetEntity<Person, PersonEventBase>(personId);
    await entityService.Apply(entity, new CreatePersonEvent { Name = "Johnny", MobilePhone = "0410 003 430", EmailAddress = "johnny@cash.com" });
    await entityService.Apply(entity, new AddPostalAddress { StreetNo = 14, StreetName = "Oak Close", City = "Nunnawadding", State = "Victoria", PostalCode = "3123" });
    await entityService.Apply(entity, new UpdateEmailAddress { EmailAddress = "jon@cash.org" });
}
```

This provess creates an entity, applies an initial event to it, and subsequent changes.

Once an entity exists, you can reload it later and replay all of its events against it by simply getting it via its id:
```c#
var entity = await entityService.GetEntity<Person, PersonEventBase>(personId);
```
## Roadmap

 * Support entity rebuild to a specific timestamp.
 * Mark entities as deleted.
 * Support checkpointing an entity (writing a full representation and replaying events only from those points)