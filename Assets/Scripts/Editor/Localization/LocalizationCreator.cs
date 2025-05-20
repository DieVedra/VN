using System;
using NaughtyAttributes;
using UnityEngine;
using MyNamespace;


[CreateAssetMenu(fileName = "LocalizationCreator", menuName = "LocalizationCreator", order = 51)]
public class LocalizationCreator : ScriptableObject
{
    [SerializeField] private string _path;

    [Button()]
    private void Create()
    {
        // JsonUtility.ToJson()
    }
}