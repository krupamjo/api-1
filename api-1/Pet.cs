namespace api_1
{
    public class Pet
    {
        public Pet(string name, DateTimeOffset dateOfBirth)
        {
            Name = name;
            DateOfBirth = dateOfBirth;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
