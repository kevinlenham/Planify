using Microsoft.EntityFrameworkCore;
using Planify.API.Data;
using Planify.API.DTOs;
using Planify.API.Models;
using Planify.API.Services;
using FluentAssertions;

namespace Planify.Tests.Services
{
    public class ProjectServiceTests
    {
        private PlanifyDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<PlanifyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new PlanifyDbContext(options);
        }

        [Fact]
        public async Task GetProject_ExistingId_ReturnsProject()
        {
            var context = CreateInMemoryContext();
            context.Projects.Add(new Project { Id = 1, Name = "Test Project", OwnerId = 1 });
            await context.SaveChangesAsync();
            var service = new ProjectService(context);

            var result = await service.GetProject(1);

            result.Should().NotBeNull();
            result.Name.Should().Be("Test Project");
        }

        [Fact]
        public async Task GetProject_InvalidId_ThrowsException()
        {
            var context = CreateInMemoryContext();
            var service = new ProjectService(context);

            await Assert.ThrowsAsync<Exception>(() => service.GetProject(999));
        }

        [Fact]
        public async Task CreateProject_ValidDto_ReturnsCreatedProject()
        {
            var context = CreateInMemoryContext();
            var service = new ProjectService(context);
            var dto = new ProjectDto { Name = "New Project", Description = "Test desc", OwnerId = 1 };

            var result = await service.CreateProject(dto);

            result.Should().NotBeNull();
            result.Name.Should().Be("New Project");
            context.Projects.Count().Should().Be(1);
        }

        [Fact]
        public async Task UpdateProject_ExistingId_UpdatesAndReturnsProject()
        {
            var context = CreateInMemoryContext();
            context.Projects.Add(new Project { Id = 1, Name = "Old Name", OwnerId = 1 });
            await context.SaveChangesAsync();
            var service = new ProjectService(context);
            var dto = new UpdateProjectDto { Name = "New Name", Description = "Updated" };

            var result = await service.UpdateProject(1, dto);

            result.Name.Should().Be("New Name");
        }

        [Fact]
        public async Task UpdateProject_InvalidId_ThrowsException()
        {
            var context = CreateInMemoryContext();
            var service = new ProjectService(context);
            var dto = new UpdateProjectDto { Name = "New Name", Description = "Updated" };

            await Assert.ThrowsAsync<Exception>(() => service.UpdateProject(999, dto));
        }

        [Fact]
        public async Task DeleteProject_ExistingId_RemovesProject()
        {
            var context = CreateInMemoryContext();
            context.Projects.Add(new Project { Id = 1, Name = "To Delete", OwnerId = 1 });
            await context.SaveChangesAsync();
            var service = new ProjectService(context);

            await service.DeleteProject(1);

            context.Projects.Count().Should().Be(0);
        }

        [Fact]
        public async Task GetProjectsByOwner_ReturnsOnlyOwnerProjects()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Projects.AddRange(
                new Project { Id = 1, Name = "Project A", OwnerId = 1 },
                new Project { Id = 2, Name = "Project B", OwnerId = 1 },
                new Project { Id = 3, Name = "Project C", OwnerId = 2 }
            );
            await context.SaveChangesAsync();
            var service = new ProjectService(context);

            var result = await service.GetProjectsByOwner(1);

            result.Should().HaveCount(2);
            result.Should().AllSatisfy(p => p.OwnerId.Should().Be(1));
        }
    }
}