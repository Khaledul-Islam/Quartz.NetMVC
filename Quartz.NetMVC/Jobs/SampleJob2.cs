using System.Diagnostics;

namespace Quartz.NetMVC.Jobs
{
    public class SampleJob2:IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("SampleJob2 is running. at" + DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
