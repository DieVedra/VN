
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UnityEditor;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using Zenject;

public class test2 : MonoBehaviour
{
    public int foo;
    [Button()]
    private void test()
    {
        AssetDatabase.Refresh();
    }
}