using Zafiro.UI.Jobs.Execution;

namespace Zafiro.UI.Jobs;

public class Job : IJob
{
    public Job(string id, string name, object icon, IObservable<object> status, IExecution execution)
    {
        Id = id;
        Name = name;
        Icon = icon;
        Status = status;
        Execution = execution;
    }

    public string Id { get; }
    public string Name { get; }
    public object Icon { get; }
    public IObservable<object> Status { get; }
    public IExecution Execution { get; }
}