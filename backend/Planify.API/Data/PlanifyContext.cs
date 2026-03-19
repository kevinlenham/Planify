using Microsoft.EntityFrameworkCore;
using Planify.API.Models;

namespace Planify.API.Data
{
    public class PlanifyDbContext : DbContext
    {
        public PlanifyDbContext(DbContextOptions<PlanifyDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }

    }
}