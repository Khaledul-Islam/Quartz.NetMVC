namespace Quartz.NetMVC.Scheduler
{
    public class SchedulerInitializer: IHostedService
    {
        private readonly IServiceProvider _provider;

        public SchedulerInitializer(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _provider.CreateScope();
            var jobScheduler = scope.ServiceProvider.GetRequiredService<JobScheduler>();
            await jobScheduler.InitializeScheduler();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Cleanup logic if needed
            return Task.CompletedTask;
        }
    }
}
