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
    private SerializedProperty _serializedPropertyInputPort;
    private SerializedProperty _serializedPropertyOutputPort;

    private SerializedProperty _addToBackgroundProperty;
    private SerializedProperty _removeFromBackgroundProperty;

    private SerializedProperty _serializedPropertyKeySprite;
    private SerializedProperty _serializedPropertyKeyBackground;
    
    private int _currentIndexSprite;
    private int _currentIndexBackground;
    private int _currentIndexRemoveSprite;
    private int _currentIndexRemoveBackground;
    
    private int _indexSprite;
    private int _indexBackground;
    private List<string> _namesBackgroundToPopup;
    private List<string> _namesSpritesToPopup;

    public override void OnBodyGUI()
    {
        if (_addSpriteNodeToBackground == null)
        {
            _addSpriteNodeToBackground = target as AddSpriteNodeToBackground;
            _serializedPropertyInputPort = serializedObject.FindProperty("Input");
            _serializedPropertyOutputPort = serializedObject.FindProperty("Output");
            _serializedPropertyColor = serializedObject.FindProperty("_color");
            _serializedPropertyPosition = serializedObject.FindProperty("_localPosition");
            _addToBackgroundProperty = serializedObject.FindProperty("_addToBackground");
            _removeFromBackgroundProperty = serializedObject.FindProperty("_removeFromBackground");
            _serializedPropertyKeySprite = serializedObject.FindProperty("_keySprite");
            _serializedPropertyKeyBackground = serializedObject.FindProperty("_keyBackground");
            _namesBackgroundToPopup = new List<string>();
            _namesSpritesToPopup = new List<string>();
        }
        else
        {
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
                _removeFromBackgroundProperty.boolValue = false;
                InitPopup();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Add to:");
                EditorGUI.BeginChangeCheck();
                _currentIndexBackground = EditorGUILayout.Popup(_currentIndexBackground,  _namesBackgroundToPopup.ToArray());
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sprite:");
                _currentIndexSprite = EditorGUILayout.Popup(_currentIndexSprite,  _namesSpritesToPopup.ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    _serializedPropertyKeyBackground.stringValue = _namesBackgroundToPopup[_currentIndexBackground];
                    _serializedPropertyKeySprite.stringValue = _namesSpritesToPopup[_currentIndexSprite];
                    serializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Remove From Background:", GUILayout.Width(200f));
            _removeFromBackgroundProperty.boolValue = EditorGUILayout.Toggle(_removeFromBackgroundProperty.boolValue);
            EditorGUILayout.EndHorizontal();
            
            if (_removeFromBackgroundProperty.boolValue)
            {
                _addToBackgroundProperty.boolValue = false;
                InitPopup();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Remove from:");
                EditorGUI.BeginChangeCheck();
                _currentIndexRemoveBackground = EditorGUILayout.Popup(_currentIndexRemoveBackground,  _namesBackgroundToPopup.ToArray());
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sprite:");
                _currentIndexRemoveSprite = EditorGUILayout.Popup(_currentIndexRemoveSprite, _namesSpritesToPopup.ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    _serializedPropertyKeyBackground.stringValue = _namesBackgroundToPopup[_currentIndexBackground];
                    _serializedPropertyKeySprite.stringValue = _namesSpritesToPopup[_currentIndexSprite];
                    serializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void InitPopup()
    {
        _indexSprite = _indexBackground = 0;

        if (_addSpriteNodeToBackground.GetBackgroundContentDictionary != null)
        {
            _namesBackgroundToPopup.Clear();
            foreach (var pair in _addSpriteNodeToBackground.GetBackgroundContentDictionary)
            {
                if (pair.Value != null)
                {
                    _namesBackgroundToPopup.Add(pair.Key);
                    if (_serializedPropertyKeyBackground.stringValue == pair.Key)
                    {
                        _currentIndexBackground = _indexBackground;
                    }
                    _indexBackground++;
                }
            }
        }

        if (_addSpriteNodeToBackground.GetAdditionalImagesToBackgroundDictionary != null)
        {
            _namesSpritesToPopup.Clear();
            foreach (var pair in _addSpriteNodeToBackground.GetAdditionalImagesToBackgroundDictionary)
            {
                if (pair.Value != null)
                {
                    _namesSpritesToPopup.Add(pair.Key);
                    if (_serializedPropertyKeySprite.stringValue == pair.Key)
                    {
                        _currentIndexSprite = _indexSprite;
                    }
                    _indexSprite++;
                }
            }
        }
    }
}