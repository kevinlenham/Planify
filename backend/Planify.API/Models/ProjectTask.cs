using ProjectTaskStatus = Planify.API.Enums.ProjectTaskStatus;

namespace Planify.API.Models
{
    public class ProjectTask : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.ToDo;
        public DateTime DueDate { get; set; } = DateTime.UtcNow;
        public int ProjectId { get; set; }
    }
}