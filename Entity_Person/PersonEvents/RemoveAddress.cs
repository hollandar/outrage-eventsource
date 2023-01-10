namespace Example.PersonEvents
{
    public abstract class RemoveAddress: PersonEventBase
    {
        public Guid AddressId { get; set; } = Guid.Empty;
    }

    public sealed class RemovePostalAddress : RemoveAddress { }
    public sealed class RemoveStreetAddress : RemoveAddress { }
}
