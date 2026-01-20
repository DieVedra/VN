
using System;
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
    private const float _panelWidthMin = 350f;
    private const float _panelWidthMax = 500f;
    [SerializeField] private RectTransform _rectTransform;
    [Button()]
    private void test1()
    {
        Debug.Log($"test1: {_rectTransform.anchorMax} {_rectTransform.anchorMin} {_rectTransform.offsetMax} {_rectTransform.offsetMin}");
    }
    [Button()]
    private void test2()
    {
        Debug.Log($"test2: {_rectTransform.anchoredPosition}");
        Debug.Log($"test2: {_rectTransform.sizeDelta.x}   {_rectTransform.sizeDelta.y}");
        
        // var a = Mathf.InverseLerp()
        // var b = Mathf.Lerp(_panelWidthMin, _panelWidthMax, 0f);

    }
}