namespace Entity_Person.PersonEvents
{
    public sealed class SetAgeEvent: PersonEventBase
    {
        public int Age { get; set; }
    }
}
