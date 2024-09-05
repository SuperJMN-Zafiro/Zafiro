using System.Reactive.Concurrency;

namespace Zafiro.UI.Jobs.Manager;

public class JobOptions
{
    public bool AutoStart { get; init; }
    public bool RemoveOnCompleted { get; init; }
    public bool RemoveOnStopped { get; init; }

    public TimeSpan RemovalDelay { get; set; } = TimeSpan.FromSeconds(5);
    
    public IScheduler DelayScheduler { get; set; } = Scheduler.Default;
}