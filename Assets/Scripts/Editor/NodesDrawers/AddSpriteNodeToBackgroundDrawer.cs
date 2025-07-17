using System.Collections.Generic;
using UnityEditor;
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
    private string[] _namesCharactersToPopup;
    private string[] _namesBackgroundToPopup;

    public override void OnBodyGUI()
    {
        _addSpriteNodeToBackground = target as AddSpriteNodeToBackground;
        if (_addSpriteNodeToBackground != null)
        {
            if (_serializedPropertyInputPort == null)
            {
                _serializedPropertyInputPort = serializedObject.FindProperty("Input");
                _serializedPropertyOutputPort = serializedObject.FindProperty("Output");
                _serializedPropertyColor = serializedObject.FindProperty("_color");
                _serializedPropertyPosition = serializedObject.FindProperty("_localPosition");
                _serializedPropertyIndexSprite = serializedObject.FindProperty("_indexSprite");
                _serializedPropertyIndexBackground = serializedObject.FindProperty("_indexBackground");
            }
            NodeEditorGUILayout.PropertyField(_serializedPropertyInputPort);
            NodeEditorGUILayout.PropertyField(_serializedPropertyOutputPort);

            _serializedPropertyColor.colorValue =
                EditorGUILayout.ColorField("Color: ", _serializedPropertyColor.colorValue);
            _serializedPropertyPosition.vector2Value =
                EditorGUILayout.Vector2Field("Position: ", _serializedPropertyPosition.vector2Value);
            
            InitPopup();
            _serializedPropertyIndexBackground.intValue = EditorGUILayout.Popup(_serializedPropertyIndexBackground.intValue,  _namesBackgroundToPopup);
            _serializedPropertyIndexSprite.intValue = EditorGUILayout.Popup(_serializedPropertyIndexSprite.intValue,  _namesCharactersToPopup);
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

            _namesCharactersToPopup = namesCharactersToPopup.ToArray();
        }
    }
}