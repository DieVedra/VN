using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(ChoiceNode))]
public class ChoiceNodeDrawer : NodeEditor
{
    private const string _namePort = "Port";
    protected object NodeForCallMethods;
    private ChoiceNode _choiceNode;
    private LineDrawer _lineDrawer;
    private SerializedProperty _showOutputProperty;
    private SerializedProperty _timerPortIndexProperty;

    private SerializedProperty _addTimerProperty;
    private SerializedProperty _timerValueProperty;
    private SerializedProperty _choiceCasesProperty;
    private SerializedProperty _caseProperty;
    private MethodInfo _privateMethod;
    private MethodInfo _addCase;
    private MethodInfo _removeCase;
    private MethodInfo _initLocalizationStringInCase;
    private string[] _timerPortIndexes;
    private bool[] _showStatsKeys;

    public override void OnBodyGUI()
    {
        if (_choiceNode == null)
        {
            _choiceNode = target as ChoiceNode;
            _showOutputProperty = serializedObject.FindProperty("_showOutput");
            _lineDrawer = new LineDrawer();
            _addTimerProperty = serializedObject.FindProperty("_addTimer");
            _timerValueProperty = serializedObject.FindProperty("_timerValue");
            _timerPortIndexProperty = serializedObject.FindProperty("_timerPortIndex");
            _showOutputProperty = serializedObject.FindProperty("_showOutput");
            _choiceCasesProperty = serializedObject.FindProperty("_choiceCases");
            _showStatsKeys = new bool[ChoiceNode.MaxCaseCount];
            for (int i = 0; i < ChoiceNode.MaxCaseCount; i++)
            {
                _showStatsKeys[i] = false;
            }
            if (NodeForCallMethods == null)
            {
                NodeForCallMethods = _choiceNode;
            }
            Type type = typeof(ChoiceNode);
            _privateMethod = type.GetMethod("SetInfoToView", BindingFlags.NonPublic | BindingFlags.Instance);
            _addCase = type.GetMethod("AddCase", BindingFlags.NonPublic | BindingFlags.Instance);
            _removeCase = type.GetMethod("RemoveCase", BindingFlags.NonPublic | BindingFlags.Instance);
            _initLocalizationStringInCase = type.GetMethod("InitLocalizationStringInCase", BindingFlags.NonPublic | BindingFlags.Instance);
            CreatePortIndexes();
        }
        else
        {
            serializedObject.Update();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));
            EditorGUILayout.LabelField($"Max cases: {ChoiceNode.MaxCaseCount}");
            EditorGUI.BeginChangeCheck();
            _showOutputProperty.boolValue = EditorGUILayout.Toggle("Show Output: ", _showOutputProperty.boolValue);

            if (_showOutputProperty.boolValue)
            {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Output"));
                EditorGUILayout.Space(10f);
            }

            EditorGUILayout.BeginHorizontal();
            _addTimerProperty.boolValue = EditorGUILayout.Toggle("Add Timer: ", _addTimerProperty.boolValue, GUILayout.Width(100f));
            if (_addTimerProperty.boolValue)
            {
                _timerValueProperty.intValue= EditorGUILayout.IntField("Timer Value: ", _timerValueProperty.intValue, GUILayout.Width(120f));
            }
            EditorGUILayout.EndHorizontal();

            if (_addTimerProperty.boolValue)
            {
                if (_timerPortIndexes != null && _timerPortIndexes.Length > 0)
                {
                    if (_timerPortIndexProperty.intValue > _timerPortIndexes.Length - 1)
                    {
                        _timerPortIndexProperty.intValue = _timerPortIndexes.Length - 1;
                    }
                    _timerPortIndexProperty.intValue =
                        EditorGUILayout.Popup("Transit To Port index: ", _timerPortIndexProperty.intValue, _timerPortIndexes);
                }
            }

            _lineDrawer.DrawHorizontalLine(Color.cyan);
            EditorGUILayout.Space(20f);
            
            for (int i = 0; i < _choiceCasesProperty.arraySize; i++)
            {
                DrawChoiceCase(_choiceCasesProperty.GetArrayElementAtIndex(i), i);
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                CallMethodSetInfoToView();
            }
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Case"))
            {
                CallMethodAddCase();
                CreatePortIndexes();
            }

            if (GUILayout.Button("Remove Case"))
            {
                CallMethodRemoveCase();
                CreatePortIndexes();
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
    private void DrawStats(IReadOnlyList<ILocalizationString> baseStatsChoiceLocalizations, SerializedProperty gameStatsFormsSerializedProperty)
    {
        SerializedProperty statFormSerializedProperty;
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(10f);
        for (int i = 0; i < gameStatsFormsSerializedProperty.arraySize; i++)
        {
            statFormSerializedProperty = gameStatsFormsSerializedProperty.GetArrayElementAtIndex(i);
            DrawField(statFormSerializedProperty.FindPropertyRelative("_value"),
                statFormSerializedProperty.FindPropertyRelative("_notificationKey"),
                baseStatsChoiceLocalizations[i].LocalizationNameToGame.DefaultText);
        }
        EditorGUILayout.EndVertical();
    }
    private void DrawField(SerializedProperty numberSerializedProperty, SerializedProperty notificationKeySerializedProperty, string nameField)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(nameField, GUILayout.Width(150f));
        numberSerializedProperty.intValue = EditorGUILayout.IntField(numberSerializedProperty.intValue, GUILayout.Width(30f));
        EditorGUILayout.LabelField("Add notification: ", GUILayout.Width(100f));
        notificationKeySerializedProperty.boolValue = EditorGUILayout.Toggle(notificationKeySerializedProperty.boolValue);
        EditorGUILayout.EndHorizontal();
    }

    private void CreatePortIndexes()
    {
        var a = _choiceNode.DynamicOutputs.Count();
        _timerPortIndexes = new string[a];
        for (int i = 0; i < a; i++)
        {
            _timerPortIndexes[i] = $"{_namePort} {i+1}";
        }
    }
    private void DrawChoiceCase(SerializedProperty caseSerializedProperty, int index)
    {
        _caseProperty = caseSerializedProperty.FindPropertyRelative("_choiceText");
        EditorGUI.BeginChangeCheck();
        DrawHorizontalField<string>(_caseProperty, "Text: ", 50f, 450f);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            _initLocalizationStringInCase.Invoke(NodeForCallMethods, new object[] {index, _caseProperty.stringValue});
            serializedObject.Update();
        }

        _caseProperty = caseSerializedProperty.FindPropertyRelative("_choicePrice");
        DrawHorizontalField<int>(_caseProperty, "Price: ", 50f);
        
        _caseProperty = caseSerializedProperty.FindPropertyRelative("_choiceAdditionaryPrice");
        DrawHorizontalField<int>(_caseProperty, "Additionary Price: ", 100f);

        _caseProperty = caseSerializedProperty.FindPropertyRelative("_showNotificationChoice");
        DrawHorizontalField<bool>(_caseProperty, "Show Notifications: ", 120f);

        _showStatsKeys[index] = EditorGUILayout.BeginFoldoutHeaderGroup(_showStatsKeys[index], "Show stats: ");
        if (_showStatsKeys[index] == true)
        {
            DrawStats(_choiceNode.GetStatsChoiceLocalizations(index), caseSerializedProperty.FindPropertyRelative("_baseStatsChoice"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        if (_showOutputProperty.boolValue == false)
        {
            NodeEditorGUILayout.PortField(_choiceNode.GetOutputPort($"{ChoiceNode.PortNamePart1}{index}{ChoiceNode.PortNamePart2}"));
        }
        _lineDrawer.DrawHorizontalLine(Color.cyan);
    }

    private void DrawHorizontalField<T>(SerializedProperty serializedProperty, string name, float widthLabel, float widthField = 50f)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name, GUILayout.Width(widthLabel));
        Type type = typeof(T);
        if (type == typeof(int))
        {
            serializedProperty.intValue = EditorGUILayout.IntField(serializedProperty.intValue, GUILayout.Width(widthField));
        }
        else if(type == typeof(float))
        {
            serializedProperty.floatValue = EditorGUILayout.FloatField(serializedProperty.floatValue, GUILayout.Width(widthField));
        }
        else if(type == typeof(bool))
        {
            serializedProperty.boolValue = EditorGUILayout.Toggle(serializedProperty.boolValue, GUILayout.Width(widthField));
        }
        else if (type == typeof(string))
        {
            _caseProperty.stringValue = EditorGUILayout.TextField(_caseProperty.stringValue, GUILayout.Width(widthField));
        }
        EditorGUILayout.EndHorizontal();
    }

    private void CallMethodAddCase()
    {
        _addCase.Invoke(NodeForCallMethods, null);
    }
    private void CallMethodRemoveCase()
    {
        _removeCase.Invoke(NodeForCallMethods, null);
    }
    private void CallMethodSetInfoToView()
    {
        _privateMethod.Invoke(NodeForCallMethods, null);
    }
}