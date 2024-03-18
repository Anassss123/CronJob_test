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

        // // Appel de la fonction AddRandomData
        //var randomDataGenerator = new RandomDataGenerator();
        // var randomCatAbsenceStatutGenerator = new RandomCatAbsenceStatutGenerator();
        // randomCatAbsenceStatutGenerator.AddRandomData(dbContext);
        //randomDataGenerator.AddRandomData(dbContext);

        // randomDataGenerator.AddRandomData(dbContext);
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


// public class RandomDataGenerator
// {
//     public void AddRandomData(DataContext dbContext)
//     {
//         // Generate and add random data to your DbContext here
//         // Example:
//         var random = new Random();
//         for (int i = 0; i < 10; i++)
//         {
//             Console.WriteLine("done");
//             dbContext.Cat_absence.Add(new CatAbsence
//             {
//                 Matricule = random.Next(1000, 9999),
//                 Nom = "RandomNom" + i,
//                 Prenom = "RandomPrenom" + i,
//                 DateAbsence = DateTime.Now.AddDays(-random.Next(1, 30)),
//                 AbsenceStatutId = random.Next(1, 5),
//                 LastUpdate = DateTime.Now,
//                 UpdateFlag = false,
//                 Type = "RandomType" + i
//             });
//         }
//     }
// }


// public class RandomCatAbsenceStatutGenerator
// {
//     public void AddRandomData(DataContext dbContext)
//     {
//         // Generate and add random data to your DbContext here
//         // Example:
//         var random = new Random();
//         for (int i = 0; i < 10; i++)
//         {
//             Console.WriteLine("done");
//             dbContext.Cat_absence_statut.Add(new CatAbsenceStatut
//             {
//                 Libelle = "RandomLibelle" + i
//             });
//         }
//     }
// }
