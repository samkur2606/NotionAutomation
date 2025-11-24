using Hangfire;

namespace NotionAutomation.Api.Scheduling;

public static class JobScheduler
{
    public static void RegisterJobs()
    {
        RegisterTestJob();
    }

    private static void RegisterTestJob()
    {
        RecurringJob.AddOrUpdate<TestJob>(
            "TestJob",
            job => job.Run(),
            Cron.Minutely
        );
    }
}