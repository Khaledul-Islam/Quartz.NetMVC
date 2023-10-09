using System.Diagnostics;

namespace Quartz.NetMVC.Jobs
{
    public class SampleJob3:IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("SampleJob3 is running. at" + DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
