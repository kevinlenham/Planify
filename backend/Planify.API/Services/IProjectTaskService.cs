using Planify.API.DTOs;
using Planify.API.Models;

namespace Planify.API.Services
{
    public interface IProjectTaskService
    {
        Task<ProjectTask> GetProjectTask(int id);
        Task<IEnumerable<ProjectTask>> GetProjectTasksByProject(int projectId);
        Task<ProjectTask> CreateProjectTask(ProjectTaskDto dto);
        Task<ProjectTask> UpdateProjectTask(int id, UpdateProjectTaskDto dto);
        Task<ProjectTask> DeleteProjectTask(int id);
    }
}