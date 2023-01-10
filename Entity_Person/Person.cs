using Example.PersonEvents;
using Outrage.EventSource.Core;

namespace Example
{
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
}