using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(SmoothTransitionNode))]
public class SmoothTransitionNodeDrawer : NodeEditor
{
    private readonly string _labelStartCurtain = "Is Start CanvasGroup: ";
    private readonly string _labelEndCurtain = "Is End CanvasGroup: ";
    
    private SmoothTransitionNode _smoothTransitionNode;
    private SerializedProperty _isStartCurtainSerializedProperty;
    private SerializedProperty _isEndCurtainSerializedProperty;
    public override void OnBodyGUI()
    {
        if (_smoothTransitionNode == null)
        {
            _smoothTransitionNode = target as SmoothTransitionNode;
        }
        serializedObject.Update();
        if (_isStartCurtainSerializedProperty == null)
        {
            _isStartCurtainSerializedProperty = serializedObject.FindProperty("_isStartCurtain");
            _isEndCurtainSerializedProperty = serializedObject.FindProperty("_isEndCurtain");
        }

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Output"));
        EditorGUI.BeginChangeCheck();
        LineDrawer.DrawHorizontalLine(Color.green);

        EditorGUILayout.Space(20f);
        if (_isEndCurtainSerializedProperty.boolValue == false)
        {
            DrawToggle(_isStartCurtainSerializedProperty, _isEndCurtainSerializedProperty,_labelStartCurtain);
        }

        if (_isStartCurtainSerializedProperty.boolValue == false)
        {
            DrawToggle(_isEndCurtainSerializedProperty, _isStartCurtainSerializedProperty, _labelEndCurtain);
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawToggle(SerializedProperty serializedProperty1, SerializedProperty serializedProperty2, string label)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label);
        serializedProperty1.boolValue = EditorGUILayout.Toggle(serializedProperty1.boolValue);
        serializedProperty2.boolValue = false;
        EditorGUILayout.EndHorizontal();
    }
}