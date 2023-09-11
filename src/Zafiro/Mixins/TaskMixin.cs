using System;
using System.Reactive;
using System.Threading.Tasks;

namespace Zafiro.Mixins;

public static class TaskMixin
{
    public static async Task<Unit> ToSignal(this Func<Task> task)
    {
        await task();
        return Unit.Default;
    }
}