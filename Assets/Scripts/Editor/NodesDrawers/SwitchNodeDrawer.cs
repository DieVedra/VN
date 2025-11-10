using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(SwitchNode))]
public class SwitchNodeDrawer : NodeEditor
{
    private const string _index = "Index: ";
    private SwitchNode _switchNode;
    private LineDrawer _lineDrawer;
    private StringBuilder _stringBuilder;
    private MethodInfo _addStatPortMethod;
    private MethodInfo _removeStatPortMethod;
    private SerializedProperty _nodeForStatsKeySerializedProperty;
    private SerializedProperty _nodeForBoolSerializedProperty;
    private SerializedProperty _casesForStatsListProperty;
    private SerializedProperty _inputPortProperty;
    private SerializedProperty _foldOutKey1SerializedProperty;
    private SerializedProperty _additionalCaseStatsArraySerializedProperty;
    private SerializedProperty _additionalCaseSerializedProperty;
    private SerializedProperty _addAdditionalCaseSerializedProperty;
    private SerializedProperty _removeAdditionalCaseIndexSerializedProperty;
    private string[] _statsNames;
    
    public override void OnBodyGUI()
    {
        if (_switchNode == null)
        {
            _switchNode = target as SwitchNode;
            _lineDrawer = new LineDrawer();
            _stringBuilder = new StringBuilder();
            _nodeForStatsKeySerializedProperty = serializedObject.FindProperty("_isNodeForStats");
            _nodeForBoolSerializedProperty = serializedObject.FindProperty("_isNodeForBool");
            _inputPortProperty = serializedObject.FindProperty("Input");
            _removeAdditionalCaseIndexSerializedProperty = serializedObject.FindProperty("_removeAdditionalCaseIndex");
            _casesForStatsListProperty = serializedObject.FindProperty("_casesForStats");
        }
        else
        {
            if (_statsNames == null)
            {
                TryInitStatsNames();
            }
            serializedObject.Update();
            NodeEditorGUILayout.PropertyField(_inputPortProperty);
            if (_nodeForBoolSerializedProperty.boolValue == false)
            {
                EditorGUILayout.BeginHorizontal();
                _nodeForStatsKeySerializedProperty.boolValue =
                    EditorGUILayout.Toggle(_nodeForStatsKeySerializedProperty.boolValue);
                EditorGUILayout.LabelField("Is Stats Switch");
                EditorGUILayout.EndHorizontal();
            }

            if (_nodeForStatsKeySerializedProperty.boolValue == false)
            {
                EditorGUILayout.BeginHorizontal();
                _nodeForBoolSerializedProperty.boolValue =
                    EditorGUILayout.Toggle(_nodeForBoolSerializedProperty.boolValue);
                EditorGUILayout.LabelField("Is Bool Switch");
                EditorGUILayout.EndHorizontal();
            }

            if (_nodeForStatsKeySerializedProperty.boolValue == true)
            {
            
                EditorGUILayout.Space(10f);
                EditorGUILayout.LabelField($"Dynamic ports count: {_switchNode.DynamicOutputs.Count()}");
                EditorGUILayout.Space(10f);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add CaseForStats"))
                {
                    AddPortCaseForStats();
                    serializedObject.Update();
                }

                if (GUILayout.Button("RemoveStatPort"))
                {
                    RemoveStatPort();
                    serializedObject.Update();
                }

                EditorGUILayout.EndHorizontal();
                if (_switchNode.SwitchNodeLogic?.Operators != null)
                {
                    if (_casesForStatsListProperty != null && _casesForStatsListProperty.arraySize > 0)
                    {
                        for (int i = 0; i < _casesForStatsListProperty.arraySize; i++)
                        {
                            if (i < _casesForStatsListProperty.arraySize)
                            {
                                NodePort port = _switchNode.GetOutputPort(_casesForStatsListProperty.GetArrayElementAtIndex(i).FindPropertyRelative("_name").stringValue);
                                DrawCase(_switchNode.CaseLocalizations[i], _casesForStatsListProperty.GetArrayElementAtIndex(i), port.fieldName, $"CaseForStats {i + 1} port");
                            }
                        }
                    }
                }
            }

            EditorGUILayout.Space(10f);
            if (_nodeForBoolSerializedProperty.boolValue == true)
            {
                EditorGUILayout.LabelField("If Input True",GUILayout.Width(100f));
                NodeEditorGUILayout.PortField(new GUIContent($"Output True Bool Port"), _switchNode.GetOutputPort("OutputTrueBool"));
            }

            _lineDrawer.DrawHorizontalLine(Color.yellow);
            if (_nodeForBoolSerializedProperty.boolValue == true)
            {
                NodeEditorGUILayout.PortField(new GUIContent("Output False Bool Port "), _switchNode.OutputPortBaseNode);
            }
            else
            {
                NodeEditorGUILayout.PortField(new GUIContent("Default Port "), _switchNode.OutputPortBaseNode);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
    
    private void AddPortCaseForStats()
    {
        CallMethod(ref _addStatPortMethod, "AddDynamicPort");
    }
    private void RemoveStatPort()
    {
        CallMethod(ref _removeStatPortMethod, "RemoveStatDynamicPort");
    }
    private void CallMethod(ref MethodInfo methodInfo, string name)
    {
        if (methodInfo == null)
        {
            methodInfo = _switchNode.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
        }
        methodInfo.Invoke(_switchNode, null);
    }
    private void DrawCase(CaseForStats statsLocalizations, SerializedProperty serializedPropertyMyCase, string portName, string portViewName)
    {
        _foldOutKey1SerializedProperty = serializedPropertyMyCase.FindPropertyRelative("_foldoutKey");
        _foldOutKey1SerializedProperty.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(_foldOutKey1SerializedProperty.boolValue,  "Settings");
        
        if (_foldOutKey1SerializedProperty.boolValue == true)
        {
            DrawStats(statsLocalizations.StatsLocalizations, serializedPropertyMyCase.FindPropertyRelative("_caseStats"));
            _additionalCaseStatsArraySerializedProperty = serializedPropertyMyCase.FindPropertyRelative("_additionalCaseStats");
            DrawAdditionalSettings();
            AddAdditionalSettings();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        NodeEditorGUILayout.PortField(new GUIContent(portViewName), _switchNode.GetOutputPort(portName));
        EditorGUILayout.Space(10f);
    }
    private void DrawStats(IReadOnlyList<ILocalizationString> caseStats, SerializedProperty statsSerializedPropertyMyCase)
    {
        SerializedProperty serializedPropertyToggle;
        SerializedProperty serializedProperty;
        for (int i = 0; i < statsSerializedPropertyMyCase.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            serializedProperty = statsSerializedPropertyMyCase.GetArrayElementAtIndex(i);
            serializedPropertyToggle = serializedProperty.FindPropertyRelative("_includeKey");
            serializedPropertyToggle.boolValue = EditorGUILayout.Toggle(serializedPropertyToggle.boolValue, GUILayout.Width(20f));
            DrawLabel(caseStats[i].LocalizationName.DefaultText);
            if (serializedPropertyToggle.boolValue)
            {
                DrawPopup(_switchNode.SwitchNodeLogic.Operators ,serializedProperty.FindPropertyRelative("_indexCurrentOperator"), 40f);
                DrawInt(serializedProperty.FindPropertyRelative("_value"));
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawLabel(string name)
    {
        EditorGUILayout.LabelField(name,GUILayout.Width(150f));
    }

    private void DrawPopup(string[] strings, SerializedProperty indexSerializedProperty, float width)
    {
        if (strings != null && indexSerializedProperty != null)
        {
            indexSerializedProperty.intValue = EditorGUILayout.Popup(indexSerializedProperty.intValue, strings, GUILayout.Width(width));
        }
    }

    private void DrawInt(SerializedProperty serializedProperty)
    {
        serializedProperty.intValue = EditorGUILayout.IntField(serializedProperty.intValue, GUILayout.Width(30f));
    }

    private void DrawAdditionalSettings()
    {
        for (int i = 0; i < _additionalCaseStatsArraySerializedProperty.arraySize; i++)
        {
            _additionalCaseSerializedProperty = _additionalCaseStatsArraySerializedProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            _stringBuilder.Append(_index);
            _stringBuilder.Append(i.ToString());
            EditorGUILayout.LabelField(_stringBuilder.ToString() ,GUILayout.Width(50f));
            if (_statsNames != null)
            {
                DrawPopup(_statsNames, _additionalCaseSerializedProperty.FindPropertyRelative("_indexStat1"), 140f);
                DrawPopup(_switchNode.SwitchNodeLogic.Operators, _additionalCaseSerializedProperty.FindPropertyRelative("_indexCurrentOperator"), 40f);
                DrawPopup(_statsNames, _additionalCaseSerializedProperty.FindPropertyRelative("_indexStat2"), 140f);
            }

            EditorGUILayout.EndHorizontal();
            _stringBuilder.Clear();
        }
    }

    private void AddAdditionalSettings()
    {
        _lineDrawer.DrawHorizontalLine(Color.green);
        EditorGUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Additional Case"))
        {
            serializedObject.Update();
            int count = _additionalCaseStatsArraySerializedProperty.arraySize;
            _additionalCaseStatsArraySerializedProperty.InsertArrayElementAtIndex(count);
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("Remove Additional Case By Index: ") && _additionalCaseStatsArraySerializedProperty.arraySize > 0)
        {
            if (_removeAdditionalCaseIndexSerializedProperty.intValue < _additionalCaseStatsArraySerializedProperty.arraySize)
            {
                serializedObject.Update();
                _additionalCaseStatsArraySerializedProperty.DeleteArrayElementAtIndex(_removeAdditionalCaseIndexSerializedProperty.intValue);
                serializedObject.ApplyModifiedProperties();
            }
        }

        DrawInt(_removeAdditionalCaseIndexSerializedProperty);
        EditorGUILayout.EndHorizontal();
    }
    private void TryInitStatsNames()
    {
        if (_switchNode.SwitchNodeLogic?.GameStats != null)
        {
            int count = _switchNode.SwitchNodeLogic.GameStats.Count;
            _statsNames = new string[count];
            for (int i = 0; i < count; i++)
            {
                _statsNames[i] = _switchNode.SwitchNodeLogic.GameStats[i].NameText;
            }
        }
    }
}