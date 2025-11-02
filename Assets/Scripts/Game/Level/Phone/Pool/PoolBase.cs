using System;
using System.Collections.Generic;
using System.Linq;
public class PoolBase<T>
{
    private readonly Func<T> _preloadFunc;
    private readonly Action<T> _returnAction;
    private Queue<T> _pool = new Queue<T>();
    private List<T> _activeContent = new List<T>();
    public IReadOnlyList<T> ActiveContent => _activeContent;
    public IReadOnlyList<T> Pool => _pool.ToList();
    public PoolBase(Func<T> preloadFunc, Action<T> returnAction, int preloadCount)
    {
        _preloadFunc = preloadFunc;
        _returnAction = returnAction;
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
        _returnAction?.Invoke(item);
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