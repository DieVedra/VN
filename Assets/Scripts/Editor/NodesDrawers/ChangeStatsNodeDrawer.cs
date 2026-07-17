using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(ChangeStatsNode))]
public class ChangeStatsNodeDrawer : NodeEditor
{
    private ChangeStatsNode _changeStatsNode;
    private SerializedProperty _statsProperty;
    private SerializedProperty _statProperty;
    private SerializedProperty _valueProperty;
    private SerializedProperty _notificationKeyProperty;
    private SerializedProperty _phoneModeProperty;
    private SerializedProperty _inputPortProperty;
    private SerializedProperty _outputPortProperty;
    private bool _showKey;

    public override void OnBodyGUI()
    {
        if (_changeStatsNode == null)
        {
            _changeStatsNode = target as ChangeStatsNode;
            _statsProperty = serializedObject.FindProperty("_stats");
            _phoneModeProperty = serializedObject.FindProperty("_phoneMode");
            _inputPortProperty = serializedObject.FindProperty("Input");
            _outputPortProperty = serializedObject.FindProperty("Output");
            return;
        }
        
        serializedObject.Update();
        
        
        try
        {
            EditorGUILayout.BeginVertical();

            NodeEditorGUILayout.PropertyField(_inputPortProperty);
            NodeEditorGUILayout.PropertyField(_outputPortProperty);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Phone Mode: ");
            _phoneModeProperty.boolValue = EditorGUILayout.Toggle(_phoneModeProperty.boolValue);
            EditorGUILayout.EndHorizontal();

            _showKey = EditorGUILayout.Foldout(_showKey, "Settings", true);
            if (_showKey)
            {
                DrawStats(_changeStatsNode.BaseStatsLocalizations);
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
        catch (System.ArgumentException) { }
    }

    private void DrawStats(IReadOnlyList<ILocalizationString> baseStatsChoiceLocalizations)
    {
        EditorGUILayout.BeginVertical("box");
        
        for (int i = 0; i < _statsProperty.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            _statProperty = _statsProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.LabelField($"{baseStatsChoiceLocalizations[i].LocalizationNameToGame.DefaultText}: ", GUILayout.Width(200f));
            
            _valueProperty = _statProperty.FindPropertyRelative("_value");
            _valueProperty.intValue = EditorGUILayout.IntField(_valueProperty.intValue, GUILayout.Width(30f));
            
            EditorGUILayout.LabelField($"Notification: ", GUILayout.Width(80f));
            _notificationKeyProperty = _statProperty.FindPropertyRelative("_notificationKey");
            _notificationKeyProperty.boolValue = EditorGUILayout.Toggle(_notificationKeyProperty.boolValue);
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
    }
}