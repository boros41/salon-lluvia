using Microsoft.EntityFrameworkCore;
using Mvc.Models;

namespace Mvc;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        string? connectionString = builder.Configuration.GetConnectionString("SalonContext");
        // Add EF Core DI
        builder.Services.AddDbContext<SalonContext>(options =>
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

        app.UseStatusCodePagesWithReExecute("/Error/{0}");

        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapAreaControllerRoute(
            name: "edit_client_appointment",
            areaName: "Admin",
            pattern: "Admin/{controller}/{action}/{appointmentId?}");

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