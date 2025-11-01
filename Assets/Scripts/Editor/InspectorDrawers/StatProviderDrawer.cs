

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SeriaStatProvider))]
public class StatProviderDrawer : Editor
{
    private string _nameField;
    private Color _colorField;
    private LineDrawer _lineDrawer;
    private SeriaStatProvider _seriaStatProvider;
    private SerializedProperty _listStatsProperty;
    private SerializedProperty _serializedProperty;
    private SerializedProperty _seriaNumberSerializedProperty;
    private MethodInfo _addNewStatMethod;
    private MethodInfo _removeStat;
    private GUIStyle _guiStyle;
    private int _statIndex;
    private int _addValue;
    private string[] _names;

    private void OnEnable()
    {
        _seriaStatProvider = target as SeriaStatProvider;
        _lineDrawer = new LineDrawer();
        _listStatsProperty = serializedObject.FindProperty($"_stats");
        _seriaNumberSerializedProperty = serializedObject.FindProperty($"_seriaNumber");
        _addNewStatMethod = _seriaStatProvider.GetType().GetMethod("AddNewStat", BindingFlags.NonPublic | BindingFlags.Instance);
        _removeStat = _seriaStatProvider.GetType().GetMethod("RemoveStat", BindingFlags.NonPublic | BindingFlags.Instance);
        SetStatNameIndex();
        _colorField = Color.white;

        TryInitStatNames();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _seriaNumberSerializedProperty.intValue = EditorGUILayout.IntField("To Seria Number:", _seriaNumberSerializedProperty.intValue);
        _lineDrawer.DrawHorizontalLine(Color.red);

        if (_listStatsProperty != null)
        {
            for (int i = 0; i < _listStatsProperty.arraySize; i++)
            {
                _serializedProperty = _listStatsProperty.GetArrayElementAtIndex(i);
                DrawFieldColor(
                    _seriaStatProvider.StatsLocalizationStrings[i].LocalizationName,
                    _serializedProperty.FindPropertyRelative("_value").intValue,
                    _serializedProperty.FindPropertyRelative("_showInEndGameResultKey"),
                    _serializedProperty.FindPropertyRelative("_colorField").colorValue);
            }
        }
        EditorGUILayout.Space(30f);
        DrawAdd();
        if (_listStatsProperty.arraySize > 0)
        {
            DrawRemove();
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawFieldColor(string fieldName, int value, SerializedProperty showInEndGameResultKeySerializedProperty, Color color)
    {
        EditorGUILayout.Space(20f);
        EditorGUILayout.BeginHorizontal();
        Color oldColor = GUI.color;
        _guiStyle = new GUIStyle(GUI.skin.label);
        _guiStyle.normal.textColor = color;
        DrawField(fieldName, _guiStyle, 200f, 2);
        _guiStyle.normal.textColor = oldColor;
        DrawField(value.ToString(), _guiStyle, 30f, 2);
        
        DrawField("ShowInGameResult:", _guiStyle, 120f, -1);
        showInEndGameResultKeySerializedProperty.boolValue =
            EditorGUILayout.Toggle(showInEndGameResultKeySerializedProperty.boolValue);

        EditorGUILayout.EndHorizontal();
    }

    private void DrawField(string fieldName, GUIStyle style, float width = 0f, int addFontSize = 0, FontStyle fontStyle = FontStyle.Normal)
    {
        int fontSize = style.fontSize;
        style.fontSize = style.fontSize + addFontSize;
        style.fontStyle = fontStyle;
        EditorGUILayout.LabelField(fieldName, style, GUILayout.Width(width));
        style.fontSize = fontSize;
    }

    private void DrawAdd()
    {
        _guiStyle = new GUIStyle(GUI.skin.label);
        _lineDrawer.DrawHorizontalLine(Color.red);
        DrawField("Add Stat: ", _guiStyle, 80f, 1, FontStyle.Bold);
        EditorGUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("NameText: ", GUILayout.Width(40f));
        _nameField = EditorGUILayout.TextField(_nameField, GUILayout.Width(150f));
        EditorGUILayout.LabelField("Color: ", GUILayout.Width(40f));
        _colorField = EditorGUILayout.ColorField(_colorField, GUILayout.Width(40f));
        if (GUILayout.Button("Add", GUILayout.Width(40f)))
        {
            _addNewStatMethod.Invoke(_seriaStatProvider, new object[]{new Stat(_nameField, _addValue,  _colorField)});
            TryInitStatNames();
            _colorField = Color.white;
            _nameField = string.Empty;
            SetStatNameIndex();
            serializedObject.Update();
        }
        EditorGUILayout.EndHorizontal();
        _addValue = EditorGUILayout.IntField("Value:", _addValue);

    }

    private void DrawRemove()
    {
        EditorGUILayout.Space(20f);
        _guiStyle = new GUIStyle(GUI.skin.label);
        DrawField("Remove Stat: ", _guiStyle, 100f, 1, FontStyle.Bold);
        EditorGUILayout.BeginHorizontal();
        _statIndex = EditorGUILayout.Popup(_statIndex, _names, GUILayout.Width(150f));
        if (GUILayout.Button("Remove", GUILayout.Width(60f)))
        {
            _removeStat.Invoke(_seriaStatProvider, new object[]{_statIndex});
            TryInitStatNames();
            SetStatNameIndex();
            serializedObject.Update();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void SetStatNameIndex()
    {
        if (_listStatsProperty.arraySize > 0)
        {
            _statIndex = _listStatsProperty.arraySize - 1;
        }
        else
        {
            _statIndex = 0;
        }
    }
    private void TryInitStatNames()
    {
        serializedObject.Update();
        List<string> names = new List<string>(_listStatsProperty.arraySize);
        for (int i = 0; i < _listStatsProperty.arraySize; i++)
        {
            _serializedProperty = _listStatsProperty.GetArrayElementAtIndex(i);
            names.Add(_seriaStatProvider.StatsLocalizationStrings[i].LocalizationName);
        }
        _names = names.ToArray();
    }
}