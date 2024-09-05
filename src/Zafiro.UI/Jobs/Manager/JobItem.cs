namespace Zafiro.UI.Jobs.Manager;

public class JobItem(IJob job, JobOptions options)
{
    public string Id => Job.Id;
    public IJob Job { get; } = job;
    public JobOptions Options { get; } = options;
}