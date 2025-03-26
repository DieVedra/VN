using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(SwitchToAnotherNodeGraphNode))]
public class SwitchToAnotherNodeGraphNodeDrawer : NodeEditor
{
    private SwitchToAnotherNodeGraphNode _switchToAnotherNodeGraphNode;
    private SerializedProperty _serializedProperty;
    private SerializedProperty _serializedPropertyPutOnSwimsuit;
    public override void OnBodyGUI()
    {
        if (_switchToAnotherNodeGraphNode == null)
        {
            _switchToAnotherNodeGraphNode = target as SwitchToAnotherNodeGraphNode;
        }

        if (_serializedProperty == null)
        {
            _serializedProperty = serializedObject.FindProperty("_nextLevelPartNodeGraph");
            _serializedPropertyPutOnSwimsuit = serializedObject.FindProperty("_putOnSwimsuit");
        }
        serializedObject.Update();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("NextNodeGraph");
        _serializedProperty.objectReferenceValue = EditorGUILayout.ObjectField(_serializedProperty.objectReferenceValue, typeof(LevelPartNodeGraph), true, GUILayout.Width(180f));
        
        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("Put On Swimsuit Character");
        _serializedPropertyPutOnSwimsuit.boolValue = EditorGUILayout.Toggle(_serializedPropertyPutOnSwimsuit.boolValue);
        
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();

    }
}