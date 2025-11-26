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
    private LineDrawer _lineDrawer;
    private SwitchNodeEditorLogic _switchNodeLogic;

    private MethodInfo _addStatPortMethod;
    private MethodInfo _removeStatPortMethod;
    private SerializedProperty _nodeForStatsKeySerializedProperty;
    private SerializedProperty _nodeForBoolSerializedProperty;
    private SerializedProperty _casesForStatsListProperty;
    private SerializedProperty _inputPortProperty;
    public override void OnBodyGUI()
    {
        if (_switchNode == null)
        {
            _switchNode = target as SwitchNode;
            _lineDrawer = new LineDrawer();
            _nodeForStatsKeySerializedProperty = serializedObject.FindProperty("_isNodeForStats");
            _nodeForBoolSerializedProperty = serializedObject.FindProperty("_isNodeForBool");
            _inputPortProperty = serializedObject.FindProperty("Input");
            _casesForStatsListProperty = serializedObject.FindProperty("_casesForStats");
            
            _switchNodeLogic = new SwitchNodeEditorLogic(serializedObject.FindProperty("_removeAdditionalCaseIndex"),
                serializedObject.FindProperty("_casesForStats"), _switchNode.GetSwitchNodeLogic,
                serializedObject, _switchNode.GetOutputPort);
            _addStatPortMethod = _switchNode.GetType().GetMethod("AddDynamicPort", BindingFlags.NonPublic | BindingFlags.Instance);
            _removeStatPortMethod = _switchNode.GetType().GetMethod("RemoveStatDynamicPort", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        else
        {
            _switchNodeLogic.TryInitStatsNames();
            _switchNodeLogic.TryUpdateAdditionalCases();
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
                    _addStatPortMethod.Invoke(_switchNode, null);
                    serializedObject.Update();
                }

                if (GUILayout.Button("RemoveStatPort"))
                {
                    _removeStatPortMethod.Invoke(_switchNode, null);
                    serializedObject.Update();
                }

                EditorGUILayout.EndHorizontal();
                if (_switchNode.GetSwitchNodeLogic?.Operators != null)
                {
                    if (_casesForStatsListProperty != null && _casesForStatsListProperty.arraySize > 0)
                    {
                        for (int i = 0; i < _casesForStatsListProperty.arraySize; i++)
                        {
                            if (i < _casesForStatsListProperty.arraySize)
                            {
                                NodePort port = _switchNode.GetOutputPort(_casesForStatsListProperty.GetArrayElementAtIndex(i).FindPropertyRelative("_name").stringValue);
                                _switchNodeLogic.DrawCase(_switchNode.CaseLocalizations[i], _casesForStatsListProperty.GetArrayElementAtIndex(i), port.fieldName, $"CaseForStats {i + 1} port");
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
}