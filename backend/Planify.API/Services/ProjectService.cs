using Planify.API.Data;
using Planify.API.DTOs;
using Planify.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Planify.API.Services
{
    public class ProjectService : IProjectService
    {
        private readonly PlanifyDbContext _context;

        public ProjectService(PlanifyDbContext context)
        {
            _context = context;
        }

        public async Task<Project> GetProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
                throw new Exception("Project not found");
            return project;
        }

        public async Task<IEnumerable<Project>> GetProjectsByOwner(int ownerId)
        {
            return await _context.Projects
                .Include(p => p.Tasks)
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<Project> CreateProject(ProjectDto dto)
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                OwnerId = dto.OwnerId
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<Project> UpdateProject(int id, UpdateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                throw new Exception("Project not found");

            project.Name = dto.Name;
            project.Description = dto.Description;

            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<Project> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                throw new Exception("Project not found");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return project;
        }
    }
}