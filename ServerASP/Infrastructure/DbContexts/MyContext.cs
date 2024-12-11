using Microsoft.EntityFrameworkCore;
using ServerASP.Infrastructure.DbModels;

namespace ServerASP.Infrastructure.DbContexts
{
    public class MyContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Status> Statuses { get; set; }

        public MyContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("FileName=MyDatabase.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Status)
                .WithMany(s => s.Employees)
                .HasForeignKey(e => e.StatusId);

            modelBuilder.Entity<Status>().HasData(
                new Status { Id = 1, Description = "Активен" },
                new Status { Id = 99, Description = "Удалён" });

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "Игорь", Surename = "Тальков", Age = 22,StatusId = 1}, 
                new Employee { Id = 2, Name = "Михаил", Surename = "Круг", StatusId = 1});
        }
    }
}
