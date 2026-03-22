using Planify.API.Data;
using Planify.API.DTOs;
using Planify.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Planify.API.Services
{
    public class ProjectTaskService : IProjectTaskService
    {
        private readonly PlanifyDbContext _context;

        public ProjectTaskService(PlanifyDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectTask> GetProjectTask(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null)
                throw new Exception("Task not found");
            return task;
        }

        public async Task<IEnumerable<ProjectTask>> GetProjectTasksByProject(int projectId)
        {
            return await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<ProjectTask> CreateProjectTask(ProjectTaskDto dto)
        {
            var task = new ProjectTask
            {
                Name = dto.Name,
                Description = dto.Description,
                DueDate = dto.DueDate,
                ProjectId = dto.ProjectId
            };

            _context.ProjectTasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<ProjectTask> UpdateProjectTask(int id, UpdateProjectTaskDto dto)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null)
                throw new Exception("Task not found");

            task.Name = dto.Name;
            task.Description = dto.Description;
            task.DueDate = dto.DueDate;
            task.Status = dto.Status;

            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<ProjectTask> DeleteProjectTask(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null)
                throw new Exception("Task not found");

            _context.ProjectTasks.Remove(task);
            await _context.SaveChangesAsync();
            return task;
        }
    }
}