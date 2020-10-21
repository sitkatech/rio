using Hangfire;

namespace Rio.API
{
    public interface IBeehiveSyncJob
    {
        void RunJob(IJobCancellationToken token);
    }
}