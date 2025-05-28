using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class ScriptableObjectFieldCollector
{
    public static List<string> FindFieldsWithAttribute<T>(this ScriptableObject so, string categoryFilter = null) where T : Attribute
    {
        var results = new List<string>();
        Type type = so.GetType();
        
        // Получаем все поля (public и private с [SerializeField])
        FieldInfo[] fields = type.GetFields(
            BindingFlags.Public | 
            BindingFlags.NonPublic | 
            BindingFlags.Instance);
Debug.Log($"fields {fields.Length}");
        foreach (FieldInfo field in fields)
        {
            T attribute = field.GetCustomAttribute<T>();
            if (attribute != null)
            {

                var value = field.GetValue(so);
                results.Add(value as string);
            }
        }

        return results;
    }

    // public static List<T> FindAllScriptableObjectsWithAttribute<T>() where T : ScriptableObject
    // {
    //     var results = new List<T>();
    //     string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
    //
    //     foreach (string guid in guids)
    //     {
    //         string path = AssetDatabase.GuidToAssetPath(guid);
    //         T so = AssetDatabase.LoadAssetAtPath<T>(path);
    //         if (so != null)
    //         {
    //             results.Add(so);
    //         }
    //     }
    //
    //     return results;
    // }
    
    public static List<FieldInfo> FindFieldsWithAttributeSimple<T>(
        this ScriptableObject so) where T : Attribute
    {
        var fields = new List<FieldInfo>();
        Type type = so.GetType();
        
        foreach (FieldInfo field in type.GetFields(
            BindingFlags.Public | 
            BindingFlags.NonPublic | 
            BindingFlags.Instance))
        {
            if (field.GetCustomAttribute<T>() != null)
            {
                fields.Add(field);
            }
        }

        return fields;
    }
}