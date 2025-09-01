
using EventExtension.Services.Interfaces;

namespace EventExtension.Services
{
    public class DailyEventCacheRefresh : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public DailyEventCacheRefresh(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                    await eventService.RefreshEvents(); 
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
