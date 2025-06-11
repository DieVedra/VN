using System;
using UnityEditor;
using UnityEngine;

public class TextNodeDrawer
{
    private readonly string _labelText;
    private readonly LocalizationStringTextDrawer _localizationStringTextDrawer;
    private readonly LineDrawer _lineDrawer;
    private readonly SerializedObject _serializedObject;
    private readonly LocalizationString _localizationString;
    private readonly Action _callMethod;


    public TextNodeDrawer(SerializedProperty textProperty, SerializedObject serializedObject, Action callMethod, string labelText, int maxCountSymbol)
    {
        _localizationStringTextDrawer = new LocalizationStringTextDrawer(new SimpleTextValidator(maxCountSymbol));
        _serializedObject = serializedObject;
        _callMethod = callMethod;
        _labelText = labelText;
        _lineDrawer = new LineDrawer();
        _localizationString = _localizationStringTextDrawer.GetLocalizationStringFromProperty(textProperty);
    }

    public void DrawGUI()
    {
        _serializedObject.Update();
        _lineDrawer.DrawHorizontalLine(Color.green);
        EditorGUI.BeginChangeCheck();
        
        _localizationStringTextDrawer.DrawTextField(_localizationString, _labelText);
        if (EditorGUI.EndChangeCheck())
        {
            _callMethod.Invoke();
        }
    }
}
