using System.Collections.Generic;
using System.Linq;

namespace Zafiro;

public class StackingLinkedList<T>
{
    private readonly LinkedList<T> linkedList = new();

    public LinkedListNode<T> Current => linkedList.Last;
    public LinkedListNode<T> Previous => Current?.Previous;

    public T CurrentValue
    {
        get => Current.Value;
        set => Current.Value = value;
    }

    public int Count => linkedList.Count;

    public T PreviousValue
    {
        get => Previous.Value;
        set => Previous.Value = value;
    }

    public void Push(T item)
    {
        linkedList.AddLast(item);
    }

    public void Pop()
    {
        linkedList.RemoveLast();
    }

    public IList<T> ToList()
    {
        return linkedList.ToList();
    }
}