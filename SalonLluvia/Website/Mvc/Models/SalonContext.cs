using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mvc.Models;

public class SalonContext : IdentityDbContext<User>
{
    public SalonContext(DbContextOptions<SalonContext> options) : base(options) { }

    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}