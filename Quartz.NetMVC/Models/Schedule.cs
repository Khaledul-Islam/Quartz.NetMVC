using System.ComponentModel.DataAnnotations;

namespace Quartz.NetMVC.Models
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }

        public string JobName { get; set; } = string.Empty;
        public string JobGroup { get; set; } = string.Empty;
        public string JobIdentity { get; set; } = string.Empty;
        public string CronSchedule { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool StopOnError { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime ExecutionDate { get; set; }
    }
}
