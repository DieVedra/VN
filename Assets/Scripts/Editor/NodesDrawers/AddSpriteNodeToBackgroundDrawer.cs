using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(AddSpriteNodeToBackground))]
public class AddSpriteNodeToBackgroundDrawer : NodeEditor
{
    private AddSpriteNodeToBackground _addSpriteNodeToBackground;

    private SerializedProperty _serializedPropertyColor;
    private SerializedProperty _serializedPropertyPosition;
    private SerializedProperty _serializedPropertyIndexSprite;
    private SerializedProperty _serializedPropertyIndexBackground;
    private SerializedProperty _serializedPropertyInputPort;
    private SerializedProperty _serializedPropertyOutputPort;
    
    private SerializedProperty _addToBackgroundProperty;
    private SerializedProperty _removeFromBackgroundProperty;
    private SerializedProperty _indexRemoveSpriteProperty;
    private SerializedProperty _indexBackgroundToRemoveProperty;
    private string[] _namesSpritesToPopup;
    private string[] _namesBackgroundToPopup;
    private bool _initPopupInFrame;
    public override void OnBodyGUI()
    {
        _addSpriteNodeToBackground = target as AddSpriteNodeToBackground;
        if (_addSpriteNodeToBackground != null)
        {
            _initPopupInFrame = false;

            if (_serializedPropertyInputPort == null)
            {
                _serializedPropertyInputPort = serializedObject.FindProperty("Input");
                _serializedPropertyOutputPort = serializedObject.FindProperty("Output");
                _serializedPropertyColor = serializedObject.FindProperty("_color");
                _serializedPropertyPosition = serializedObject.FindProperty("_localPosition");
                _serializedPropertyIndexSprite = serializedObject.FindProperty("_indexSprite");
                _serializedPropertyIndexBackground = serializedObject.FindProperty("_indexBackground");
                
                _addToBackgroundProperty = serializedObject.FindProperty("_addToBackground");
                _removeFromBackgroundProperty = serializedObject.FindProperty("_removeFromBackground");
                _indexRemoveSpriteProperty = serializedObject.FindProperty("_indexRemoveSprite");
                _indexBackgroundToRemoveProperty = serializedObject.FindProperty("_indexBackgroundToRemove");
            }
            NodeEditorGUILayout.PropertyField(_serializedPropertyInputPort);
            NodeEditorGUILayout.PropertyField(_serializedPropertyOutputPort);

            _serializedPropertyColor.colorValue =
                EditorGUILayout.ColorField("Color: ", _serializedPropertyColor.colorValue);
            _serializedPropertyPosition.vector2Value =
                EditorGUILayout.Vector2Field("Position: ", _serializedPropertyPosition.vector2Value);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Add To Background:", GUILayout.Width(200f));
            _addToBackgroundProperty.boolValue = EditorGUILayout.Toggle(_addToBackgroundProperty.boolValue);
            EditorGUILayout.EndHorizontal();
            
            if (_addToBackgroundProperty.boolValue)
            {
                if (_initPopupInFrame == false)
                {
                    InitPopup();
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Add to:");
                _serializedPropertyIndexBackground.intValue = EditorGUILayout.Popup(_serializedPropertyIndexBackground.intValue,  _namesBackgroundToPopup);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sprite:");
                _serializedPropertyIndexSprite.intValue = EditorGUILayout.Popup(_serializedPropertyIndexSprite.intValue,  _namesSpritesToPopup);
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Remove From Background:", GUILayout.Width(200f));
            _removeFromBackgroundProperty.boolValue = EditorGUILayout.Toggle(_removeFromBackgroundProperty.boolValue);
            EditorGUILayout.EndHorizontal();
            
            if (_removeFromBackgroundProperty.boolValue)
            {
                if (_initPopupInFrame == false)
                {
                    InitPopup();
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Remove from:");
                _indexBackgroundToRemoveProperty.intValue = EditorGUILayout.Popup(_indexBackgroundToRemoveProperty.intValue,  _namesBackgroundToPopup);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sprite:");
                _indexRemoveSpriteProperty.intValue = EditorGUILayout.Popup(_indexRemoveSpriteProperty.intValue,  _namesSpritesToPopup);
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void InitPopup()
    {
        if (_addSpriteNodeToBackground.Backgrounds != null)
        {
            List<string> namesBackgroundToPopup = new List<string>();
            if (_addSpriteNodeToBackground.Backgrounds != null)
            {
                for (int i = 0; i < _addSpriteNodeToBackground.Backgrounds.Count; ++i)
                {
                    if (_addSpriteNodeToBackground.Backgrounds[i] != null)
                    {
                        namesBackgroundToPopup.Add(_addSpriteNodeToBackground.Backgrounds[i].name);
                    }
                }
            }

            _namesBackgroundToPopup = namesBackgroundToPopup.ToArray();
        }

        if (_addSpriteNodeToBackground.AdditionalImagesToBackground != null)
        {
            List<string> namesCharactersToPopup = new List<string>();
            if (_addSpriteNodeToBackground.AdditionalImagesToBackground != null)
            {
                for (int i = 0; i < _addSpriteNodeToBackground.AdditionalImagesToBackground.Count; ++i)
                {
                    if (_addSpriteNodeToBackground.AdditionalImagesToBackground[i] != null)
                    {
                        namesCharactersToPopup.Add(_addSpriteNodeToBackground.AdditionalImagesToBackground[i].name);
                    }
                }
            }

            _namesSpritesToPopup = namesCharactersToPopup.ToArray();
            _initPopupInFrame = true;
        }
    }
}