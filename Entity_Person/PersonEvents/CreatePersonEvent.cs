namespace Entity_Person.PersonEvents
{
    public class CreatePersonEvent: PersonEventBase
    {
        public string Name { get; set; } = String.Empty;
        public string MobilePhone { get; set; } = String.Empty;
        public string EmailAddress { get; set; } = String.Empty;
    }
}