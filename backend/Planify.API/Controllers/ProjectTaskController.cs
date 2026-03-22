using Microsoft.AspNetCore.Mvc;
using Planify.API.DTOs;
using Planify.API.Services;

namespace Planify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectTasksController : ControllerBase
    {
        private readonly IProjectTaskService _projectTaskService;

        public ProjectTasksController(IProjectTaskService projectTaskService)
        {
            _projectTaskService = projectTaskService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectTask(int id)
        {
            try
            {
                var response = await _projectTaskService.GetProjectTask(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetProjectTasksByProject(int projectId)
        {
            try
            {
                var response = await _projectTaskService.GetProjectTasksByProject(projectId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProjectTask(ProjectTaskDto dto)
        {
            try
            {
                var response = await _projectTaskService.CreateProjectTask(dto);
                return CreatedAtAction(nameof(GetProjectTask), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProjectTask(int id, UpdateProjectTaskDto dto)
        {
            try
            {
                var response = await _projectTaskService.UpdateProjectTask(id, dto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectTask(int id)
        {
            try
            {
                var response = await _projectTaskService.DeleteProjectTask(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}