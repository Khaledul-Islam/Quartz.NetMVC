using Microsoft.EntityFrameworkCore;
using Quartz.Impl.Matchers;
using Quartz.NetMVC.Data;
using Quartz.NetMVC.Jobs;
using Quartz.NetMVC.Models;
using System.Globalization;

namespace Quartz.NetMVC.Scheduler
{
    public class JobScheduler
    {
        private readonly IScheduler _scheduler;
        private readonly ApplicationDbContext _dbContext;//replace your IRepo here

        public JobScheduler(IScheduler scheduler, ApplicationDbContext dbContext)
        {
            _scheduler = scheduler;
            _dbContext = dbContext;
        }

        public async Task InitializeScheduler()
        {
            await _scheduler.Start();
            await ScheduleJobsFromDatabase();
        }

        public async Task ScheduleJobsFromDatabase()
        {
            var dbSchedules = await _dbContext.Schedules.ToListAsync();

            foreach (var dbSchedule in dbSchedules)
                if (dbSchedule.IsActive)
                    await ScheduleJob(dbSchedule);
                else
                    await UnScheduleJob(dbSchedule);
        }
        public async Task ScheduleJob(Schedule dbSchedule)
        {
            var jobDetail = JobBuilder.Create(GetJobType(dbSchedule.JobName))
                .WithIdentity(dbSchedule.JobName, dbSchedule.JobGroup)
                .DisallowConcurrentExecution()
                .Build();

            var cronBuilder = CronScheduleBuilder
                .CronSchedule(dbSchedule.CronSchedule)
                .InTimeZone(TimeZoneInfo.Local)
                .WithMisfireHandlingInstructionFireAndProceed();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{dbSchedule.JobName}-T", dbSchedule.JobGroup)
                .WithSchedule(cronBuilder)
                //.WithSimpleSchedule(x=>x.WithIntervalInSeconds(5).RepeatForever())
                .Build();


            await _scheduler.ScheduleJob(jobDetail, trigger);
        }
        public async Task ExecuteJobNow(Schedule entity)
        {
            var existingJobKey = new JobKey(entity.JobIdentity, entity.JobGroup);

            if (await _scheduler.CheckExists(existingJobKey))
                await _scheduler.TriggerJob(existingJobKey);
            else
            {
                var jobDetail = JobBuilder.Create(GetJobType(entity.JobName))
                    .WithIdentity(entity.JobIdentity, entity.JobGroup)
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .StartNow()
                    .Build();

                await _scheduler.ScheduleJob(jobDetail, trigger);
            }
        }

        public async Task RescheduleJob(string jobIdentity, string jobGroup, string newSchedule)
        {
            var existingTriggerKey = new TriggerKey(jobIdentity);
            var existingTrigger = await _scheduler.GetTrigger(existingTriggerKey);

            if (existingTrigger != null)
            {
                var newTrigger = TriggerBuilder.Create()
                    .WithIdentity(jobIdentity, jobGroup)
                    .WithCronSchedule(newSchedule)
                    .Build();

                await _scheduler.RescheduleJob(existingTriggerKey, newTrigger);
            }
        }

        public async Task UnScheduleJob(Schedule entity)
        {
            var triggerKey = new TriggerKey(entity.JobIdentity, entity.JobGroup);
            await _scheduler.UnscheduleJob(triggerKey);
        }

        public async Task<List<JobInformation>> GetAllJobs()
        {
            var jobGroups = await _scheduler.GetJobGroupNames();
            var jobInfoList = new List<JobInformation>();
            foreach (var group in jobGroups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeys = await _scheduler.GetJobKeys(groupMatcher);
                foreach (var jobKey in jobKeys)
                {
                    var detail = await _scheduler.GetJobDetail(jobKey);
                    var triggers = await _scheduler.GetTriggersOfJob(jobKey);
                    foreach (var trigger in triggers)
                    {
                        var jobInfo = new JobInformation
                        {
                            GroupName = group,
                            JobKeyName = jobKey.Name,
                            JobDescription = detail?.Description ?? "Not Found",
                            TriggerKeyName = trigger.Key.Name,
                            TriggerKeyGroup = trigger.Key.Group,
                            TriggerType = trigger.GetType().Name,
                            TriggerState = _scheduler.GetTriggerState(trigger.Key)
                        };
                        var nextFireTime = trigger.GetNextFireTimeUtc();//
                        if (nextFireTime.HasValue) jobInfo.NextFireTime = nextFireTime.Value.LocalDateTime.ToString(CultureInfo.InvariantCulture);
                        var previousFireTime = trigger.GetPreviousFireTimeUtc();
                        if (previousFireTime.HasValue) jobInfo.PreviousFireTime = previousFireTime.Value.LocalDateTime.ToString(CultureInfo.InvariantCulture);
                        jobInfoList.Add(jobInfo);
                    }
                }
            }

            return jobInfoList;
        }

        private Type GetJobType(string jobType)
        {
            switch (jobType)
            {
                case "SampleJob1":
                    return typeof(SampleJob1);
                case "SampleJob2":
                    return typeof(SampleJob2);
                case "SampleJob3":
                    return typeof(SampleJob3);
                default:
                    throw new ArgumentException("Invalid job type specified.");
            }
        }


    }
}
