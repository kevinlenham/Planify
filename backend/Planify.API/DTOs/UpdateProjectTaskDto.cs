using Planify.API.Enums;

namespace Planify.API.DTOs
{
    public class UpdateProjectTaskDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public ProjectTaskStatus Status { get; set; }
    }
}