using CronAbsence.Api.Service;

namespace CronAbsence.Api.Schedule
{
    public class ScheduleJobService : BackgroundService
    {
        private readonly ILogger<ScheduleJobService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ScheduleJobService(ILogger<ScheduleJobService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try 
            {
                _logger.LogInformation("ScheduleJobService is starting.");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var scheduleHandler = scope.ServiceProvider.GetRequiredService<IScheduleHandler>();
                    await scheduleHandler.ProcessAsync();
                }

                _logger.LogInformation("ScheduleJobService completed successfully.");
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
