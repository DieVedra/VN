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

    [SerializeField] private string _text;
    [SerializeField] private SeriaNodeGraphsHandler _seriaForCreateFileLocalization;

    [Button()]
    private void CreateSeriaFileLocalization()
    {
        if (_seriaForCreateFileLocalization != null)
        {
            List<LocalizationString> seriaStrings = new List<LocalizationString>();
            foreach (var seria in _seriaForCreateFileLocalization.SeriaPartNodeGraphs)
            {
                for (int i = 0; i < seria.nodes.Count; i++)
                {
                    if (seria.nodes[i] is BaseNode baseNode)
                    {
                        seriaStrings.AddRange(baseNode.StringsToLocalization ?? Enumerable.Empty<LocalizationString>());
                    }
                }
            }
            CreateFile(CreateDictionary(seriaStrings));
        }
    }
    
    [Button()]
    private void Create()
    {
        CreateFile(CreateDictionary(LocalizationString.LocalizationStrings));
    }

    private void CreateFile(Dictionary<string, string> dictionary)
    {
        string path = $"{Application.dataPath}{_path}";
        string file = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
        File.WriteAllText(path, file);
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
    [Button()]
    private void Show()
    {
        
        foreach (var VARIABLE in CreateDictionary(LocalizationString.LocalizationStrings))
        {
            Debug.Log($"{VARIABLE.Key} {VARIABLE.Value}");
        }
    }
    private Dictionary<string,string> CreateDictionary(List<LocalizationString> seriaStrings)
    {
        var dict = new Dictionary<string, string>();
        foreach (var item in seriaStrings)
        {
            if (!dict.ContainsKey(item.Key))
            {
                dict.Add(item.Key, item.DefaultText);
            }
        }


        foreach (var VARIABLE in dict)
        {
            Debug.Log($" {VARIABLE.Key }  {VARIABLE.Value}");
        }
        return dict;
        
        
        // return seriaStrings.GroupBy(x => x.Key).ToDictionary(
        //     x => x.Key,
        //     x => x.First().DefaultText);
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