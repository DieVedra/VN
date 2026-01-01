using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(HeaderNode))]
public class HeaderNodeDrawer : NodeEditor
{
    private const int _maxCountSymbols = 200;
    private HeaderNode _headerNode;
    private SerializedProperty _text1SerializedProperty;
    private SerializedProperty _text2SerializedProperty;
    private SerializedProperty _color1SerializedProperty;
    private SerializedProperty _color2SerializedProperty;
    private SerializedProperty _textSize1SerializedProperty;
    private SerializedProperty _textSize2SerializedProperty;
    private SerializedProperty _keyBackgroundSerializedProperty;
    private SerializedProperty _backgroundPositionValueSerializedProperty;
    private SerializedProperty _indexHeaderAudioSerializedProperty;
    private SerializedProperty _playHeaderAudioSerializedProperty;
    private SerializedProperty _inputPortSerializedProperty;
    private SerializedProperty _outputPortSerializedProperty;
    private LocalizationStringTextDrawer _localizationStringTextDrawer;
    private LocalizationString _localizationText1;
    private LocalizationString _localizationText2;
    private MethodInfo _privateMethod;

    private List<string> _namesToPopup;

    private string[] _audioNames;
    private int _currentIndex;
    private int _index;
    public override void OnBodyGUI()
    {
        if (_headerNode == null)
        {
            _headerNode = target as HeaderNode;
            _text1SerializedProperty = serializedObject.FindProperty("_localizationText1");
            _text2SerializedProperty = serializedObject.FindProperty("_localizationText2");
            _color1SerializedProperty = serializedObject.FindProperty("_colorField1");
            _color2SerializedProperty = serializedObject.FindProperty("_colorField2");
            _textSize1SerializedProperty = serializedObject.FindProperty("_textSize1");
            _textSize2SerializedProperty = serializedObject.FindProperty("_textSize2");
            _keyBackgroundSerializedProperty = serializedObject.FindProperty("_keyBackground");
            _backgroundPositionValueSerializedProperty = serializedObject.FindProperty("_backgroundPositionValue");
            _playHeaderAudioSerializedProperty = serializedObject.FindProperty("_playHeaderAudio");
            _indexHeaderAudioSerializedProperty = serializedObject.FindProperty("_indexHeaderAudio");
            _inputPortSerializedProperty = serializedObject.FindProperty("Input");
            _outputPortSerializedProperty = serializedObject.FindProperty("Output");
            _localizationStringTextDrawer = new LocalizationStringTextDrawer(new SimpleTextValidator(_maxCountSymbols));
            _localizationText1 = _localizationStringTextDrawer.GetLocalizationStringFromProperty(_text1SerializedProperty);
            _localizationText2 = _localizationStringTextDrawer.GetLocalizationStringFromProperty(_text2SerializedProperty);
        }
        else
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            NodeEditorGUILayout.PropertyField(_inputPortSerializedProperty);
            NodeEditorGUILayout.PropertyField(_outputPortSerializedProperty);
            DrawTextArea(_localizationText1, _color1SerializedProperty, _textSize1SerializedProperty,"Text Chapter Title:");
            DrawTextArea(_localizationText2, _color2SerializedProperty, _textSize2SerializedProperty,"Text Title:");
            DrawPopup();
            DrawSlider();
            DrawAudio();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                SetInfoToView();
            }
        }
    }

    private void DrawTextArea(LocalizationString localizationString, SerializedProperty colorSerializedProperty,
        SerializedProperty textSizeSerializedProperty, string label)
    {
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
        if (_headerNode.BackgroundsDictionary != null && _headerNode.BackgroundsDictionary.Count > 0)
        {
            if (_namesToPopup == null)
            {
                _namesToPopup = new List<string>();
            }
            else
            {
                _namesToPopup.Clear();
            }
            _index = 0;
            _currentIndex = 0;
            foreach (var pair in _headerNode.BackgroundsDictionary)
            {
                if (pair.Value != null)
                {
                    if (pair.Value?.name == _keyBackgroundSerializedProperty.stringValue)
                    {
                        _currentIndex = _index;
                    }

                    _namesToPopup.Add(pair.Value.name);
                    _index++;
                }
            }
        
            EditorGUI.BeginChangeCheck();
            _currentIndex = EditorGUILayout.Popup(_currentIndex,  _namesToPopup.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                if (_namesToPopup.Count > _currentIndex)
                {
                    _keyBackgroundSerializedProperty.stringValue = _namesToPopup[_currentIndex];
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }

    private void DrawSlider()
    {
        EditorGUILayout.Space(10f);

        EditorGUILayout.LabelField("PositionBackground: ");

        _backgroundPositionValueSerializedProperty.floatValue 
            = GUILayout.HorizontalSlider(_backgroundPositionValueSerializedProperty.floatValue, 0f, 1f, GUILayout.Width(170f));
        EditorGUILayout.Space(30f);

    }

    private void DrawAudio()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("PlayHeaderAudio: ", GUILayout.Width(120f));
        _playHeaderAudioSerializedProperty.boolValue = EditorGUILayout.Toggle( _playHeaderAudioSerializedProperty.boolValue);
        EditorGUILayout.EndHorizontal();
        if (_playHeaderAudioSerializedProperty.boolValue)
        {
            InitAudioNames();
            if (_audioNames != null && _audioNames.Length > 0)
            {
                _indexHeaderAudioSerializedProperty.intValue =
                    EditorGUILayout.Popup(_indexHeaderAudioSerializedProperty.intValue, _audioNames);
            }
        }
    }

    private void InitAudioNames()
    {
        if (_headerNode.Sound != null && _headerNode.Sound.Clips.Count > 0)
        {
            List<string> audioNames = new List<string>();
            for (int i = 0; i < _headerNode.Sound.Clips.Count; i++)
            {
                if (_headerNode.Sound.Clips[i] != null)
                {
                    audioNames.Add(_headerNode.Sound.Clips[i].name);
                }
            }

            _audioNames = audioNames.ToArray();
        }
    }
    private void SetInfoToView()
    {
        if (_privateMethod == null)
        {
            _privateMethod = _headerNode.GetType().GetMethod("SetInfoToView", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        _privateMethod.Invoke(_headerNode, null);
    }
}