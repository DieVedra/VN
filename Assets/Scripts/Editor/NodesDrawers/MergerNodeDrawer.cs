
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(MergerNode))]
public class MergerNodeDrawer : NodeEditor
{
    private MergerNode _mergerNode;
    private MethodInfo _addPortMethod;
    private MethodInfo _removePortMethod;
    private SerializedProperty _showNeedClickToSwitchNextSlideSerializedProperty;

    public override void OnBodyGUI()
    {
        if (_mergerNode == null)
        {
            _mergerNode = target as MergerNode;
        }
        serializedObject.Update();

        if (_showNeedClickToSwitchNextSlideSerializedProperty == null)
        {
            _showNeedClickToSwitchNextSlideSerializedProperty = serializedObject.FindProperty("_autoSwitchToNextSlide");
        }
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));
        EditorGUI.BeginChangeCheck();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("AutoSwitchToNextSlide: ");
        _showNeedClickToSwitchNextSlideSerializedProperty.boolValue = EditorGUILayout.Toggle(_showNeedClickToSwitchNextSlideSerializedProperty.boolValue);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField($"Dynamic ports count: {_mergerNode.DynamicOutputs.Count()}");
        EditorGUILayout.Space(10f);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Output"))
        {
            AddPort();
        }

        if (GUILayout.Button("Remove"))
        {
            RemovePort();
        }
        EditorGUILayout.EndHorizontal();

        if (_mergerNode.DynamicOutputs.Count() > 0)
        {
            foreach (var port in _mergerNode.DynamicOutputs)
            {
                DrawPort(port.fieldName);
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
    private void AddPort()
    {
        CallMethod(ref _addPortMethod, "AddDynamicPort");
    }
    private void RemovePort()
    {
        CallMethod(ref _removePortMethod, "RemovePorts");
    }

    private void CallMethod(ref MethodInfo methodInfo, string name)
    {
        if (methodInfo == null)
        {
            methodInfo = _mergerNode.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
        }
        methodInfo.Invoke(_mergerNode, null);
    }
    private void DrawPort(string name)
    {
        NodeEditorGUILayout.PortField(new GUIContent(name), target.GetOutputPort(name));
    }
}