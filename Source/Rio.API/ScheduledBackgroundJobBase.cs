using System;
using System.Collections.Generic;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rio.EFModels.Entities;

namespace Rio.API
{
    public abstract class ScheduledBackgroundJobBase<T>
    {
        /// <summary>
        /// A safety guard to ensure only one job is running at a time, some jobs seem like they would collide if allowed to run concurrently or possibly drag the server down.
        /// </summary>
        private static readonly object ScheduledBackgroundJobLock = new object();

        private readonly string _jobName;
        protected readonly ILogger<T> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly RioDbContext _rioDbContext;

        /// <summary> 
        /// Jobs must have a proscribed environment to run in (for example, to prevent a job that makes a lot of calls to an external API from accidentally DOSing that API by running on all local boxes, QA, and Prod at the same time.
        /// </summary>
        public abstract List<RunEnvironment> RunEnvironments { get; }

        protected ScheduledBackgroundJobBase(string jobName, ILogger<T> logger, IWebHostEnvironment webHostEnvironment, RioDbContext rioDbContext)
        {
            _jobName = jobName;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _rioDbContext = rioDbContext;
        }

        /// <summary>
        /// This wraps the call to <see cref="RunJobImplementation"/> with all of the housekeeping for being a scheduled job.
        /// </summary>
        public void RunJob(IJobCancellationToken token)
        {
            RunJob(token, null);
        }

        /// <summary>
        /// This wraps the call to <see cref="RunJobImplementation"/> with all of the housekeeping for being a scheduled job.
        /// </summary>
        public void RunJob(IJobCancellationToken token, string additionalArguments)
        {
            lock (ScheduledBackgroundJobLock)
            {
                // No-Op if we're not running in an allowed environment
                if (_webHostEnvironment.IsDevelopment() && !RunEnvironments.Contains(RunEnvironment.Development))
                {
                    return;
                }
                if (_webHostEnvironment.IsStaging() && !RunEnvironments.Contains(RunEnvironment.Staging))
                {
                    return;
                }
                if (_webHostEnvironment.IsProduction() && !RunEnvironments.Contains(RunEnvironment.Production))
                {
                    return;
                }

                token.ThrowIfCancellationRequested();

                try
                {
                    _logger.LogInformation($"Begin Job {_jobName}");
                    if (!string.IsNullOrEmpty(additionalArguments))
                    {
                        RunJobImplementation(additionalArguments);
                    }
                    else
                    {
                        RunJobImplementation();
                    }
                    _logger.LogInformation($"End Job {_jobName}");
                }
                catch (Exception ex)
                {
                    // Wrap and rethrow with the information about which job encountered the problem
                    _logger.LogError(ex.Message);
                    throw new ScheduledBackgroundJobException(_jobName, ex);
                }
            }
        }

        /// <summary>
        /// Jobs can fill this in with whatever they need to run. This is called by <see cref="RunJob"/> which handles other miscellaneous stuff
        /// </summary>
        protected abstract void RunJobImplementation();

        protected abstract void RunJobImplementation(string additionalArguments);
    }
}