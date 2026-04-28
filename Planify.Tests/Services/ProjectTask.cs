using Microsoft.EntityFrameworkCore;
using Planify.API.Data;
using Planify.API.DTOs;
using Planify.API.Models;
using Planify.API.Services;
using Planify.API.Enums;
using FluentAssertions;

namespace Planify.Tests.Services
{
    public class ProjectTaskServiceTests
    {
        private PlanifyDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<PlanifyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new PlanifyDbContext(options);
        }

        // -------------------------
        // GetProjectTask Tests
        // -------------------------

        [Fact]
        public async Task GetProjectTask_ExistingId_ReturnsTask()
        {
            var context = CreateInMemoryContext();
            context.ProjectTasks.Add(new ProjectTask { Id = 1, Name = "Test Task", ProjectId = 1 });
            await context.SaveChangesAsync();
            var service = new ProjectTaskService(context);

            var result = await service.GetProjectTask(1);

            result.Should().NotBeNull();
            result.Name.Should().Be("Test Task");
        }

        [Fact]
        public async Task GetProjectTask_InvalidId_ThrowsException()
        {
            var context = CreateInMemoryContext();
            var service = new ProjectTaskService(context);

            await Assert.ThrowsAsync<Exception>(() => service.GetProjectTask(999));
        }

        // -------------------------
        // GetProjectTasksByProject Tests
        // -------------------------

        [Fact]
        public async Task GetProjectTasksByProject_ReturnsOnlyProjectTasks()
        {
            var context = CreateInMemoryContext();
            context.ProjectTasks.AddRange(
                new ProjectTask { Id = 1, Name = "Task A", ProjectId = 1 },
                new ProjectTask { Id = 2, Name = "Task B", ProjectId = 1 },
                new ProjectTask { Id = 3, Name = "Task C", ProjectId = 2 }
            );
            await context.SaveChangesAsync();
            var service = new ProjectTaskService(context);

            var result = await service.GetProjectTasksByProject(1);

            result.Should().HaveCount(2);
            result.Should().AllSatisfy(t => t.ProjectId.Should().Be(1));
        }

        [Fact]
        public async Task GetProjectTasksByProject_NoTasks_ReturnsEmptyList()
        {
            var context = CreateInMemoryContext();
            var service = new ProjectTaskService(context);

            var result = await service.GetProjectTasksByProject(999);

            result.Should().BeEmpty();
        }

        // -------------------------
        // CreateProjectTask Tests
        // -------------------------

        [Fact]
        public async Task CreateProjectTask_ValidDto_ReturnsCreatedTask()
        {
            var context = CreateInMemoryContext();
            var service = new ProjectTaskService(context);
            var dto = new ProjectTaskDto
            {
                Name = "New Task",
                Description = "Test description",
                DueDate = DateTime.UtcNow.AddDays(7),
                ProjectId = 1
            };

            var result = await service.CreateProjectTask(dto);

            result.Should().NotBeNull();
            result.Name.Should().Be("New Task");
            result.ProjectId.Should().Be(1);
            context.ProjectTasks.Count().Should().Be(1);
        }

        // -------------------------
        // UpdateProjectTask Tests
        // -------------------------

        [Fact]
        public async Task UpdateProjectTask_ExistingId_UpdatesAndReturnsTask()
        {
            var context = CreateInMemoryContext();
            context.ProjectTasks.Add(new ProjectTask { Id = 1, Name = "Old Name", ProjectId = 1 });
            await context.SaveChangesAsync();
            var service = new ProjectTaskService(context);
            var dto = new UpdateProjectTaskDto
            {
                Name = "New Name",
                Description = "Updated",
                DueDate = DateTime.UtcNow.AddDays(3),
                Status = ProjectTaskStatus.InProgress
            };

            var result = await service.UpdateProjectTask(1, dto);

            result.Name.Should().Be("New Name");
            result.Status.Should().Be(ProjectTaskStatus.InProgress);
        }

        [Fact]
        public async Task UpdateProjectTask_InvalidId_ThrowsException()
        {
            var context = CreateInMemoryContext();
            var service = new ProjectTaskService(context);
            var dto = new UpdateProjectTaskDto { Name = "New Name", Description = "Updated" };

            await Assert.ThrowsAsync<Exception>(() => service.UpdateProjectTask(999, dto));
        }

        // -------------------------
        // DeleteProjectTask Tests
        // -------------------------

        [Fact]
        public async Task DeleteProjectTask_ExistingId_RemovesTask()
        {
            var context = CreateInMemoryContext();
            context.ProjectTasks.Add(new ProjectTask { Id = 1, Name = "To Delete", ProjectId = 1 });
            await context.SaveChangesAsync();
            var service = new ProjectTaskService(context);

            await service.DeleteProjectTask(1);

            context.ProjectTasks.Count().Should().Be(0);
        }

        [Fact]
        public async Task DeleteProjectTask_InvalidId_ThrowsException()
        {
            var context = CreateInMemoryContext();
            var service = new ProjectTaskService(context);

            await Assert.ThrowsAsync<Exception>(() => service.DeleteProjectTask(999));
        }
    }
}