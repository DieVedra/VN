using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelEntryPointEditor))]
public class GameEntryPointDrawer : Editor
{
    private LevelEntryPointEditor _gameEntryPoint;
    private LineDrawer _lineDrawer;
    private SerializedProperty _isTestModeSerializedProperty;
    private SerializedProperty _seriaIndexSerializedProperty;
    private SerializedProperty _graphIndexSerializedProperty;
    private SerializedProperty _nodeIndexSerializedProperty;
    private SerializedProperty _testModeEditorSerializedProperty;
    private SerializedProperty _seriaGameStatsProviderEditorSerializedProperty;
    private SeriaGameStatsProviderEditor _seriaGameStatsProviderEditor;
    private List<BaseStat> _stats;
    private void OnEnable()
    {
        _gameEntryPoint = target as LevelEntryPointEditor;
        _lineDrawer = new LineDrawer();
        _seriaGameStatsProviderEditorSerializedProperty = serializedObject.FindProperty("_seriaGameStatsProviderEditor");
        _testModeEditorSerializedProperty = serializedObject.FindProperty("_testModeEditor");
        _isTestModeSerializedProperty = _testModeEditorSerializedProperty.FindPropertyRelative("_isTestMode");
        _seriaIndexSerializedProperty = _testModeEditorSerializedProperty.FindPropertyRelative("_seriaIndex");
        _graphIndexSerializedProperty = _testModeEditorSerializedProperty.FindPropertyRelative("_graphIndex");
        _nodeIndexSerializedProperty = _testModeEditorSerializedProperty.FindPropertyRelative("_nodeIndex");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.Space(20f);
        if (GameInitializer.IsGamePlayMode == false)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Test Mode: ");
            _isTestModeSerializedProperty.boolValue = EditorGUILayout.Toggle(_isTestModeSerializedProperty.boolValue);
            EditorGUILayout.EndHorizontal();
            // if (_isTestModeSerializedProperty.boolValue)
            // {
            //     EditorGUILayout.Space(10f);
            //     EditorGUILayout.LabelField("Start from: ");
            //     _lineDrawer.DrawHorizontalLine(Color.green);
            //     DrawTestField(_seriaIndexSerializedProperty, "Seria index: ");
            //     DrawTestField(_graphIndexSerializedProperty, "Graph index: ");
            //     DrawTestField(_nodeIndexSerializedProperty, "Node index: ");
            //     _lineDrawer.DrawHorizontalLine(Color.green);
            //     StatsDrawer();
            //     _lineDrawer.DrawHorizontalLine(Color.red);
            //     
            //     if (GUILayout.Button("UpdateStatsForm"))
            //     {
            //         Debug.Log($"UpdateStatsForm1");
            //         UpdateStatsForm();
            //     }
            // }
            EditorGUILayout.Space(30f);

            if (GUILayout.Button("Initialize"))
            {
                if (_gameEntryPoint.IsInitializing == false)
                {
                    _gameEntryPoint.Init();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawTestField(SerializedProperty serializedProperty, string name)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name);
        serializedProperty.intValue = EditorGUILayout.IntField(serializedProperty.intValue);
        EditorGUILayout.EndHorizontal();
    }

    private void StatsDrawer()
    {
        if (_seriaGameStatsProviderEditor == null)
        {
            _seriaGameStatsProviderEditor = GetSeriaGameStatsProviderEditorFromProperty(_seriaGameStatsProviderEditorSerializedProperty);
        }

        if (_stats != null)
        {
            foreach (var stat in _stats)
            {
                StatDrawer(stat);
            }
        }
        else
        {
            UpdateStatsForm();
        }
    }

    private void StatDrawer(BaseStat stat)
    {
        FieldInfo fieldInfo = stat.GetType().GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
        int value = (int)fieldInfo.GetValue(stat);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(stat.Name);
        value = EditorGUILayout.IntField(value);
        EditorGUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {
            fieldInfo.SetValue(stat, value);
        }
    }
    private SeriaGameStatsProviderEditor GetSeriaGameStatsProviderEditorFromProperty(SerializedProperty property)
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
            return (SeriaGameStatsProviderEditor)fieldInfo.GetValue(targetObject);
        }
        return null;
    }

    private void TryUpdateStatsForm()
    {
        // if (_stats)
        // {
        //     
        // }
    }
    private void UpdateStatsForm()
    {
        Debug.Log($"UpdateStatsForm");
        _stats = _seriaGameStatsProviderEditor.GameStatsHandler?.GetGameBaseStatsForm();
    }
}