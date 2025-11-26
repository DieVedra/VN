using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

public class SwitchNodeEditorLogic
{
    private const string _index = "Index: ";
    private readonly LineDrawer _lineDrawer = new LineDrawer();
    private readonly StringBuilder _stringBuilder = new StringBuilder();
    private readonly SerializedProperty _removeAdditionalCaseIndexSerializedProperty;
    private readonly SwitchNodeLogic _switchNodeLogic;
    private readonly SerializedProperty _casesForStatsListProperty;
    private readonly SerializedObject _serializedObject;
    private readonly Func<string, NodePort> _getOutputPort;
    private bool _updateAdditionalCases;
    private string[] _statsNames;

    private SerializedProperty _caseForStats;
    private SerializedProperty _additionalCaseStats;
    private SerializedProperty _foldOutKeySerializedProperty;
    private SerializedProperty _additionalCaseStatsArraySerializedProperty;
    private SerializedProperty _additionalCaseSerializedProperty;
    private SerializedProperty _indexStatSerializedProperty;
    private SerializedProperty _keySerializedProperty;

    public SwitchNodeEditorLogic(SerializedProperty removeAdditionalCaseIndexSerializedProperty,
        SerializedProperty casesForStatsListProperty, SwitchNodeLogic switchNodeLogic,
        SerializedObject serializedObject, Func<string, NodePort> getOutputPort)
    {
        _removeAdditionalCaseIndexSerializedProperty = removeAdditionalCaseIndexSerializedProperty;
        _switchNodeLogic = switchNodeLogic;
        _casesForStatsListProperty = casesForStatsListProperty;
        _serializedObject = serializedObject;
        _getOutputPort = getOutputPort;
    }

    public void DrawCase(CaseForStats statsLocalizations, SerializedProperty serializedPropertyMyCase, string portName, string portViewName)
    {
        _foldOutKeySerializedProperty = serializedPropertyMyCase.FindPropertyRelative("_foldoutKey");
        _foldOutKeySerializedProperty.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(_foldOutKeySerializedProperty.boolValue, $"Settings {portViewName}");
        if (_foldOutKeySerializedProperty.boolValue)
        {
            DrawStats(statsLocalizations.StatsLocalizations, serializedPropertyMyCase.FindPropertyRelative("_caseStats"));
            _additionalCaseStatsArraySerializedProperty = serializedPropertyMyCase.FindPropertyRelative("_additionalCaseStats");
            DrawAdditionalSettings();
            AddAdditionalSettings();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        NodeEditorGUILayout.PortField(new GUIContent(portViewName), _getOutputPort.Invoke(portName));
        EditorGUILayout.Space(10f);
    }
    private void DrawStats(IReadOnlyList<ILocalizationString> caseStats, SerializedProperty statsSerializedPropertyMyCase)
    {
        SerializedProperty serializedPropertyToggle;
        SerializedProperty serializedProperty;
        for (int i = 0; i < statsSerializedPropertyMyCase.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            serializedProperty = statsSerializedPropertyMyCase.GetArrayElementAtIndex(i);
            serializedPropertyToggle = serializedProperty.FindPropertyRelative("_includeKey");
            serializedPropertyToggle.boolValue = EditorGUILayout.Toggle(serializedPropertyToggle.boolValue, GUILayout.Width(20f));
            DrawLabel(caseStats[i].LocalizationName.DefaultText);
            if (serializedPropertyToggle.boolValue)
            {
                DrawPopup(_switchNodeLogic.Operators ,serializedProperty.FindPropertyRelative("_indexCurrentOperator"), 40f);
                DrawInt(serializedProperty.FindPropertyRelative("_value"));
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    public void TryInitStatsNames()
    {
        if (_switchNodeLogic?.GameStatsCopied != null)
        {
            int count = _switchNodeLogic.GameStatsCopied.Count;
            _statsNames = new string[count];
            for (int i = 0; i < count; i++)
            {
                _statsNames[i] = _switchNodeLogic.GameStatsCopied[i].NameText;
            }
        }
    }
    public void TryUpdateAdditionalCases()
    {
        if (_updateAdditionalCases == false && _switchNodeLogic != null)
        {
            _updateAdditionalCases = true;
            for (int i = 0; i < _casesForStatsListProperty.arraySize; i++)
            {
                _caseForStats = _casesForStatsListProperty.GetArrayElementAtIndex(i);
                _additionalCaseStats = _caseForStats.FindPropertyRelative("_additionalCaseStats");
                for (int j = 0; j < _additionalCaseStats.arraySize; j++)
                {
                    _additionalCaseSerializedProperty = _additionalCaseStats.GetArrayElementAtIndex(j);
                    _indexStatSerializedProperty = _additionalCaseSerializedProperty.FindPropertyRelative("_indexStat1");
                    _keySerializedProperty = _additionalCaseSerializedProperty.FindPropertyRelative("_stat1Key");
                    Update();
                    _indexStatSerializedProperty = _additionalCaseSerializedProperty.FindPropertyRelative("_indexStat2");
                    _keySerializedProperty = _additionalCaseSerializedProperty.FindPropertyRelative("_stat2Key");
                    Update();
                }
            }
        }

        void Update()
        {
            if (_keySerializedProperty.stringValue != _switchNodeLogic
                .GameStatsCopied[_indexStatSerializedProperty.intValue].NameKey)
            {
                for (int i = 0; i < _switchNodeLogic.GameStatsCopied.Count; i++)
                {
                    if (_switchNodeLogic.GameStatsCopied[i].NameKey == _keySerializedProperty.stringValue)
                    {
                        _indexStatSerializedProperty.intValue = i;
                        break;
                    }
                }
            }
        }
    }
    private void DrawLabel(string name)
    {
        EditorGUILayout.LabelField(name,GUILayout.Width(150f));
    }

    private void DrawPopup(string[] strings, SerializedProperty indexSerializedProperty, float width)
    {
        if (strings != null && indexSerializedProperty != null)
        {
            indexSerializedProperty.intValue = EditorGUILayout.Popup(indexSerializedProperty.intValue, strings, GUILayout.Width(width));
        }
    }

    private void DrawInt(SerializedProperty serializedProperty)
    {
        serializedProperty.intValue = EditorGUILayout.IntField(serializedProperty.intValue, GUILayout.Width(30f));
    }

    private void DrawAdditionalSettings()
    {
        for (int i = 0; i < _additionalCaseStatsArraySerializedProperty.arraySize; i++)
        {
            _additionalCaseSerializedProperty = _additionalCaseStatsArraySerializedProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            _stringBuilder.Append(_index);
            _stringBuilder.Append(i.ToString());
            EditorGUILayout.LabelField(_stringBuilder.ToString() ,GUILayout.Width(50f));
            if (_statsNames != null)
            {
                _indexStatSerializedProperty = _additionalCaseSerializedProperty.FindPropertyRelative("_indexStat1");
                _keySerializedProperty = _additionalCaseSerializedProperty.FindPropertyRelative("_stat1Key");
                DrawPopupStat();
                DrawPopup(_switchNodeLogic.Operators, _additionalCaseSerializedProperty.FindPropertyRelative("_indexCurrentOperator"), 40f);
                _indexStatSerializedProperty = _additionalCaseSerializedProperty.FindPropertyRelative("_indexStat2");
                _keySerializedProperty = _additionalCaseSerializedProperty.FindPropertyRelative("_stat2Key");
                DrawPopupStat();
            }

            EditorGUILayout.EndHorizontal();
            _stringBuilder.Clear();
        }

        void DrawPopupStat()
        {
            DrawPopup(_statsNames, _indexStatSerializedProperty, 140f);
            _keySerializedProperty.stringValue = _switchNodeLogic.GameStatsCopied[_indexStatSerializedProperty.intValue].NameKey;
        }
    }

    private void AddAdditionalSettings()
    {
        _lineDrawer.DrawHorizontalLine(Color.green);
        EditorGUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Additional Case"))
        {
            _serializedObject.Update();
            int count = _additionalCaseStatsArraySerializedProperty.arraySize;
            _additionalCaseStatsArraySerializedProperty.InsertArrayElementAtIndex(count);
            _serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("Remove Additional Case By Index: ") && _additionalCaseStatsArraySerializedProperty.arraySize > 0)
        {
            if (_removeAdditionalCaseIndexSerializedProperty.intValue < _additionalCaseStatsArraySerializedProperty.arraySize)
            {
                _serializedObject.Update();
                _additionalCaseStatsArraySerializedProperty.DeleteArrayElementAtIndex(_removeAdditionalCaseIndexSerializedProperty.intValue);
                _serializedObject.ApplyModifiedProperties();
            }
        }

        DrawInt(_removeAdditionalCaseIndexSerializedProperty);
        EditorGUILayout.EndHorizontal();
    }
}