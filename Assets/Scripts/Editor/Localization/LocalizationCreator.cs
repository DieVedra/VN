using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NaughtyAttributes;
using UnityEngine;
using MyNamespace;
using Newtonsoft.Json;
using UnityEditor;


[CreateAssetMenu(fileName = "LocalizationCreator", menuName = "LocalizationCreator", order = 51)]
public class LocalizationCreator : ScriptableObject
{
    private string _path = "/Localization/JsonFile.json";

    [Button()]
    private void Create()
    {
        string path = $"{Application.dataPath}{_path}";
        string file = JsonConvert.SerializeObject(CreateDictionary(), Formatting.Indented);
        File.WriteAllText(path, file);
        AssetDatabase.Refresh();
    }

    // [Button()]
    // private void FindFields()
    // {
    //     var a = AppDomain.CurrentDomain.GetAssemblies();
    //     foreach (var VARIABLE in a)
    //     {
    //         if (VARIABLE.FullName.StartsWith("System.") ||
    //             VARIABLE.FullName.StartsWith("UnityEngine.") ||
    //             VARIABLE.FullName.StartsWith("Unity.") ||
    //             VARIABLE.FullName.StartsWith("mscorlib"))
    //         {
    //             continue;
    //         }
    //
    //
    //         try
    //         {
    //             Debug.Log($"try");
    //
    //             Type type;
    //             for (int i = 0; i < a.Length; i++)
    //             {
    //
    //                 type = a[i].GetType();
    //                 var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    //                 foreach (var field in fields)
    //                 {
    //                     var res = field.GetCustomAttributes(typeof(LocalizationAttribute), true);
    //                     if (res.Length > 0)
    //                     {
    //                         
    //                         Debug.Log($"{type.Name}   {field.Name}");
    //                     }
    //                 }
    //
    //             }
    //         }
    //         catch (ReflectionTypeLoadException)
    //         {
    //             continue;
    //         }
    //     }
    // }
    [Button()]
    private void Show()
    {
        
        foreach (var VARIABLE in CreateDictionary())
        {
            Debug.Log($"{VARIABLE.Key} {VARIABLE.Value}");
        }
    }

    private Dictionary<string,string> CreateDictionary()
    {
        return LocalizationString.LocalizationStrings.Distinct().ToDictionary(
            x => x.Key,
            x => x.DefaultText);
    }
}