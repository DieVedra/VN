
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UniRx;
using UnityEngine;

public class test : MonoBehaviour
{
    
    [Button()]
    private void test1()
    {
        Observable.Create<object>(_ =>
        {
            
            
            
            return null;
        });
    }
    
    [Button()]
    private void test2()
    {
        
    }
}