using System.Reactive.Subjects;

namespace Zafiro.Core
{
    public interface IOperationProgress
    {
        ISubject<double> Percentage { get; set; }
        ISubject<long> Value { get; set; }
    }
}