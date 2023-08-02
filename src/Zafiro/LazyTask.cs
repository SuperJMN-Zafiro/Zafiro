﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Zafiro.Zafiro;

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
        lock (this._syncObj)
        {
            if (this._exception != null)
            {
                ExceptionDispatchInfo.Throw(this._exception);
            }

            if (!this.IsCompleted)
            {
                throw new Exception("Not Completed");
            }

            return this._result;
        }
    }

    public bool IsCompleted { get; private set; }

    public void OnCompleted(Action continuation)
    {
        lock (this._syncObj)
        {
            if (this._asyncStateMachine != null)
            {
                try
                {
                    this._asyncStateMachine.MoveNext();
                }
                finally
                {
                    this._asyncStateMachine = null;
                }
            }
            if (this._continuation == null)
            {
                this._continuation = continuation;
            }
            else
            {
                this._continuation += continuation;
            }
            this.TryCallContinuation();
        }
    }

    public LazyTask<T> GetAwaiter() => this;

    internal void SetResult(T result)
    {
        lock (this._syncObj)
        {
            this._result = result;
            this.IsCompleted = true;
            this.TryCallContinuation();
        }
    }

    internal void SetException(Exception exception)
    {
        lock (this._syncObj)
        {
            this._exception = exception;
            this.IsCompleted = true;
            this.TryCallContinuation();
        }
    }

    internal void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        this._asyncStateMachine = stateMachine;
    }

    private void TryCallContinuation()
    {
        if (this.IsCompleted && this._continuation != null)
        {
            try
            {
                this._continuation();
            }
            finally
            {
                this._continuation = null;
            }
        }
    }
}