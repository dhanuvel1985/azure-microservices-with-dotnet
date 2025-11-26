namespace Pet_Management_Api.DataAccess.Entities
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int BreedId { get; set; }
        public Breed Breed { get; set; }
    }

    public record Breed(int Id, string Name);
}
