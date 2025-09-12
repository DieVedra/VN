using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(ChangeLookCustomCharacterNode))]
public class ChangeLookCustomCharacterNodeDrawer : NodeEditor
{
    private ChangeLookCustomCharacterNode _changeLookCustomCharacterNode;
    private SerializedProperty _skipToWardrobeVariantProperty;
    private SerializedProperty _customizationCharacterIndexProperty;
    private string[] _namesCharactersToPopup;
    private PopupDrawer _popupDrawer;

    public override void OnBodyGUI()
    {
        if (_changeLookCustomCharacterNode == null)
        {
            _changeLookCustomCharacterNode = target as ChangeLookCustomCharacterNode;
        }

        if (_skipToWardrobeVariantProperty == null)
        {
            _skipToWardrobeVariantProperty = serializedObject.FindProperty("_skipToWardrobeVariant");
            _customizationCharacterIndexProperty = serializedObject.FindProperty("_customizationCharacterIndex");
            _popupDrawer = new PopupDrawer();
        }
        InitCharactersNames();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));
        
        _popupDrawer.DrawPopup(_namesCharactersToPopup, _customizationCharacterIndexProperty);

        if (_skipToWardrobeVariantProperty.boolValue == false)
        {
            
        }
        
        
        
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("SkipToWardrobeVariant: ", GUILayout.Width(200f));
        _skipToWardrobeVariantProperty.boolValue = EditorGUILayout.Toggle(_skipToWardrobeVariantProperty.boolValue, GUILayout.Width(30f));
        EditorGUILayout.EndHorizontal();

        
        
        
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Output"));

    }
    private void InitCharactersNames()
    {
        if (_changeLookCustomCharacterNode.CustomizableCharacters != null)
        {
            var namesCharactersToPopup = new List<string>();
            for (int i = 0; i < _changeLookCustomCharacterNode.CustomizableCharacters.Count; i++)
            {
                if (_changeLookCustomCharacterNode.CustomizableCharacters[i] != null)
                {
                    namesCharactersToPopup.Add(_changeLookCustomCharacterNode.CustomizableCharacters[i].MyNameText);
                }
            }
            _namesCharactersToPopup = namesCharactersToPopup.ToArray();
        }
    }
}