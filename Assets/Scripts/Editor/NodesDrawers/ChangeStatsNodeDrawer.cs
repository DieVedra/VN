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
    private SerializedProperty _inputPortProperty;
    private SerializedProperty _outputPortProperty;
    private bool _showKey;

    public override void OnBodyGUI()
    {
        if (_changeStatsNode == null)
        {
            _changeStatsNode = target as ChangeStatsNode;
            _statsProperty = serializedObject.FindProperty("_stats");
            _inputPortProperty = serializedObject.FindProperty("Input");
            _outputPortProperty = serializedObject.FindProperty("Output");
        }
        else
        {
            serializedObject.Update();
            NodeEditorGUILayout.PropertyField(_inputPortProperty);
            NodeEditorGUILayout.PropertyField(_outputPortProperty);
            _showKey = EditorGUILayout.BeginFoldoutHeaderGroup(_showKey, "Settings");
            if (_showKey)
            {
                DrawStats(_changeStatsNode.BaseStatsLocalizations);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawStats(IReadOnlyList<ILocalizationString> baseStatsChoiceLocalizations)
    {
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
    }
}