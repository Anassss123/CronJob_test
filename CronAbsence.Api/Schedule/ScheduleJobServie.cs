using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CronAbsence.Api.Service;

namespace CronAbsence.Api.Schedule
{
    public class ScheduleJobService : BackgroundService
    {
        private readonly ILogger<ScheduleJobService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public ScheduleJobService(ILogger<ScheduleJobService> logger, IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartProcessAsync(stoppingToken);
        }

        private async Task StartProcessAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("ScheduleJobService is starting.");

                while (!stoppingToken.IsCancellationRequested)
                {
                    // Resolve the scoped service within the method where it's needed
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var scheduleHandler = scope.ServiceProvider.GetRequiredService<IScheduleHandler>();
                        await scheduleHandler.ProcessAsync();
                    }

                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ScheduleJobService.");
            }
            finally
            {
                _logger.LogInformation("ScheduleJobService is stopping.");
            }
        }
    }
}
