using Quartz.Spi;

namespace Quartz.NetMVC.Factory
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob ?? throw new InvalidOperationException();
        }

        public void ReturnJob(IJob job)
        {
            // Dispose or release resources if necessary
        }
    }
}
