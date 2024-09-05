using DynamicData;

namespace Zafiro.UI.Jobs.Manager;

public interface IJobManager
{
    IObservable<IChangeSet<JobItem, string>> Tasks { get; }
    void Add(IJob job, JobOptions options);
}