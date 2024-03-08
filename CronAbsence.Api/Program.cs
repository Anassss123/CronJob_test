using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CronAbsence.Api.Schedule;
using CronAbsence.Api.Service;
using CronAbsence.Infrastructure.Excel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddLogging();
builder.Services.AddScoped<IScheduleHandler, ScheduleHandler>();
builder.Services.AddScoped<IExcelReaderService, ExcelReaderService>();


// Add hosted service
builder.Services.AddSingleton<IHostedService, ScheduleJobService>();

var app = builder.Build();

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
