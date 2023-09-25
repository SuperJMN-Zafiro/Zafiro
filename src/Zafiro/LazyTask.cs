using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Zafiro;

[AsyncMethodBuilder(typeof(LazyTaskMethodBuilder<>))]
public class LazyTask<T> : INotifyCompletion
{
    private readonly object _syncObj = new object();

    private T _result;

    private Exception _exception;

    private IAsyncStateMachine _asyncStateMachine;

    private Action _continuation;

    internal LazyTask()
    {
    }

    public T GetResult()
    {
        lock (_syncObj)
        {
            if (_exception != null)
            {
                ExceptionDispatchInfo.Throw(_exception);
            }

            if (!IsCompleted)
            {
                throw new Exception("Not Completed");
            }

            return _result;
        }
    }

    public bool IsCompleted { get; private set; }

    public void OnCompleted(Action continuation)
    {
        lock (_syncObj)
        {
            if (_asyncStateMachine != null)
            {
                try
                {
                    _asyncStateMachine.MoveNext();
                }
                finally
                {
                    _asyncStateMachine = null;
                }
            }
            if (_continuation == null)
            {
                _continuation = continuation;
            }
            else
            {
                _continuation += continuation;
            }
            TryCallContinuation();
        }
    }

    public LazyTask<T> GetAwaiter() => this;

    internal void SetResult(T result)
    {
        lock (_syncObj)
        {
            _result = result;
            IsCompleted = true;
            TryCallContinuation();
        }
    }

    internal void SetException(Exception exception)
    {
        lock (_syncObj)
        {
            _exception = exception;
            IsCompleted = true;
            TryCallContinuation();
        }
    }

    internal void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        _asyncStateMachine = stateMachine;
    }

    private void TryCallContinuation()
    {
        if (IsCompleted && _continuation != null)
        {
            try
            {
                _continuation();
            }
            finally
            {
                _continuation = null;
            }
        }
    }
}