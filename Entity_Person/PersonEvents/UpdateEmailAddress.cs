namespace Example.PersonEvents
{
    public class UpdateEmailAddress:PersonEventBase
    {
        public string EmailAddress { get; set; } = String.Empty;
    }
}
