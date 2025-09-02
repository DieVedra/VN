using System;
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
    private SerializedProperty _addMonetsSerializedProperty;
    private SerializedProperty _addHeartsSerializedProperty;
    private SerializedProperty _statsSerializedProperty;
    private SerializedProperty _walletSerializedProperty;
    private SeriaGameStatsProviderEditor _seriaGameStatsProviderEditor;
    private TestModeEditor _testModeEditor;
    private Wallet _wallet;
    private void OnEnable()
    {
        _gameEntryPoint = target as LevelEntryPointEditor;
        _lineDrawer = new LineDrawer();
        _seriaGameStatsProviderEditorSerializedProperty = serializedObject.FindProperty("_seriaGameStatsProviderEditor");
        _testModeEditorSerializedProperty = serializedObject.FindProperty("_testModeEditor");
        _walletSerializedProperty = serializedObject.FindProperty("_wallet");
        _isTestModeSerializedProperty = _testModeEditorSerializedProperty.FindPropertyRelative("_isTestMode");
        _seriaIndexSerializedProperty = _testModeEditorSerializedProperty.FindPropertyRelative("_seriaIndex");
        _graphIndexSerializedProperty = _testModeEditorSerializedProperty.FindPropertyRelative("_graphIndex");
        _nodeIndexSerializedProperty = _testModeEditorSerializedProperty.FindPropertyRelative("_nodeIndex");
        _addMonetsSerializedProperty = _testModeEditorSerializedProperty.FindPropertyRelative("_addMonets");
        _addHeartsSerializedProperty = _testModeEditorSerializedProperty.FindPropertyRelative("_addHearts");
        _statsSerializedProperty = _testModeEditorSerializedProperty.FindPropertyRelative("_stats");
        _wallet = GetFromProperty<Wallet>(_walletSerializedProperty);
        _testModeEditor = GetFromProperty<TestModeEditor>(_testModeEditorSerializedProperty);
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
            EditorGUILayout.Space(20f);
            if (_isTestModeSerializedProperty.boolValue)
            {
                AddToWallet();
                EditorGUILayout.Space(10f);
                EditorGUILayout.LabelField("Start from: ");
                _lineDrawer.DrawHorizontalLine(Color.green);
                DrawTestField(_seriaIndexSerializedProperty, "Seria index: ");
                DrawTestField(_graphIndexSerializedProperty, "Graph index: ");
                DrawTestField(_nodeIndexSerializedProperty, "Node index: ");
                _lineDrawer.DrawHorizontalLine(Color.green);
                StatsDrawer();
                _lineDrawer.DrawHorizontalLine(Color.red);
                if (GUILayout.Button("UpdateStatsForm"))
                {
                    _testModeEditor.SetStats(_seriaGameStatsProviderEditor.GameStatsHandler?.GetGameBaseStatsForm());
                }
            }

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
            _seriaGameStatsProviderEditor = GetFromProperty<SeriaGameStatsProviderEditor>(_seriaGameStatsProviderEditorSerializedProperty);
        }

        if (_statsSerializedProperty != null)
        {
            for (int i = 0; i < _testModeEditor.Stats.Count; i++)
            {
                StatDrawer(_testModeEditor.Stats[i]);
            }
        }
    }
    private void StatDrawer(BaseStat stat)
    {
        FieldInfo fieldInfo = stat.GetType().GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
        int value = (int)fieldInfo.GetValue(stat);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(stat.NameText);
        value = EditorGUILayout.IntField(value);
        EditorGUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {
            fieldInfo.SetValue(stat, value);
        }
    }
    private T GetFromProperty<T>(SerializedProperty property)
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
        return default;
    }

    private void AddToWallet()
    {
        if (Application.isPlaying)
        {
            _addMonetsSerializedProperty.intValue = EditorGUILayout.IntField("AddMonets ", _addMonetsSerializedProperty.intValue);
            _addHeartsSerializedProperty.intValue = EditorGUILayout.IntField("AddHearts ", _addHeartsSerializedProperty.intValue);
            if (GUILayout.Button("AddToWallet"))
            {
                _wallet.AddCash(_addMonetsSerializedProperty.intValue);
                _wallet.AddHearts(_addHeartsSerializedProperty.intValue);
            }
        }
    }
}