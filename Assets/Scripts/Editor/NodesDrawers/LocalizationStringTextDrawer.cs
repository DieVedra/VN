﻿
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class LocalizationStringTextDrawer
{
    private const string _errorText = "Размер строки превышен ";
    private readonly SimpleTextValidator _simpleTextValidator;

    public LocalizationStringTextDrawer(SimpleTextValidator simpleTextValidator)
    {
        _simpleTextValidator = simpleTextValidator;
    }
    public LocalizationStringTextDrawer() { }
    public void DrawTextField(LocalizationString localizationString, string label, bool drawTextArea = true)
    {
        if (_simpleTextValidator == null)
        {
            return;
        }
        _simpleTextValidator.ValidText = localizationString.DefaultText;
        if (drawTextArea)
        {
            EditorGUILayout.LabelField(label);
            _simpleTextValidator.ValidText = EditorGUILayout.TextArea(_simpleTextValidator.ValidText, GUILayout.Height(50f), GUILayout.Width(150f));
        }
        else
        {
            _simpleTextValidator.ValidText = EditorGUILayout.TextField(label, _simpleTextValidator.ValidText, GUILayout.Width(450f));
        }

        if (_simpleTextValidator.TryValidate())
        {
            localizationString.SetText(_simpleTextValidator.ValidText);
        }
        else
        {
            EditorGUILayout.HelpBox(_errorText , MessageType.Error);
        }
    }
    public LocalizationString GetLocalizationStringFromProperty(SerializedProperty property)
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
            return (LocalizationString)fieldInfo.GetValue(targetObject);
        }
        return null;
    }
}