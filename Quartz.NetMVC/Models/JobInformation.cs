namespace Quartz.NetMVC.Models
{
    public class JobInformation
    {
        public string GroupName { get; set; }
        public string JobDescription { get; set; }
        public string JobKeyName { get; set; }
        public string TriggerKeyName { get; set; }
        public string TriggerKeyGroup { get; set; }
        public string TriggerType { get; set; }
        public Task<TriggerState> TriggerState { get; set; }
        public string NextFireTime { get; set; }
        public string PreviousFireTime { get; set; }
    }
}
