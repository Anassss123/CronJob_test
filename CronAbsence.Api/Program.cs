using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CronAbsence.Infrastructure.Configuration;
using CronAbsence.Api.Service;
using CronAbsence.Infrastructure.Service.Excel;
using CronAbsence.Api.Schedule;
using CronAbsence.Domain.Models;
using CronAbsence.Infrastructure.Service.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddLogging();
builder.Services.AddScoped<IScheduleHandler, ScheduleHandler>();
builder.Services.AddScoped<IExcelReaderService, ExcelReaderService>();
builder.Services.AddScoped<IDatabaseReaderService, DatabaseReaderService>();

// Add hosted service
builder.Services.AddSingleton<IHostedService, ScheduleJobService>();

// Configure Database SqlServer
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Attempt to connect to the database and print connection status
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<DataContext>();
        dbContext.Database.EnsureCreated(); // Ensures that the database exists. Note: This method is typically used for development and not recommended for production.
        Console.WriteLine("Connected successfully to the database.");
        dbContext.SaveChanges();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to connect to the database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
