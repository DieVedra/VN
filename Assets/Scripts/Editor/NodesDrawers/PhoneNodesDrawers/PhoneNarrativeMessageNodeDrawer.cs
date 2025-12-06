using UnityEditor;
using UnityEngine;

[CustomNodeEditor(typeof(PhoneNarrativeMessageNode))]

public class PhoneNarrativeMessageNodeDrawer : NarrativeNodeDrawer
{
    private PhoneNarrativeMessageNode _phoneNarrativeMessageNode;
    private SerializedProperty _isReadedTypeProperty;

    public override void OnBodyGUI()
    {
        if (_phoneNarrativeMessageNode == null)
        {
            _phoneNarrativeMessageNode = target as PhoneNarrativeMessageNode;
            _isReadedTypeProperty = serializedObject.FindProperty("_isReaded");

        }
        base.OnBodyGUI();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is Readed: ", GUILayout.Width(60f));
        _isReadedTypeProperty.boolValue = EditorGUILayout.Toggle(_isReadedTypeProperty.boolValue);
        EditorGUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }
}