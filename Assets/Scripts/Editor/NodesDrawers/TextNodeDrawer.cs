using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

public class TextNodeDrawer
{
    private readonly int _maxCountSymbol;
    private readonly string _labelText;

    private string _validText;
    
    private readonly SerializedProperty _textNarrativeProperty;
    private readonly SimpleTextValidator _textValidator;
    private readonly SerializedObject _serializedObject;
    private readonly Action _callMethod;


    public TextNodeDrawer(SerializedProperty textNarrativeProperty, SerializedObject serializedObject, Action callMethod, string labelText, int maxCountSymbol)
    {
        _textNarrativeProperty = textNarrativeProperty;
        _textValidator = new SimpleTextValidator();
        _validText = _textNarrativeProperty.stringValue;
        _serializedObject = serializedObject;
        _callMethod = callMethod;
        _labelText = labelText;
        _maxCountSymbol = maxCountSymbol;
    }

    public void DrawGUI()
    {
        _serializedObject.Update();
        LineDrawer.DrawHorizontalLine(Color.green);
        EditorGUILayout.LabelField(_labelText);
        EditorGUI.BeginChangeCheck();
        _validText = EditorGUILayout.TextArea(_validText, GUILayout.Height(50f), GUILayout.Width(150f));
            
        if (EditorGUI.EndChangeCheck())
        {
            if (_textValidator.TryValidate(ref _validText, _maxCountSymbol))
            {
                _textNarrativeProperty.stringValue = _validText;
                _serializedObject.ApplyModifiedProperties();
                _callMethod.Invoke();
            }
            else
            {
                EditorGUILayout.HelpBox("Размер строки превышен " , MessageType.Error);
            }
        }
    }
}
