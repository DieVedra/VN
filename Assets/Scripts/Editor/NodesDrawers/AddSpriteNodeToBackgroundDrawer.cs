using System;
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

    private SerializedProperty _serializedPropertyKeySprite;
    private SerializedProperty _serializedPropertyKeyBackground;
    private SerializedProperty _keyRemoveSpriteProperty;
    private SerializedProperty _keyBackgroundToRemoveProperty;
    
    private int _currentIndexSprite;
    private int _currentIndexBackground;
    private int _currentIndexRemoveSprite;
    private int _currentIndexRemoveBackground;
    
    private int _indexSprite;
    private int _indexBackground;
    private int _indexRemoveSprite;
    private int _indexRemoveBackground;
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


            _serializedPropertyIndexSprite = serializedObject.FindProperty("_indexSprite");

            _serializedPropertyIndexBackground = serializedObject.FindProperty("_indexBackground");

            _addToBackgroundProperty = serializedObject.FindProperty("_addToBackground");
            _removeFromBackgroundProperty = serializedObject.FindProperty("_removeFromBackground");

            _indexRemoveSpriteProperty = serializedObject.FindProperty("_indexRemoveSprite");
            _indexBackgroundToRemoveProperty = serializedObject.FindProperty("_indexBackgroundToRemove");
            
            _serializedPropertyKeySprite = serializedObject.FindProperty("_keySprite");
            _serializedPropertyKeyBackground = serializedObject.FindProperty("_keyBackground");
            _keyRemoveSpriteProperty = serializedObject.FindProperty("_keyRemoveSprite");
            _keyBackgroundToRemoveProperty = serializedObject.FindProperty("_keyBackgroundToRemove");
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
                _indexSprite = _indexBackground = 0;
                InitPopup(_serializedPropertyKeyBackground, _serializedPropertyKeySprite, a,b);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Add to:");
                EditorGUI.BeginChangeCheck();
                _currentIndexBackground = EditorGUILayout.Popup(_currentIndexBackground,  _namesBackgroundToPopup.ToArray());
                // _serializedPropertyIndexBackground.intValue = EditorGUILayout.Popup(_serializedPropertyIndexBackground.intValue,  _namesBackgroundToPopup);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sprite:");
                _currentIndexSprite = EditorGUILayout.Popup(_currentIndexSprite,  _namesSpritesToPopup.ToArray());
                // _serializedPropertyIndexSprite.intValue = EditorGUILayout.Popup(_serializedPropertyIndexSprite.intValue,  _namesSpritesToPopup);
                if (EditorGUI.EndChangeCheck())
                {
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
                _indexRemoveBackground = _indexRemoveSprite = 0;
                InitPopup(_keyBackgroundToRemoveProperty, _keyRemoveSpriteProperty, c,d);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Remove from:");
                EditorGUI.BeginChangeCheck();
                _currentIndexRemoveBackground = EditorGUILayout.Popup(_currentIndexRemoveBackground,  _namesBackgroundToPopup.ToArray());
                // _indexBackgroundToRemoveProperty.intValue = EditorGUILayout.Popup(_indexBackgroundToRemoveProperty.intValue,  _namesBackgroundToPopup.ToArray());
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sprite:");
                _currentIndexRemoveSprite = EditorGUILayout.Popup(_currentIndexRemoveSprite, _namesSpritesToPopup.ToArray());
                // _indexRemoveSpriteProperty.intValue = EditorGUILayout.Popup(_indexRemoveSpriteProperty.intValue, _namesSpritesToPopup.ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void InitPopup(SerializedProperty serializedPropertyKeyBackground, SerializedProperty serializedPropertyKeySprite,
        Action<string, string> operation1, Action<string, string> operation2)
    {
        if (_addSpriteNodeToBackground.GetBackgroundContentDictionary != null)
        {
            _namesBackgroundToPopup.Clear();
            foreach (var pair in _addSpriteNodeToBackground.GetBackgroundContentDictionary)
            {
                Debug.Log($"{pair.Key} {pair.Value}");
                if (pair.Value != null)
                {
                    _namesBackgroundToPopup.Add(pair.Key);
                    operation1.Invoke(serializedPropertyKeyBackground.stringValue, pair.Key);
                }
            }
            Debug.Log($"_currentIndexBackground {_currentIndexBackground}");
        }

        if (_addSpriteNodeToBackground.GetAdditionalImagesToBackgroundDictionary != null)
        {
            _namesSpritesToPopup.Clear();
            foreach (var pair in _addSpriteNodeToBackground.GetAdditionalImagesToBackgroundDictionary)
            {
                if (pair.Value != null)
                {
                    _namesSpritesToPopup.Add(pair.Key);
                    operation2.Invoke(serializedPropertyKeySprite.stringValue, pair.Key);
                }
            }
        }
    }

    private void a(string keyField, string key)
    {
        if (key == keyField)
        {
            _currentIndexBackground = _indexBackground;
        }
        _indexBackground++;
    }

    private void b(string keyField, string key)
    {
        if (key == keyField)
        {
            _currentIndexSprite = _indexSprite;
        }
        _indexSprite++;
    }
    private void c(string keyField, string key)
    {
        if (key == keyField)
        {
            _currentIndexRemoveBackground = _indexRemoveBackground;
        }
        _indexRemoveBackground++;
    }

    private void d(string keyField, string key)
    {
        if (key == keyField)
        {
            _currentIndexRemoveSprite = _indexRemoveSprite;
        }
        _indexRemoveSprite++;
    }
}