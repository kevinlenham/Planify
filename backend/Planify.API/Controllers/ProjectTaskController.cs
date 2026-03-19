using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planify.API.Data;
using Planify.API.Models;

namespace Planify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectTasksController : ControllerBase
    {
        private readonly PlanifyDbContext _context;

        public ProjectTasksController(PlanifyDbContext context)
        {
            _context = context;
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetProjectTasks(int projectId)
        {
            var tasks = await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectTask(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProjectTask(ProjectTask task)
        {
            _context.ProjectTasks.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProjectTask), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProjectTask(int id, ProjectTask task)
        {
            if (id != task.Id) return BadRequest();
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectTask(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null) return NotFound();
            _context.ProjectTasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}