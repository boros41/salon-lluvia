using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mvc.Models.Gallery;

namespace Mvc.Models;

public class SalonContext : IdentityDbContext<User>
{
    public SalonContext(DbContextOptions<SalonContext> options) : base(options) { }

    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<Image> Images { get; set; } = null!;
    public DbSet<HairProfile> HairProfiles { get; set; } = null!;
    public DbSet<HairStyle> HairStyles { get; set; } = null!;
    public DbSet<HairColor> HairColors { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}