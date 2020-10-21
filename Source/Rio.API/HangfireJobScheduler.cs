using Hangfire;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rio.API
{
    public class HangfireJobScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            var recurringJobIds = new List<string>();

            //AddRecurringJob<BeehiveSyncJob>(BeehiveSyncJob.JobName, x => x.RunJob(Null), Cron.Daily(8, 30), recurringJobIds);

            // Remove any jobs we haven't explicitly scheduled
            RemoveExtraneousJobs(recurringJobIds);
        }

        private static void AddRecurringJob<T>(string jobName, Expression<Action<T>> methodCallExpression,
            string cronExpression, ICollection<string> recurringJobIds)
        {
            RecurringJob.AddOrUpdate<T>(jobName, methodCallExpression, cronExpression);
            recurringJobIds.Add(jobName);
        }

        private static void RemoveExtraneousJobs(List<string> recurringJobIds)
        {
            using var connection = JobStorage.Current.GetConnection();
            var recurringJobs = connection.GetRecurringJobs();
            var jobsToRemove = recurringJobs.Where(x => !recurringJobIds.Contains(x.Id)).ToList();
            foreach (var job in jobsToRemove)
            {
                RecurringJob.RemoveIfExists(job.Id);
            }
        }
    }
}