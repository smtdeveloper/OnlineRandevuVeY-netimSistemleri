using Entities.Model;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

public class AppointmentDbContext : DbContext
{
    public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<User> Users { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Service>().HasData(
            new Service { Id = Guid.Parse("982dbc65-d3f1-4b3c-8c25-96d688bf5301"), Name = "Egzoz Gazı Ölçümü", CreatedDate = DateTime.Parse("2024-11-14 23:38:47.8239553") },
            new Service { Id = Guid.Parse("04d0608d-f2c9-4044-b4d9-31c5733da48e"), Name = "Fren Testi", CreatedDate = DateTime.Parse("2024-11-14 23:38:47.8239556") },
            new Service { Id = Guid.Parse("cff66929-3faf-42fb-ac5e-63d413777d64"), Name = "Far Ayarı", CreatedDate = DateTime.Parse("2024-11-14 23:38:47.8239558") }
        );

        base.OnModelCreating(modelBuilder);
    }
}