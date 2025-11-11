
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class test : MonoBehaviour
{
    [SerializeField] private bool a;
    [SerializeField] private bool b;
    [Button()]
    private void test1()
    {
        var res = a && b;
        Debug.Log($"{res}");
    }
    
    [Button()]
    private void test2()
    {
        
    }
    [Button()]
    private void test3()
    {
        
    }
    [Button()]
    private void test4()
    {
        
    }
}