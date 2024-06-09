using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CronAbsence.Infrastructure.Configuration;
using CronAbsence.Api.Service;
using CronAbsence.Infrastructure.Service.Excel;
using CronAbsence.Api.Schedule;
using CronAbsence.Domain.Models;
using CronAbsence.Infrastructure.Service.Data;
using CronAbsence.Infrastructure.Service.Process;
using System;
using CronAbsence.Domain.Interfaces;
using CronAbsence.Infrastructure.Interfaces;
using CronAbsence.Api.Schedule.Interface;
using CronAbsence.Domain.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddLogging();

// Add DbContext
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add scoped services
builder.Services.AddScoped<IDatabaseReaderService, DatabaseReaderService>();
builder.Services.AddScoped<IScheduleHandler, ScheduleHandler>();
builder.Services.AddScoped<IExcelReaderService, ExcelReaderService>();
builder.Services.AddScoped<IDataComparer, DataComparer>();
builder.Services.AddScoped<IFtpService, FtpService>();
builder.Services.AddScoped<ILoggerService, LoggerService>();
builder.Services.AddScoped<IDataConverter, DataConverter>();

// Add ScheduleJobService as a singleton
builder.Services.AddSingleton<ScheduleJobService>(); 

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

var scheduleJobService = app.Services.GetRequiredService<ScheduleJobService>();

lifetime.ApplicationStarted.Register(() =>
{
    scheduleJobService.StartAsync(default);
});

lifetime.ApplicationStopping.Register(() =>
{
    scheduleJobService.StopAsync(default);
});

app.Run();
