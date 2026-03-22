namespace Planify.API.DTOs
{
    public class ProjectTaskDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public int ProjectId { get; set; }
    }
}