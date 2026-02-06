using XNode;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class MergedNodeSharedStorage
{
    public readonly Dictionary<Type, Node> MergerObjects;
    public readonly List<UniTask> TaskList;

    public MergedNodeSharedStorage()
    {
        MergerObjects = new Dictionary<Type, Node>();
        TaskList = new List<UniTask>();
    }

    public void Clear()
    {
        MergerObjects.Clear();
        TaskList.Clear();
    }
}