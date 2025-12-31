using System.Collections.Generic;
using System.Reflection;
using MyProject;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(BackgroundNode))]
public class BackgroundNodeDrawer : NodeEditor
{
    private BackgroundNode _backgroundNode;
    private List<string> _namesToPopup;
    private MethodInfo _privateMethodOnSetBackground;
    private SerializedProperty _isSmoothCurtainSerializedProperty;
    private SerializedProperty _keySerializedProperty;
    private SerializedProperty _keyToSerializedProperty;
    private SerializedProperty _backgroundNodeModeSerializedProperty;
    private SerializedProperty _inputPortSerializedProperty;
    private SerializedProperty _outputPortSerializedProperty;
    private SerializedProperty _backgroundPositionSerializedProperty;
    private SerializedProperty _colorSerializedProperty;
    private SerializedProperty _changeColorDurationSerializedProperty;
    private SerializedProperty _mode3EnableSerializedProperty;
    private SerializedProperty _changeMode2DurationSerializedProperty;
    private SerializedProperty _backgroundPositionMode2SerializedProperty;
    private SerializedProperty _awaitedSmoothChangeBackgroundSerializedProperty;
    private SerializedProperty _awaitedSmoothBackgroundChangePositionSerializedProperty;
    private SerializedProperty _awaitedSetColorOverlayBackgroundSerializedProperty;
    private MyProject.EnumPopupDrawer _enumPopupDrawer;
    private LineDrawer _lineDrawer;
    private int _currentIndex;
    private int _index;
    private bool _discriptionKey;
    private string _discriptionText1 = $"Mode1: Изменение фона с положением";
    private string _discriptionText2 = $"Mode2: Плавное перетекание фона в другой";
    private string _discriptionText3 = $"Mode3: Плавное изменение цвета фона";
    public override void OnBodyGUI()
    {
        if (_backgroundNode == null)
        {
            _backgroundNode = target as BackgroundNode;
            _backgroundNodeModeSerializedProperty = serializedObject.FindProperty("_backgroundNodeMode");
            _isSmoothCurtainSerializedProperty = serializedObject.FindProperty("_isSmoothCurtain");
            _inputPortSerializedProperty = serializedObject.FindProperty("Input");
            _outputPortSerializedProperty = serializedObject.FindProperty("Output");
            _backgroundPositionSerializedProperty = serializedObject.FindProperty("_backgroundPosition");
            _colorSerializedProperty = serializedObject.FindProperty("_color");
            _changeColorDurationSerializedProperty = serializedObject.FindProperty("_changeColorDuration");
            _mode3EnableSerializedProperty = serializedObject.FindProperty("_mode3Enable");
            _changeMode2DurationSerializedProperty = serializedObject.FindProperty("_changeMode2Duration");
            _backgroundPositionMode2SerializedProperty = serializedObject.FindProperty("_backgroundPositionMode2");
            _keySerializedProperty = serializedObject.FindProperty("_key");
            _keyToSerializedProperty = serializedObject.FindProperty("_keyTo");
            _awaitedSmoothChangeBackgroundSerializedProperty = serializedObject.FindProperty("_awaitedSmoothChangeBackground");
            _awaitedSmoothBackgroundChangePositionSerializedProperty = serializedObject.FindProperty("_awaitedSmoothBackgroundChangePosition");
            _awaitedSetColorOverlayBackgroundSerializedProperty = serializedObject.FindProperty("_awaitedSetColorOverlayBackground");
            _enumPopupDrawer = new EnumPopupDrawer();
            _lineDrawer = new LineDrawer();
        }
        else
        {
            serializedObject.Update();
        
            NodeEditorGUILayout.PropertyField(_inputPortSerializedProperty);
            NodeEditorGUILayout.PropertyField(_outputPortSerializedProperty);

            _enumPopupDrawer.DrawEnumPopup<BackgroundNodeMode>(_backgroundNodeModeSerializedProperty, "Current Mode: ");

            _discriptionKey = EditorGUILayout.Foldout(_discriptionKey, "Discription:");
            if (_discriptionKey)
            {
                EditorGUILayout.LabelField(_discriptionText1);
                EditorGUILayout.LabelField(_discriptionText2);
                EditorGUILayout.LabelField(_discriptionText3);
            }
            // EditorGUILayout
            _lineDrawer.DrawHorizontalLine(Color.green);
            switch (_backgroundNodeModeSerializedProperty.enumValueIndex)
            {
                case (int)BackgroundNodeMode.Mode1:
                    DrawMode1();
                    break;
                case (int)BackgroundNodeMode.Mode2:
                    DrawMode2();
                    break;
                case (int)BackgroundNodeMode.Mode3:
                    DrawMode3();
                    break;
            }
        }
    }

    private void DrawMode1()
    {
        EditorGUILayout.LabelField("ChangeBackground");
        if (_backgroundNode.BackgroundsDictionary != null && _backgroundNode.BackgroundsDictionary.Count > 0)
        {
            EditorGUI.BeginChangeCheck();
            DrawPopup(_keySerializedProperty, "Current: ");
            _enumPopupDrawer.DrawEnumPopup<BackgroundPosition>(_backgroundPositionSerializedProperty, "Current Pos: ");
            _awaitedSmoothBackgroundChangePositionSerializedProperty.boolValue =
                EditorGUILayout.Toggle("Awaited: ", _awaitedSmoothBackgroundChangePositionSerializedProperty.boolValue);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("IsSmoothCurtain: ");
            _isSmoothCurtainSerializedProperty.boolValue =
                EditorGUILayout.Toggle(_isSmoothCurtainSerializedProperty.boolValue);
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                SetBackground();
            }
        }
    }

    private void DrawMode2()
    {
        EditorGUILayout.LabelField("SmoothSwitchBackgrounds");
        EditorGUI.BeginChangeCheck();
        _awaitedSmoothChangeBackgroundSerializedProperty.boolValue =
            EditorGUILayout.Toggle("Awaited: ", _awaitedSmoothChangeBackgroundSerializedProperty.boolValue);
        _changeMode2DurationSerializedProperty.floatValue = EditorGUILayout.FloatField("Duration: ", _changeMode2DurationSerializedProperty.floatValue);
        DrawPopup(_keyToSerializedProperty, "To: ");
        _enumPopupDrawer.DrawEnumPopup<BackgroundPosition>(_backgroundPositionMode2SerializedProperty, "To End Pos: ");
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            SetBackground();
        }
    }
    private void DrawMode3()
    {
        EditorGUILayout.LabelField("SmoothChangeOverlayColor");
        EditorGUI.BeginChangeCheck();
        _awaitedSetColorOverlayBackgroundSerializedProperty.boolValue = EditorGUILayout.Toggle("Awaited: ", _awaitedSetColorOverlayBackgroundSerializedProperty.boolValue);
        _mode3EnableSerializedProperty.boolValue = EditorGUILayout.Toggle("Enabled:  ", _mode3EnableSerializedProperty.boolValue);
        _changeColorDurationSerializedProperty.floatValue = EditorGUILayout.FloatField("Duration: ", _changeColorDurationSerializedProperty.floatValue);
        _colorSerializedProperty.colorValue = EditorGUILayout.ColorField("Target color: ", _colorSerializedProperty.colorValue);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            SetBackground();
        }
    }
    private void DrawPopup(SerializedProperty serializedPropertyKey, string label)
    {
        if (_backgroundNode.BackgroundsDictionary != null)
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
            foreach (var pair in _backgroundNode.BackgroundsDictionary)
            {
                if (pair.Value.name == serializedPropertyKey.stringValue)
                {
                    _currentIndex = _index;
                }
                _namesToPopup.Add(pair.Value.name);
                _index++;
            }

            EditorGUI.BeginChangeCheck();
            _currentIndex = EditorGUILayout.Popup(label, _currentIndex,  _namesToPopup.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                if (_namesToPopup.Count > _currentIndex)
                {
                    serializedPropertyKey.stringValue = _namesToPopup[_currentIndex];
                    serializedObject.ApplyModifiedProperties();
                    Debug.Log($"_currentIndex {_currentIndex} {serializedPropertyKey.stringValue}");
                }
            }
        }
    }

    private void SetBackground()
    {
        if (_privateMethodOnSetBackground == null)
        {
            _privateMethodOnSetBackground =  _backgroundNode.GetType().GetMethod("SetInfoToView", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        _privateMethodOnSetBackground?.Invoke(_backgroundNode, null);
    }
}