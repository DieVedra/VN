using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(SwitchNode))]
public class SwitchNodeDrawer : NodeEditor
{
    private SwitchNode _switchNode;
    private MethodInfo _addStatPortMethod;
    private MethodInfo _removeStatPortMethod;
    private SerializedProperty _nodeForStatsKeySerializedProperty;
    private SerializedProperty _nodeForBoolSerializedProperty;
    private SerializedProperty _casesForStatsListProperty;
    public override void OnBodyGUI()
    {
        if (_switchNode == null)
        {
            _switchNode = target as SwitchNode;
        }
        if (_nodeForStatsKeySerializedProperty == null)
        {
            _nodeForStatsKeySerializedProperty = serializedObject.FindProperty("_isNodeForStats");
            _nodeForBoolSerializedProperty = serializedObject.FindProperty("_isNodeForBool");
        }
        serializedObject.Update();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));

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
            _casesForStatsListProperty = serializedObject.FindProperty("_casesForStats");
            if (_casesForStatsListProperty != null && _casesForStatsListProperty.arraySize > 0)
            {
                for (int i = 0; i < _casesForStatsListProperty.arraySize; i++)
                {
                    if (i < _casesForStatsListProperty.arraySize)
                    {
                        NodePort port = _switchNode.GetOutputPort(_casesForStatsListProperty.GetArrayElementAtIndex(i).FindPropertyRelative("_name").stringValue);
                        DrawCase( _casesForStatsListProperty.GetArrayElementAtIndex(i), port.fieldName, $"CaseForStats {i + 1} port");
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
        LineDrawer.DrawHorizontalLine(Color.yellow);

        if (_nodeForBoolSerializedProperty.boolValue == true)
        {
            NodeEditorGUILayout.PortField(new GUIContent("Output False Bool Port "), _switchNode.GetOutputPort("Output"));
        }
        else
        {
            NodeEditorGUILayout.PortField(new GUIContent("Default Port "), _switchNode.GetOutputPort("Output"));
        }

        serializedObject.ApplyModifiedProperties();
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
    private void DrawCase(SerializedProperty serializedPropertyMyCase, string portName, string portViewName)
    {
        SerializedProperty serializedProperty = serializedPropertyMyCase.FindPropertyRelative("_foldoutKey");
        serializedProperty.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(serializedProperty.boolValue,  "Settings");
        
        if (serializedProperty.boolValue == true)
        {
            DrawStats(serializedPropertyMyCase.FindPropertyRelative("_caseStats"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        NodeEditorGUILayout.PortField(new GUIContent(portViewName), _switchNode.GetOutputPort(portName));
        EditorGUILayout.Space(10f);

    }
    private void DrawStats(SerializedProperty statsSerializedPropertyMyCase)
    {
        SerializedProperty serializedPropertyToggle;
        SerializedProperty serializedProperty;
        for (int i = 0; i < statsSerializedPropertyMyCase.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            serializedProperty = statsSerializedPropertyMyCase.GetArrayElementAtIndex(i);
            serializedPropertyToggle = serializedProperty.FindPropertyRelative("_includeKey");
            serializedPropertyToggle.boolValue = EditorGUILayout.Toggle(serializedPropertyToggle.boolValue, GUILayout.Width(20f));
            DrawLabel(serializedProperty.FindPropertyRelative("_name"));
            if (serializedPropertyToggle.boolValue)
            {
                DrawPopup(serializedProperty.FindPropertyRelative("_indexCurrentOperator"));
                DrawInt(serializedProperty.FindPropertyRelative("_value"));
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawLabel(SerializedProperty serializedProperty)
    {
        EditorGUILayout.LabelField(serializedProperty.stringValue,GUILayout.Width(150f));
    }

    private void DrawPopup(SerializedProperty indexSerializedProperty)
    {
        if (_switchNode.Operators != null && indexSerializedProperty != null)
        {
            indexSerializedProperty.intValue = EditorGUILayout.Popup(indexSerializedProperty.intValue, _switchNode.Operators.ToArray(), GUILayout.Width(40f));
        }
    }

    private void DrawInt(SerializedProperty serializedProperty)
    {
        serializedProperty.intValue = EditorGUILayout.IntField(serializedProperty.intValue, GUILayout.Width(30f));
    }
}