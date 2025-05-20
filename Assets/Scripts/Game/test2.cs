
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
    [SerializeField] private LocalizationString _localizationString1;
    [SerializeField] private LocalizationString _localizationString2;
    [SerializeField] private LocalizationString _localizationString3;
    private LocalizationString _localizationString4 = "Загр33";
    private LocalizationString _localizationString5 = "Загр44";
    private LocalizationString _localizationString6 = "Загр55";
    
    [Button()]
    private void test()
    {
        Debug.Log($"  {_localizationString1.DefaultText} {_localizationString1.Key}");
        Debug.Log($"  {_localizationString2.DefaultText} {_localizationString2.Key}");
        Debug.Log($"  {_localizationString3.DefaultText} {_localizationString3.Key}");
        Debug.Log($"  {_localizationString4.DefaultText} {_localizationString4.Key}");
        Debug.Log($"  {_localizationString5.DefaultText} {_localizationString5.Key}");
        Debug.Log($"  {_localizationString6.DefaultText} {_localizationString6.Key}");
    }
}