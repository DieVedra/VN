using System;
using System.Reflection;
using UnityEditor;

public class ObjectProviderFromProperty
{
    public T GetPropertyObject<T>(SerializedProperty property)
    {
        object targetObject = property.serializedObject.targetObject;
        Type parentType = targetObject.GetType();
        FieldInfo fieldInfo = parentType.GetField(
            property.propertyPath,
            BindingFlags.Instance | 
            BindingFlags.Public | 
            BindingFlags.NonPublic
        );
        
        if (fieldInfo != null)
        {
            return (T)fieldInfo.GetValue(targetObject);
        }
        else
        {
            return default;
        }
    }
    public T GetObject<T>(SerializedProperty property)
    {
        object targetObject = property.serializedObject.targetObject;
        return (T)targetObject;
    }
}