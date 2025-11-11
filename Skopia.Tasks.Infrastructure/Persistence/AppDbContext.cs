using Microsoft.EntityFrameworkCore;
using Skopia.Tasks.Domain.Entities;

namespace Skopia.Tasks.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<TaskComment> TaskComments => Set<TaskComment>();
        public DbSet<TaskHistory> TaskHistories => Set<TaskHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table names
            modelBuilder.Entity<Project>().ToTable("Projects");
            modelBuilder.Entity<TaskItem>().ToTable("Tasks");
            modelBuilder.Entity<TaskComment>().ToTable("TaskComments");
            modelBuilder.Entity<TaskHistory>().ToTable("TaskHistories");

            // Relationships
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasMany(t => t.Comments)
                .WithOne(c => c.TaskItem)
                .HasForeignKey(c => c.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasMany(t => t.History)
                .WithOne(h => h.TaskItem)
                .HasForeignKey(h => h.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
