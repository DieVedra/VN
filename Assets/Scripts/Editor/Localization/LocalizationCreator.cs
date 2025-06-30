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
    private string _path = "/Localization";
    private string _fileName = "/JsonFile.json";

    [SerializeField] private SeriaNodeGraphsHandler _seriaForCreateFileLocalization;
    [SerializeField] private SeriaStatProvider _seriaStatProvider;
    [SerializeField] private CharactersData _charactersData;
    [SerializeField] private List<CustomizableCharacter> _customizableCharacters;

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
                    if (seria.nodes[i] is BaseNode baseNode)
                    {
                        seriaStrings.AddRange(baseNode.StringsLocalization ?? Enumerable.Empty<LocalizationString>());
                    }
                }
            }
        }

        if (_seriaStatProvider != null)
        {
            foreach (var localizationString in _seriaStatProvider.StatsLocalizationStrings)
            {
                seriaStrings.Add(localizationString.LocalizationName);
            }
        }

        if (_charactersData != null)
        {
            foreach (var simpleCharacter in _charactersData.SimpleCharacters)
            {
                seriaStrings.Add(simpleCharacter.Name);
            }
        }

        if (_customizableCharacters.Count > 0)
        {
            foreach (var customizableCharacter in _customizableCharacters)
            {
                
            }
        }
        CreateFile(CreateDictionary(seriaStrings), $"{Application.dataPath}{_path}/SeriaFileLocalization.json");
    }


    [Button()]
    private void Create()
    {
        CreateFile(CreateDictionary(LocalizationString.LocalizationStrings), $"{Application.dataPath}{_path}{_fileName}");
    }

    private void CreateFile(Dictionary<string, string> dictionary, string newPath)
    {
        // string path = $"{Application.dataPath}{_path}";
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

    [SerializeField, Space(30f)] private string _text;

    [Button()]
    private void CreateKeyByText()
    {
        if (string.IsNullOrEmpty(_text) == false)
        {
            Debug.Log($"{_forging}{LocalizationString.GenerateStableHash(_text)}{_forging}{_colon} {_forging}{_text}{_forging}");
        }
    }
}