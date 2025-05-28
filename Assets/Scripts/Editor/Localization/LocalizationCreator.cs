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
using Object = UnityEngine.Object;


[CreateAssetMenu(fileName = "LocalizationCreator", menuName = "LocalizationCreator", order = 51)]
public class LocalizationCreator : ScriptableObject
{
    private const char _forging = '"';
    private const char _colon = ':';
    private string _path = "/Localization/JsonFile.json";

    [Localization, SerializeField] private string test = "test";
    [SerializeField] private string _text;

    [Button()]
    private void Create()
    {
        string path = $"{Application.dataPath}{_path}";
        string file = JsonConvert.SerializeObject(CreateDictionary(), Formatting.Indented);
        File.WriteAllText(path, file);
        AssetDatabase.Refresh();
    }

    [Button()]
    private void tst()
    {
        var a = this.FindFieldsWithAttribute<LocalizationAttribute>();
        for (int i = 0; i < a.Count; i++)
        {
            Debug.Log($"    {a[i]}");
        }
    }
    [Button()]
    private void FindAttributes()
    {
        Debug.Log($"{test} 1");
        // MonoBehaviour[] allBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();
        Object[] allBehaviours = GameObject.FindObjectsOfType<Object>();
        var result = new Dictionary<string, List<string>>();
        foreach (var behaviour in allBehaviours)
        {
            // Получаем все поля класса
            FieldInfo[] fields = behaviour.GetType().GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                // Проверяем наличие нашего атрибута
                LocalizationAttribute attribute =
                    Attribute.GetCustomAttribute(
                            field,
                            typeof(LocalizationAttribute))
                        as LocalizationAttribute;

                if (attribute != null && field.FieldType == typeof(string))
                {
                    // Debug.Log($"attribute.Key {attribute.Key}");
                    // string category = attribute.Key;
                    string fieldValue = (string) field.GetValue(behaviour);

                    // if (!result.ContainsKey(category))
                    // {
                    //     result[category] = new List<string>();
                    // }

                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        Debug.Log($"fieldValue {fieldValue}");

                        // result[category].Add(fieldValue);
                    }
                    else
                    {
                        Debug.Log($"false");

                    }
                    field.SetValue(behaviour, "test2");
                    
                }
            }
        }
        Debug.Log($"{test} 2");

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

    [Button()]
    private void CreateKeyByText()
    {
        if (string.IsNullOrEmpty(_text) == false)
        {
            Debug.Log($"{_forging}{LocalizationString.GenerateStableHash(_text)}{_forging}{_colon} {_forging}{_text}{_forging}");
        }
    }
}