﻿namespace Zafiro;

public interface IAdd<in T>
{
    void Add(T item);
}