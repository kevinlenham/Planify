namespace Planify.API.DTOs
{
    public class UpdateProjectDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}