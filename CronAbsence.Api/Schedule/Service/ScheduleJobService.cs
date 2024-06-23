using CronAbsence.Api.Schedule.Interface;

namespace CronAbsence.Api.Schedule
{
    public class ScheduleJobService : BackgroundService
    {
        private readonly ILogger<ScheduleJobService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScheduleJobService(ILogger<ScheduleJobService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try 
            {
                _logger.LogInformation("ScheduleJobService is starting.");

                using (var scope = _serviceScopeFactory.CreateScope())
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
