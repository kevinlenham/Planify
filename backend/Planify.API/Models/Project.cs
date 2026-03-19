namespace Planify.API.Models
{
    public class Project : BaseEntity
    {
        public required string Name { get; set; }
        public string Description { get; set; }
        public int OwnerId { get; set; }
        public required User Owner { get; set; }
    }
}