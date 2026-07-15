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
    private SerializedProperty _inputPortProperty;
    
    private MyProject.EnumPopupDrawer _enumPopupDrawer;
    private bool _needsRepaint;

    public override void OnBodyGUI()
    {
        if (_mergerNode == null)
        {
            _mergerNode = target as MergerNode;
            _showNeedClickToSwitchNextSlideSerializedProperty = serializedObject.FindProperty("_autoSwitchToNextSlide");
            _enterAsyncModeSerializedProperty = serializedObject.FindProperty("_enterAsyncMode");
            _exitAsyncModeSerializedProperty = serializedObject.FindProperty("_exitAsyncMode");
            _inputPortProperty = serializedObject.FindProperty("Input");
            _enumPopupDrawer = new EnumPopupDrawer();
            return;
        }
        
        serializedObject.Update();
        
        EditorGUILayout.BeginVertical();
        
        try
        {
            NodeEditorGUILayout.PropertyField(_inputPortProperty);
            
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
                _needsRepaint = true;
                EditorApplication.delayCall += () => AddPort();
            }

            if (GUILayout.Button("Remove"))
            {
                _needsRepaint = true;
                EditorApplication.delayCall += () => RemovePort();
            }

            EditorGUILayout.EndHorizontal();
            
            var ports = _mergerNode.DynamicOutputs.ToList();
            if (ports.Count > 0)
            {
                EditorGUILayout.BeginVertical("box");
                foreach (var port in ports)
                {
                    DrawPort(port.fieldName);
                }
                EditorGUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
        catch (System.ArgumentException) {}
        
        EditorGUILayout.EndVertical();
        
        if (_needsRepaint)
        {
            _needsRepaint = false;
            NodeEditorWindow.current.Repaint();
        }
    }
    
    private void AddPort()
    {
        CallMethod(ref _addPortMethod, "AddDynamicPort");
        NodeEditorWindow.current.Repaint();
    }
    
    private void RemovePort()
    {
        CallMethod(ref _removePortMethod, "RemovePorts");
        NodeEditorWindow.current.Repaint();
    }

    private void CallMethod(ref MethodInfo methodInfo, string name)
    {
        if (methodInfo == null)
        {
            methodInfo = _mergerNode.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
        }
        methodInfo?.Invoke(_mergerNode, null);
    }
    
    private void DrawPort(string name)
    {
        var port = target.GetOutputPort(name);
        if (port == null) return;
        
        try
        {
            NodeEditorGUILayout.PortField(new GUIContent(name), port);
        }
        catch (System.ArgumentException) { }
    }
}