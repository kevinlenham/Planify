using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Planify.API.Data;
using Planify.API.DTOs;
using Planify.API.Models;
using Planify.API.Services;
using FluentAssertions;
using Moq;

namespace Planify.Tests.Services
{
    public class AuthServiceTests
    {
        private PlanifyDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<PlanifyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new PlanifyDbContext(options);
        }

        private IConfiguration CreateMockConfiguration()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyForTestingPurposes123!");
            mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            return mockConfig.Object;
        }

        // -------------------------
        // Register Tests
        // -------------------------

        [Fact]
        public async Task Register_ValidDto_ReturnsAuthResponse()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AuthService(context, CreateMockConfiguration());
            var dto = new RegisterDto
            {
                FirstName = "Kevin",
                LastName = "Le",
                Email = "kevin@test.com",
                Password = "password123"
            };

            // Act
            var result = await service.Register(dto);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be("kevin@test.com");
            result.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Register_DuplicateEmail_ThrowsException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Users.Add(new User
            {
                FirstName = "Kevin",
                LastName = "Le",
                Email = "kevin@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
            });
            await context.SaveChangesAsync();
            var service = new AuthService(context, CreateMockConfiguration());
            var dto = new RegisterDto
            {
                FirstName = "Kevin",
                LastName = "Le",
                Email = "kevin@test.com",
                Password = "password123"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.Register(dto));
        }

        // -------------------------
        // Login Tests
        // -------------------------

        [Fact]
        public async Task Login_ValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Users.Add(new User
            {
                FirstName = "Kevin",
                LastName = "Le",
                Email = "kevin@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
            });
            await context.SaveChangesAsync();
            var service = new AuthService(context, CreateMockConfiguration());
            var dto = new LoginDto { Email = "kevin@test.com", Password = "password123" };

            // Act
            var result = await service.Login(dto);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be("kevin@test.com");
            result.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_WrongPassword_ThrowsException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Users.Add(new User
            {
                FirstName = "Kevin",
                LastName = "Le",
                Email = "kevin@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
            });
            await context.SaveChangesAsync();
            var service = new AuthService(context, CreateMockConfiguration());
            var dto = new LoginDto { Email = "kevin@test.com", Password = "wrongpassword" };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.Login(dto));
        }

        [Fact]
        public async Task Login_NonExistentEmail_ThrowsException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AuthService(context, CreateMockConfiguration());
            var dto = new LoginDto { Email = "nobody@test.com", Password = "password123" };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.Login(dto));
        }

        // -------------------------
        // UpdateUser Tests
        // -------------------------

        [Fact]
        public async Task UpdateUser_ValidId_UpdatesAndReturnsUser()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Users.Add(new User { Id = 1, FirstName = "Kevin", LastName = "Le", Email = "kevin@test.com", PasswordHash = "hash" });
            await context.SaveChangesAsync();
            var service = new AuthService(context, CreateMockConfiguration());
            var dto = new UpdateUserDto { FirstName = "Updated", LastName = "Name" };

            // Act
            var result = await service.UpdateUser(1, dto);

            // Assert
            result.FirstName.Should().Be("Updated");
            result.LastName.Should().Be("Name");
        }

        [Fact]
        public async Task UpdateUser_InvalidId_ThrowsException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AuthService(context, CreateMockConfiguration());
            var dto = new UpdateUserDto { FirstName = "Updated" };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.UpdateUser(999, dto));
        }

        // -------------------------
        // ChangePassword Tests
        // -------------------------

        [Fact]
        public async Task ChangePassword_ValidCredentials_UpdatesPassword()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Users.Add(new User { Id = 1, FirstName = "Kevin", LastName = "Le", Email = "kevin@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword") });
            await context.SaveChangesAsync();
            var service = new AuthService(context, CreateMockConfiguration());
            var dto = new ChangePasswordDto { CurrentPassword = "oldpassword", NewPassword = "newpassword" };

            // Act
            var result = await service.ChangeUserPassword(1, dto);

            // Assert
            BCrypt.Net.BCrypt.Verify("newpassword", result.PasswordHash).Should().BeTrue();
        }

        [Fact]
        public async Task ChangePassword_WrongCurrentPassword_ThrowsException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Users.Add(new User { Id = 1, FirstName = "Kevin", LastName = "Le", Email = "kevin@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword") });
            await context.SaveChangesAsync();
            var service = new AuthService(context, CreateMockConfiguration());
            var dto = new ChangePasswordDto { CurrentPassword = "wrongpassword", NewPassword = "newpassword" };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.ChangeUserPassword(1, dto));
        }

        // -------------------------
        // DeleteUser Tests
        // -------------------------

        [Fact]
        public async Task DeleteUser_ValidId_RemovesUser()
        {
            // Arrange
            var context = CreateInMemoryContext();
            context.Users.Add(new User { Id = 1, FirstName = "Kevin", LastName = "Le", Email = "kevin@test.com", PasswordHash = "hash" });
            await context.SaveChangesAsync();
            var service = new AuthService(context, CreateMockConfiguration());

            // Act
            await service.DeleteUser(1);

            // Assert
            context.Users.Count().Should().Be(0);
        }

        [Fact]
        public async Task DeleteUser_InvalidId_ThrowsException()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new AuthService(context, CreateMockConfiguration());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.DeleteUser(999));
        }
    }
}