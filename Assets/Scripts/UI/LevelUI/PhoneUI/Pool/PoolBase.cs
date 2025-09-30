using System;
using System.Collections.Generic;

public class PoolBase<T>
{
    private readonly Func<T> _preloadFunc;
    private Queue<T> _pool = new Queue<T>();
    private List<T> _activeContent = new List<T>();
    public PoolBase(Func<T> preloadFunc, int preloadCount)
    {
        _preloadFunc = preloadFunc;
        for (int i = 0; i < preloadCount; i++)
        {
            Return(preloadFunc());
        }
    }
    public T Get()
    {
        T item = _pool.Count > 0 ? _pool.Dequeue() : _preloadFunc();
        _activeContent.Add(item);
        return item;
    }
    public void Return(T item)
    {
        _pool.Enqueue(item);
    }
    public void ReturnAll()
    {
        foreach (T item in _activeContent)
        {
            Return(item);
        }
        _activeContent.Clear();
    }
}