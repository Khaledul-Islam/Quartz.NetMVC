using System.Diagnostics;

namespace Quartz.NetMVC.Jobs
{
    public class SampleJob1:IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("SampleJob1 is running. at" + DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
