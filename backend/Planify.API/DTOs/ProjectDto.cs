namespace Planify.API.DTOs
{
    public class ProjectDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int OwnerId { get; set; }
    }
}