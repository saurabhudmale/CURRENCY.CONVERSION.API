using Quartz;

namespace CURRENCY.CONVERSION.API.Extensions
{
    public static class QuartzSchedulerExtension
    {
        public static void ScheduleJob<TJob>
        (
            this IServiceCollectionQuartzConfigurator quartz,
            string jobName,
            int intervalInMinutes
        ) where TJob : IJob
        {
            var jobKey = new JobKey(jobName);
            quartz.AddJob<TJob>(opts => opts.WithIdentity(jobKey));
            quartz.AddTrigger(opts => opts
                  .ForJob(jobKey)
                  .StartNow()
                  .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(intervalInMinutes)
                    .RepeatForever()));
        }
    }
}