namespace Salus.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Name { get; set; } = string.Empty;

        public Profile Profile { get; set; } = null!;
    }
}
