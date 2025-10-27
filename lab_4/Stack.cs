using System;
using System.Collections.Generic;

namespace lab_4;

public class StackOverflowEventArgs : EventArgs
{

    public object? Element { get; }

    public int Capacity { get; }

    public StackOverflowEventArgs(object? element, int capacity)
    {
        Element = element;
        Capacity = capacity;
    }
}

public class StackChangedEventArgs<T> : EventArgs
{
    public T Element { get; }
    public StackChangedEventArgs(T element)
    {
        Element = element;
    }
}

public class MyStack<T>
{
    private readonly LinkedList<T> _elements;
    private readonly int _capacity;

    public int Count => _elements.Count;

    public int Capacity => _capacity;

    public event EventHandler<StackOverflowEventArgs>? StackOverflow;

    public event EventHandler<StackChangedEventArgs<T>>? ElementPushed;
    public event EventHandler<StackChangedEventArgs<T>>? ElementPopped;

    public MyStack(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException("Ємність повинна бути додатним числом", nameof(capacity));
        }
        _capacity = capacity;
        _elements = new LinkedList<T>();
    }

    public void Push(T element)
    {
        // Перевірка на переповнення
        if (_elements.Count >= _capacity)
        {
            OnStackOverflow(new StackOverflowEventArgs(element, _capacity));
            return;
        }

        _elements.AddLast(element);
        ElementPushed?.Invoke(this, new StackChangedEventArgs<T>(element));
    }

    public T Pop()
    {
        if (_elements.Count == 0)
        {
            throw new InvalidOperationException("Стек порожній");
        }

        T element = _elements.Last!.Value;
        _elements.RemoveLast();
        ElementPopped?.Invoke(this, new StackChangedEventArgs<T>(element));
        return element;
    }

    public T Peek()
    {
        if (_elements.Count == 0)
        {
            throw new InvalidOperationException("Стек порожній");
        }
        return _elements.Last!.Value;
    }

    protected virtual void OnStackOverflow(StackOverflowEventArgs e)
    {
        StackOverflow?.Invoke(this, e);
    }
}