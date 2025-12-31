using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NaughtyAttributes;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;
using XNode;
using Object = UnityEngine.Object;


[CreateAssetMenu(fileName = "LocalizationCreator", menuName = "LocalizationCreator", order = 51)]
public class LocalizationCreator : ScriptableObject
{
    private const char _forging = '"';
    private const char _colon = ':';
    private const string _path = "/Localization";
    private const string _fileName = "/JsonFile.json";
    private const string _seriaFileLocalizationFileName = "/FileLocalizationSeria.json";

        
    [SerializeField] private SeriaNodeGraphsHandler _seriaForCreateFileLocalization;
    [SerializeField] private PhoneContactsProvider _phoneContactsProvider;
    [SerializeField, Space(30f)] private string _text;
    [SerializeField] private string _key;
    // [SerializeField] private TextAsset jsonFile;
    [Button()]
    private void CreateSeriaFileLocalization()
    {
        List<LocalizationString> seriaStrings = new List<LocalizationString>();
        if (_seriaForCreateFileLocalization != null)
        {
            foreach (var seria in _seriaForCreateFileLocalization.SeriaPartNodeGraphs)
            {
                foreach (var t in seria.nodes)
                {
                    if (t is ILocalizable localizable)
                    {
                        seriaStrings.AddRange(localizable.GetLocalizableContent());
                    }
                }
            }
        }

        if (_phoneContactsProvider != null)
        {
            foreach (var t in _phoneContactsProvider.PhoneContacts)
            {
                seriaStrings.Add(t.NameLocalizationString);
            }
        }
        
        CreateFile(CreateDictionary(seriaStrings), $"{Application.dataPath}{_path}{_seriaFileLocalizationFileName}");
    }

    private void CreateFile(Dictionary<string, string> dictionary, string newPath)
    {
        string file = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
        File.WriteAllText(newPath, file);
        Debug.Log($"Localization File Created: {newPath}");
        AssetDatabase.Refresh();
    }

    // [Button()]
    // private void Remove()
    // {
    //     Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonFile.text);
    //     Debug.Log($"{result.Count}");
    //     foreach (var VARIABLE in result)
    //     {
    //         Debug.Log($"{VARIABLE.Key}   {VARIABLE.Value}");
    //
    //     }
    //     Debug.Log($"-------------");
    //
    //     foreach (var graph in _seriaForCreateFileLocalization.SeriaPartNodeGraphs)
    //     {
    //         foreach (var node in graph.nodes)
    //         {
    //             if (node is ChoiceNode choiceNode)
    //             {
    //                 Debug.Log($"index: {graph.nodes.IndexOf(node)}");
    //                 var a = choiceNode.GetLocalizableContent();
    //                 foreach (var LS in choiceNode.GetLocalizableContent())
    //                 {
    //                     if (string.IsNullOrEmpty(LS.Key) == false)
    //                     {
    //                         if (string.IsNullOrEmpty(LS.DefaultText))
    //                         {
    //                             Debug.Log($"LS.DefaultText Empty   Key{LS.Key}");
    //                             if (result.TryGetValue(LS.Key, out string txt))
    //                             {
    //                                 LS.SetText(txt);
    //                                 Debug.Log($"{LS.DefaultText}");
    //
    //                             }
    //                             EditorUtility.SetDirty(choiceNode);
    //                         }
    //                     }
    //                     else
    //                     {
    //                         Debug.Log($"LS.Key Empty");
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }
    // [Button()]
    // private void FindAttributes()
    // {
    //     // MonoBehaviour[] allBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();
    //     Object[] allBehaviours = GameObject.FindObjectsOfType<Object>();
    //     var result = new Dictionary<string, List<string>>();
    //     foreach (var behaviour in allBehaviours)
    //     {
    //         // Получаем все поля класса
    //         FieldInfo[] fields = behaviour.GetType().GetFields(
    //             BindingFlags.Public |
    //             BindingFlags.NonPublic |
    //             BindingFlags.Instance);
    //
    //         foreach (FieldInfo field in fields)
    //         {
    //             // Проверяем наличие нашего атрибута
    //             LocalizationAttribute attribute =
    //                 Attribute.GetCustomAttribute(
    //                         field,
    //                         typeof(LocalizationAttribute))
    //                     as LocalizationAttribute;
    //
    //             if (attribute != null && field.FieldType == typeof(string))
    //             {
    //                 string fieldValue = (string) field.GetValue(behaviour);
    //
    //                 if (!string.IsNullOrEmpty(fieldValue))
    //                 {
    //                     Debug.Log($"fieldValue {fieldValue}");
    //                 }
    //                 else
    //                 {
    //                     Debug.Log($"false");
    //
    //                 }
    //                 field.SetValue(behaviour, "test2");
    //                 
    //             }
    //         }
    //     }
    // }

    // [Button()]
    // private void Show()
    // {
    //     
    //     foreach (var VARIABLE in CreateDictionary(LocalizationString.LocalizationStrings))
    //     {
    //         Debug.Log($"{VARIABLE.NameKey} {VARIABLE.Value}");
    //     }
    // }


    private Dictionary<string,string> CreateDictionary(List<LocalizationString> seriaStrings)
    {
        var dict = new Dictionary<string, string>();
        foreach (var item in seriaStrings)
        {
            if (item != null)
            {
                if (item.TryRegenerateKey())
                {
                    if (!dict.ContainsKey(item.Key))
                    {
                        dict.Add(item.Key, item.DefaultText);
                    }
                }
            }
        }


        foreach (var VARIABLE in dict)
        {
            Debug.Log($" {VARIABLE.Key }  {VARIABLE.Value}");
        }
        return dict;
    }

    [Button()]
    private void CreateKeyByText()
    {
        if (string.IsNullOrEmpty(_text) == false)
        {
            string key = LocalizationString.GenerateStableHash(_text);
            _key = key;
            Debug.Log($"{_forging}{key}{_forging}{_colon} {_forging}{_text}{_forging}");
        }
    }

    [Button()]
    private void Clear()
    {
        _seriaForCreateFileLocalization = null;
        _phoneContactsProvider = null;
    }
}