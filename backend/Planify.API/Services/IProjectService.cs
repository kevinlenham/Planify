using Planify.API.DTOs;

namespace Planify.API.Services
{
    public interface IProjectService
    {
        Task<Project> GetProject(int id);
        Task<IEnumerable<Project>> GetProjectsByOwner(int ownerId);
        Task<Project> CreateProject(ProjectDto dto);
        Task<Project> UpdateProject(int id, UpdateProjectDto dto);
        Task<Project> DeleteProject(int id);
    }
}
