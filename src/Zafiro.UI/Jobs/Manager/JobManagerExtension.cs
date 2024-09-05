using DynamicData;

namespace Zafiro.UI.Jobs.Manager;

public static class JobManagerExtension
{
    public static bool IsTransient(this JobItem jobItem)
    {
        return jobItem.Options.RemoveOnCompleted || jobItem.Options.RemoveOnCompleted;
    }

    public static IObservable<IChangeSet<JobItem, string>> Transient(this IObservable<IChangeSet<JobItem, string>> tasks)
    {
        return tasks.Filter(x => x.IsTransient());
    }

    public static IObservable<IChangeSet<JobItem, string>> Permanent(this IObservable<IChangeSet<JobItem, string>> tasks)
    {
        return tasks.Filter(x => !x.IsTransient());
    }
}