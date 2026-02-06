
using System.Linq;
using System.Reflection;
using MyProject;
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
    private SerializedProperty _enterAsyncModeSerializedProperty;
    private SerializedProperty _exitAsyncModeSerializedProperty;
    
    private MyProject.EnumPopupDrawer _enumPopupDrawer;

    public override void OnBodyGUI()
    {
        if (_mergerNode == null)
        {
            _mergerNode = target as MergerNode;
            _showNeedClickToSwitchNextSlideSerializedProperty = serializedObject.FindProperty("_autoSwitchToNextSlide");
            _enterAsyncModeSerializedProperty = serializedObject.FindProperty("_enterAsyncMode");
            _exitAsyncModeSerializedProperty = serializedObject.FindProperty("_exitAsyncMode");
            _enumPopupDrawer = new EnumPopupDrawer();
        }
        else
        {
            serializedObject.Update();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("AutoSwitchToNextSlide: ");
            _showNeedClickToSwitchNextSlideSerializedProperty.boolValue = EditorGUILayout.Toggle(_showNeedClickToSwitchNextSlideSerializedProperty.boolValue);
            EditorGUILayout.EndHorizontal();
            
            _enumPopupDrawer.DrawEnumPopup<AsyncMode>(_enterAsyncModeSerializedProperty, "EnterAsyncMode");
            _enumPopupDrawer.DrawEnumPopup<AsyncMode>(_exitAsyncModeSerializedProperty, "ExitAsyncMode");
            
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