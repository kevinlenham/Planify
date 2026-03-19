using Planify.API.Enums;

namespace Planify.API.Models
{
    public class Task : BaseEntity
    {
        public required string Name { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        public DateTime DueDate { get; set; } = DateTime.UtcNow;
        public int ProjectId { get; set; }
        public required Project Project { get; set; }
        public int? AssignedUserId { get; set; }
        public User? AssignedUser { get; set; }
    }
}