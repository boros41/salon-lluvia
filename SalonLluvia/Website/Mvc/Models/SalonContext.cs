using Microsoft.EntityFrameworkCore;

namespace Mvc.Models;

public class SalonContext : DbContext
{
    public SalonContext(DbContextOptions<SalonContext> options) : base(options) { }

    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;

    /*protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Appointment>().HasData(
            new Appointment()
            {
                Id = 1,
                ClientId = 1,
                Date = new DateTime(2025, 2, 12, 4, 30, 0),
                DesiredService = "Tinte"
            },
            new Appointment()
            {
                Id = 2,
                ClientId = 1,
                Date = new DateTime(2025, 2, 15, 10, 0, 0),
                DesiredService = "Peinado"
            },
            new Appointment()
            {
                Id = 3,
                ClientId = 2,
                Date = new DateTime(2025, 2, 17, 8, 0, 0),
                DesiredService = "Tinte"
            }
        );

        modelBuilder.Entity<Client>().HasData(
            new Client
            {
                Id = 1,
                Name = "Kevin",
                PhoneNumber = "123-456-7890"
            },
            new Client
            {
                Id = 2,
                Name = "John",
                PhoneNumber = "111-234-5678"
            }
        );
    }*/
}