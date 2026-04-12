using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private List<int> operations;
    [SerializeField] private int _taskCountMax;
    [Button()]
    private void test1()
    {
        AddToQueue(operations);
    }
    
    [Button()]
    private void test2()
    {
        
    }
    
    public void AddToQueue(List<int> operations, CancellationTokenSource cancellationToken = null)
    {
        if (operations.Count > _taskCountMax)
        {
            int iterations = operations.Count / _taskCountMax;
            int minIndex = iterations * _taskCountMax;
            int maxIndex = operations.Count - 1;
            for (int i = 0; i < iterations; i++)
            {
                Debug.Log($"minIndex: {minIndex}     maxIndex: {maxIndex}");

                var list = new List<int>();
                for (int j = maxIndex; j >= minIndex; j--)
                {
                    Debug.Log($"index: {j}");

                    list.Add(operations[j]);
                    operations.RemoveAt(j);
                }
                Debug.Log($"Added: {list.Count}");
                maxIndex = minIndex - 1;
                minIndex -= _taskCountMax;
            }
        }
        Debug.Log($" last Added: {operations.Count}");
    }
}