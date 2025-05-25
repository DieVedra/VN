using System;
using System.IO;
using NaughtyAttributes;
using UnityEngine;
using MyNamespace;
using UnityEditor;


[CreateAssetMenu(fileName = "LocalizationCreator", menuName = "LocalizationCreator", order = 51)]
public class LocalizationCreator : ScriptableObject
{
    [SerializeField] private string _path;

    [Button()]
    private void Create()
    {
        // string path = Path.Combine(Application.dataPath, "testfile.json");
        string path = $"{Application.dataPath}{_path}/testfile.json";
        Debug.Log($"{path}");

        // JsonUtility.ToJson()
        // string file = JsonUtility.ToJson(LocalizationString.LocalizationStringDictionary);
        // string file = JsonConvert. (LocalizationString.LocalizationStringDictionary);
        // string file = JsonUtility.ToJson("LocalizationString.LocalizationStringDictionary");
        // Debug.Log($"{file}");
        // File.WriteAllText(path, file);
        // AssetDatabase.Refresh();

    }
    
    [Button()]
    private void Show()
    {
        foreach (var VARIABLE in LocalizationString.LocalizationStringDictionary)
        {
            Debug.Log($"{VARIABLE.Key} {VARIABLE.Value}");
        }
    }
}