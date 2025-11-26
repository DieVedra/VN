using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(PhoneSwitchNode))]
public class PhoneSwitchNodeDrawer : NodeEditor
{
    private PhoneSwitchNode _phoneSwitchNode;
    private SwitchNodeEditorLogic _switchNodeLogic;
    private LineDrawer _lineDrawer;
    private SerializedProperty _inputPortProperty;
    private SerializedProperty _casesForStatsListProperty;
    private MethodInfo _addStatPortMethod;
    private MethodInfo _removeStatPortMethod;
    public override void OnBodyGUI()
    {
        if (_phoneSwitchNode == null)
        {
            _phoneSwitchNode = target as PhoneSwitchNode;
            _lineDrawer = new LineDrawer();
            _inputPortProperty = serializedObject.FindProperty("Input");
            _casesForStatsListProperty = serializedObject.FindProperty("_casesForStats");

            _switchNodeLogic = new SwitchNodeEditorLogic(serializedObject.FindProperty("_removeAdditionalCaseIndex"),
                serializedObject.FindProperty("_casesForStats"), _phoneSwitchNode.GetSwitchNodeLogic,
                serializedObject, _phoneSwitchNode.GetOutputPort);
            
            Type parentType = typeof(SwitchNode);
            _addStatPortMethod = parentType.GetMethod("AddDynamicPort", BindingFlags.NonPublic | BindingFlags.Instance);
            _removeStatPortMethod = parentType.GetMethod("RemoveStatDynamicPort", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        else
        {
            _switchNodeLogic.TryInitStatsNames();
            _switchNodeLogic.TryUpdateAdditionalCases();
            serializedObject.Update();
            NodeEditorGUILayout.PropertyField(_inputPortProperty);
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField($"Dynamic ports count: {_phoneSwitchNode.DynamicOutputs.Count()}");
            EditorGUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add CaseForStats"))
            {
                _addStatPortMethod.Invoke(_phoneSwitchNode, null);
                serializedObject.Update();
            }

            if (GUILayout.Button("RemoveStatPort"))
            {
                _removeStatPortMethod.Invoke(_phoneSwitchNode, null);
                serializedObject.Update();
            }

            EditorGUILayout.EndHorizontal();
            if (_phoneSwitchNode.GetSwitchNodeLogic?.Operators != null)
            {
                if (_casesForStatsListProperty != null && _casesForStatsListProperty.arraySize > 0)
                {
                    for (int i = 0; i < _casesForStatsListProperty.arraySize; i++)
                    {
                        if (i < _casesForStatsListProperty.arraySize)
                        {
                            NodePort port = _phoneSwitchNode.GetOutputPort(_casesForStatsListProperty.GetArrayElementAtIndex(i).FindPropertyRelative("_name").stringValue);
                            _switchNodeLogic.DrawCase(_phoneSwitchNode.CaseLocalizations[i], _casesForStatsListProperty.GetArrayElementAtIndex(i), port.fieldName, $"CaseForStats {i + 1} port");
                        }
                    }
                }
            }
            EditorGUILayout.Space(10f);
            _lineDrawer.DrawHorizontalLine(Color.yellow);
            NodeEditorGUILayout.PortField(new GUIContent("Default Port "), _phoneSwitchNode.OutputPortBaseNode);
            serializedObject.ApplyModifiedProperties();
        }
    }
}