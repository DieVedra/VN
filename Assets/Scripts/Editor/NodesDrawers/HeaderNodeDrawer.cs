using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(HeaderNode))]
public class HeaderNodeDrawer : NodeEditor
{
    private HeaderNode _headerNode;
    private SerializedProperty _spriteSerializedProperty;
    private SerializedProperty _text1SerializedProperty;
    private SerializedProperty _text2SerializedProperty;
    private SerializedProperty _color1SerializedProperty;
    private SerializedProperty _color2SerializedProperty;
    private SerializedProperty _textSize1SerializedProperty;
    private SerializedProperty _textSize2SerializedProperty;
    public override void OnBodyGUI()
    {
        if (_headerNode == null)
        {
            _headerNode = target as HeaderNode;
        }
        EditorGUI.BeginChangeCheck();

        serializedObject.Update();
        
        if (_spriteSerializedProperty == null)
        {
            _spriteSerializedProperty = serializedObject.FindProperty("_sprite");
            _text1SerializedProperty = serializedObject.FindProperty("_text1");
            _text2SerializedProperty = serializedObject.FindProperty("_text2");
            _color1SerializedProperty = serializedObject.FindProperty("_colorField1");
            _color2SerializedProperty = serializedObject.FindProperty("_colorField2");
            _textSize1SerializedProperty = serializedObject.FindProperty("_textSize1");
            _textSize2SerializedProperty = serializedObject.FindProperty("_textSize2");
        }
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Output"));
        
        // EditorGUILayout.LabelField("Text Chapter Title:");
        // _text1SerializedProperty.stringValue = EditorGUILayout.TextArea(_text1SerializedProperty.stringValue, GUILayout.Height(50f), GUILayout.Width(150f));
        DrawTextArea(_text1SerializedProperty, _color1SerializedProperty, _textSize1SerializedProperty,"Text Chapter Title:");
        DrawTextArea(_text2SerializedProperty, _color2SerializedProperty, _textSize2SerializedProperty,"Text Title:");
        // EditorGUILayout.LabelField("Text Title:");
        // _text2SerializedProperty.stringValue = EditorGUILayout.TextArea(_text2SerializedProperty.stringValue, GUILayout.Height(50f), GUILayout.Width(150f));
        //
        EditorGUILayout.LabelField("Sprite:");
        _spriteSerializedProperty.objectReferenceValue = EditorGUILayout.ObjectField(_spriteSerializedProperty.objectReferenceValue, typeof(Sprite), false);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawTextArea(SerializedProperty textSerializedProperty, SerializedProperty colorSerializedProperty,
        SerializedProperty textSizeSerializedProperty, string label)
    {
        // EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(label);
        textSerializedProperty.stringValue = EditorGUILayout.TextArea(textSerializedProperty.stringValue, GUILayout.Height(50f), GUILayout.Width(150f));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Text Color: ",GUILayout.Width(80f));
        colorSerializedProperty.colorValue = EditorGUILayout.ColorField(colorSerializedProperty.colorValue, GUILayout.Width(40f));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("TextSize: ");
        textSizeSerializedProperty.intValue = EditorGUILayout.IntField(textSizeSerializedProperty.intValue, GUILayout.Width(40f));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        // EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10f);
    }
}