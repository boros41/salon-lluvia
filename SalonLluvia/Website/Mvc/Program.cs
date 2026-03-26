using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mvc.Data.Repository;
using Mvc.Integrations.AzureBlobStorage;
using Mvc.Integrations.AzureBlobStorage.Interfaces;
using Mvc.Integrations.Calendly;
using Mvc.Models;
using Mvc.Models.Gallery;
using Mvc.Utilities;

namespace Mvc;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddIdentity<User, IdentityRole>()
                        .AddEntityFrameworkStores<SalonContext>()
                        .AddDefaultTokenProviders();

        #region Map dependencies for DI
        // repository pattern
        builder.Services.AddTransient(typeof(IRepository<Appointment>), typeof(Repository<Appointment>));
        builder.Services.AddTransient(typeof(IRepository<Client>), typeof(Repository<Client>));
        builder.Services.AddTransient(typeof(IRepository<HairStyle>), typeof(Repository<HairStyle>));
        builder.Services.AddTransient(typeof(IRepository<HairColor>), typeof(Repository<HairColor>));

        // Calendly API service
        builder.Services.AddTransient<ICalendlyAvailableDays, CalendlyAvailableDays>();
        builder.Services.AddTransient<ICalendlyAppointment, CalendlyAppointment>();

        // Azure Blob Storage API service
        builder.Services.AddTransient<IAzureBlobStorageImages, AzureBlobStorageImages>();
        #endregion

        string? connectionString = builder.Configuration.GetConnectionString("SalonContext");
        // Add EF Core DI
        builder.Services.AddDbContext<DbContext, SalonContext>(options =>
        {
            if (builder.Environment.IsDevelopment())
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                // TODO: Setup production database on deployment
                //options.UseSqlServer(connectionString);
            }
        });

        builder.Services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.AppendTrailingSlash = true;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        IServiceScopeFactory scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            await ConfigureIdentity.CreateAdminUserAsync(scope.ServiceProvider);
        }

        app.UseStatusCodePagesWithReExecute("/Error/{0}");

        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapAreaControllerRoute(
            name: "clients_appointments",
            areaName: "Admin",
            pattern: "Admin/{controller}/{action}/{id?}");

        app.MapAreaControllerRoute(
            name: "clients_list",
            areaName: "Admin",
            pattern: "Admin/{controller}/{action}");

        app.MapAreaControllerRoute(
            name: "appointments_list",
            areaName: "Admin",
            pattern: "Admin/{controller=Appointment}/{action=List}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
           .WithStaticAssets();

        app.Run();
    }
}