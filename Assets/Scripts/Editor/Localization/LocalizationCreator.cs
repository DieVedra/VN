using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NaughtyAttributes;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;
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

    [SerializeField, Space(30f)] private string _text;
    [SerializeField] private string _key;

    [Button()]
    private void CreateSeriaFileLocalization()
    {
        List<LocalizationString> seriaStrings = new List<LocalizationString>();
        if (_seriaForCreateFileLocalization != null)
        {
            foreach (var seria in _seriaForCreateFileLocalization.SeriaPartNodeGraphs)
            {
                for (int i = 0; i < seria.nodes.Count; i++)
                {
                    if (seria.nodes[i] is ILocalizable localizable)
                    {
                        seriaStrings.AddRange(localizable.GetLocalizableContent());
                    }

                    if (seria.nodes[i] is CharacterNode characterNode)
                    {
                        if (characterNode.Characters == null)
                        {
                            Debug.Log($"characterNode null {characterNode.graph.name}");
                        }
                        foreach (var character in characterNode.Characters)
                        {
                            seriaStrings.Add(character.Name);
                        }
                    }
                }
            }
        }
        CreateFile(CreateDictionary(seriaStrings), $"{Application.dataPath}{_path}{_seriaFileLocalizationFileName}");
    }

    // [Button()]
    // private void Create()
    // {
    //     CreateFile(CreateDictionary(LocalizationString.LocalizationStrings), $"{Application.dataPath}{_path}{_fileName}");
    // }

    private void CreateFile(Dictionary<string, string> dictionary, string newPath)
    {
        string file = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
        File.WriteAllText(newPath, file);
        AssetDatabase.Refresh();
    }

    [Button()]
    private void FindAttributes()
    {
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
                    string fieldValue = (string) field.GetValue(behaviour);

                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        Debug.Log($"fieldValue {fieldValue}");
                    }
                    else
                    {
                        Debug.Log($"false");

                    }
                    field.SetValue(behaviour, "test2");
                    
                }
            }
        }
    }

    // [Button()]
    // private void Show()
    // {
    //     
    //     foreach (var VARIABLE in CreateDictionary(LocalizationString.LocalizationStrings))
    //     {
    //         Debug.Log($"{VARIABLE.Key} {VARIABLE.Value}");
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
}