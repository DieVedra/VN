using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(BackgroundNode))]
public class BackgroundNodeDrawer : NodeEditor
{
    private BackgroundNode _backgroundNode;
    private List<string> _namesToPopup;
    private MethodInfo _privateMethodOnSetBackground;
    private SerializedProperty _isSmoothCurtainSerializedProperty;

    public override void OnBodyGUI()
    {
        _backgroundNode = target as BackgroundNode;
        serializedObject.Update();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Output"));
        
        if (_isSmoothCurtainSerializedProperty == null)
        {
            _isSmoothCurtainSerializedProperty = serializedObject.FindProperty("_isSmoothCurtain");
        }
        
        if (_backgroundNode != null)
        {
            if (_backgroundNode.Backgrounds != null && _backgroundNode.Backgrounds.Count > 0)
            {
                EditorGUI.BeginChangeCheck();
                DrawPopup();
                DrawEnumPopup();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("IsSmoothCurtain: ");
                _isSmoothCurtainSerializedProperty.boolValue = EditorGUILayout.Toggle(_isSmoothCurtainSerializedProperty.boolValue);
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    SetBackground();
                }

            }
        }
    }

    private void DrawPopup()
    {
        if (_backgroundNode.Backgrounds != null)
        {
            _namesToPopup = new List<string>();
            foreach (BackgroundContent content in _backgroundNode.Backgrounds)
            {
                if (content != null)
                {
                    _namesToPopup.Add(content.name);
                }
            }
            SerializedProperty serializedPropertyValue = serializedObject.FindProperty("_index");
            GUIContent arrayLabel = new GUIContent("Current: ");
            serializedPropertyValue.intValue = EditorGUILayout.Popup(arrayLabel, serializedPropertyValue.intValue,  _namesToPopup.ToArray());
        }
    }

    private void DrawEnumPopup()
    {
        SerializedProperty enumType = serializedObject.FindProperty("_backgroundPosition");
        BackgroundPosition currentEnumType = (BackgroundPosition)enumType.enumValueIndex;
        GUIContent arrayLabel = new GUIContent("Current Pos: ");
        currentEnumType = (BackgroundPosition)EditorGUILayout.EnumPopup(arrayLabel, currentEnumType);
        enumType.enumValueIndex = (int)currentEnumType;
    }

    private void SetBackground()
    {
        if (_privateMethodOnSetBackground == null)
        {
            _privateMethodOnSetBackground =  _backgroundNode.GetType().GetMethod("SetInfoToView", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        _privateMethodOnSetBackground.Invoke(_backgroundNode, null);
    }
}