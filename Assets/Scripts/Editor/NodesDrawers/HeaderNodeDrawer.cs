using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(HeaderNode))]
public class HeaderNodeDrawer : NodeEditor
{
    private const int _maxCountSymbols = 200;
    private HeaderNode _headerNode;
    private SerializedProperty _spriteSerializedProperty;
    private SerializedProperty _text1SerializedProperty;
    private SerializedProperty _text2SerializedProperty;
    private SerializedProperty _color1SerializedProperty;
    private SerializedProperty _color2SerializedProperty;
    private SerializedProperty _textSize1SerializedProperty;
    private SerializedProperty _textSize2SerializedProperty;
    private SerializedProperty _indexBackgroundSerializedProperty;
    private SerializedProperty _inputSerializedProperty;
    private SerializedProperty _outputSerializedProperty;
    private SerializedProperty _backgroundPositionValueSerializedProperty;
    private LocalizationStringTextDrawer _localizationStringTextDrawer;
    private LocalizationString _localizationText1;
    private LocalizationString _localizationText2;


    private string[] _backgroundsNames;

    public override void OnBodyGUI()
    {
        if (_headerNode == null)
        {
            _headerNode = target as HeaderNode;
        }
        EditorGUI.BeginChangeCheck();

        serializedObject.Update();
        if (_localizationStringTextDrawer == null)
        {            
            _localizationStringTextDrawer = new LocalizationStringTextDrawer(new SimpleTextValidator(_maxCountSymbols));
        }
        if (_spriteSerializedProperty == null)
        {
            _spriteSerializedProperty = serializedObject.FindProperty("_sprite");
            _text1SerializedProperty = serializedObject.FindProperty("_localizationText1");
            _text2SerializedProperty = serializedObject.FindProperty("_localizationText2");
            _color1SerializedProperty = serializedObject.FindProperty("_colorField1");
            _color2SerializedProperty = serializedObject.FindProperty("_colorField2");
            _textSize1SerializedProperty = serializedObject.FindProperty("_textSize1");
            _textSize2SerializedProperty = serializedObject.FindProperty("_textSize2");
            _indexBackgroundSerializedProperty = serializedObject.FindProperty("_indexBackground");
            _inputSerializedProperty = serializedObject.FindProperty("Input");
            _outputSerializedProperty = serializedObject.FindProperty("Output");
            _backgroundPositionValueSerializedProperty = serializedObject.FindProperty("_backgroundPositionValue");
            _localizationText1 = _localizationStringTextDrawer.GetLocalizationStringFromProperty(_text1SerializedProperty);
            _localizationText2 = _localizationStringTextDrawer.GetLocalizationStringFromProperty(_text2SerializedProperty);
        }
        InitNames();
        NodeEditorGUILayout.PropertyField(_inputSerializedProperty);
        NodeEditorGUILayout.PropertyField(_outputSerializedProperty);
        DrawTextArea(_localizationText1, _color1SerializedProperty, _textSize1SerializedProperty,"Text Chapter Title:");
        DrawTextArea(_localizationText2, _color2SerializedProperty, _textSize2SerializedProperty,"Text Title:");
        DrawPopup();
        DrawSlider();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawTextArea(LocalizationString localizationString, SerializedProperty colorSerializedProperty,
        SerializedProperty textSizeSerializedProperty, string label)
    {
        // EditorGUILayout.LabelField(label);
        // textSerializedProperty.stringValue = EditorGUILayout.TextArea(textSerializedProperty.stringValue, GUILayout.Height(50f), GUILayout.Width(150f));
        _localizationStringTextDrawer.DrawTextField(localizationString, label);
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
        
        EditorGUILayout.Space(10f);
    }

    private void DrawPopup()
    {
        if (_headerNode.Backgrounds == null || _headerNode.Backgrounds.Count == 0)
        {
            return;
        }
        _indexBackgroundSerializedProperty.intValue =
            EditorGUILayout.Popup(_indexBackgroundSerializedProperty.intValue, _backgroundsNames);
    }

    private void DrawSlider()
    {
        EditorGUILayout.Space(10f);

        EditorGUILayout.LabelField("PositionBackground: ");

        _backgroundPositionValueSerializedProperty.floatValue 
            = GUILayout.HorizontalSlider(_backgroundPositionValueSerializedProperty.floatValue, 0f, 1f, GUILayout.Width(170f));
        EditorGUILayout.Space(30f);

    }
    private void InitNames()
    {
        if (_headerNode.Backgrounds != null && _headerNode.Backgrounds.Count > 0)
        {
            List<string> backgroundsNames = new List<string>();
            for (int i = 0; i < _headerNode.Backgrounds.Count; i++)
            {
                if (_headerNode.Backgrounds[i] != null)
                {
                    backgroundsNames.Add(_headerNode.Backgrounds[i].name);
                }
            }

            _backgroundsNames = backgroundsNames.ToArray();
        }
    }
}