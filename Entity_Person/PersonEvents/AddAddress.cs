namespace Example.PersonEvents
{
    public abstract class AddAddress: PersonEventBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int StreetNo { get; set; } = 0;
        public string StreetName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
    }

    public sealed class AddPostalAddress:AddAddress { }
    public sealed class AddStreetAddress:AddAddress { }
}