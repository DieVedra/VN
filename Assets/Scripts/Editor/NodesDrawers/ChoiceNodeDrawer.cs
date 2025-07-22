using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(ChoiceNode))]
public class ChoiceNodeDrawer : NodeEditor
{
    private const int _maxPortCount = 3;
    private const int _maxCountSymbols = 500;
    private ChoiceNode _choiceNode;
    private LineDrawer _lineDrawer;
    private SerializedProperty _choiceText1Property;
    private SerializedProperty _choiceText2Property;
    private SerializedProperty _choiceText3Property;
    private SerializedProperty _showChoice3Property;
    private SerializedProperty _showOutputProperty;
    private SerializedProperty _timerPortIndexProperty;
    
    private SerializedProperty _addTimerProperty;
    private SerializedProperty _timerValueProperty;
    
    private SerializedProperty _showStatsChoice1KeyProperty;
    private SerializedProperty _showStatsChoice2KeyProperty;
    private SerializedProperty _showStatsChoice3KeyProperty;

    private LocalizationString _localizationStringText1;
    private LocalizationString _localizationStringText2;
    private LocalizationString _localizationStringText3;
    private SerializedProperty _choice1PriceProperty;
    private SerializedProperty _choice2PriceProperty;
    private SerializedProperty _choice3PriceProperty;
    private SerializedProperty _choice1AdditionaryPriceProperty;
    private SerializedProperty _choice2AdditionaryPriceProperty;
    private SerializedProperty _choice3AdditionaryPriceProperty;
    private LocalizationStringTextDrawer _localizationStringTextDrawer;
    private MethodInfo _privateMethod;
    private string[] _timerPortIndexes;

    private bool NamesNotEmpty()
    {
        bool result = false;
        if (_choiceNode.NamesPorts != null)
        {
            if (_choiceNode.NamesPorts.Count > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
        }
        else
        {
            result = false;
        }

        return result;
    }

    public override void OnBodyGUI()
    {
        if (_choiceNode == null)
        {
            _choiceNode = target as ChoiceNode;
        }

        if (_choiceText1Property == null)
        {
            _choiceText1Property = serializedObject.FindProperty("_localizationChoiceText1");
            _choiceText2Property = serializedObject.FindProperty("_localizationChoiceText2");
            _choiceText3Property = serializedObject.FindProperty("_localizationChoiceText3");
            _showChoice3Property = serializedObject.FindProperty("_showChoice3Key");
            _showOutputProperty = serializedObject.FindProperty("_showOutput");
        
            _addTimerProperty = serializedObject.FindProperty("_addTimer");
            _timerValueProperty = serializedObject.FindProperty("_timerValue");
            _timerPortIndexProperty = serializedObject.FindProperty("_timerPortIndex");

            _showStatsChoice1KeyProperty = serializedObject.FindProperty("_showStatsChoice1Key");
            _showStatsChoice2KeyProperty = serializedObject.FindProperty("_showStatsChoice2Key");
            _showStatsChoice3KeyProperty = serializedObject.FindProperty("_showStatsChoice3Key");


            _choice1PriceProperty = serializedObject.FindProperty("_choice1Price");
            _choice2PriceProperty = serializedObject.FindProperty("_choice2Price");
            _choice3PriceProperty = serializedObject.FindProperty("_choice3Price");
            _choice1AdditionaryPriceProperty = serializedObject.FindProperty("_choice1AdditionaryPrice");
            _choice2AdditionaryPriceProperty = serializedObject.FindProperty("_choice2AdditionaryPrice");
            _choice3AdditionaryPriceProperty = serializedObject.FindProperty("_choice3AdditionaryPrice");
            _lineDrawer = new LineDrawer();
            _localizationStringTextDrawer = new LocalizationStringTextDrawer(new SimpleTextValidator(_maxCountSymbols));
            _localizationStringText1 = _localizationStringTextDrawer.GetLocalizationStringFromProperty(_choiceText1Property);
            _localizationStringText2 = _localizationStringTextDrawer.GetLocalizationStringFromProperty(_choiceText2Property);
            _localizationStringText3 = _localizationStringTextDrawer.GetLocalizationStringFromProperty(_choiceText3Property);
        }

        serializedObject.Update();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));
        
        
        _showOutputProperty.boolValue = EditorGUILayout.Toggle("Show Output: ", _showOutputProperty.boolValue);
        if (_showOutputProperty.boolValue)
        {
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Output"));
        }
        
        EditorGUILayout.BeginHorizontal();

        _addTimerProperty.boolValue = EditorGUILayout.Toggle("Add Timer: ", _addTimerProperty.boolValue, GUILayout.Width(100f));
        if (_addTimerProperty.boolValue)
        {
            _timerValueProperty.intValue= EditorGUILayout.IntField("Timer Value: ", _timerValueProperty.intValue, GUILayout.Width(120f));
        }

        EditorGUILayout.EndHorizontal();
        if (_addTimerProperty.boolValue)
        {
            TryCreatePortIndexes();
            _timerPortIndexProperty.intValue =
                EditorGUILayout.Popup("Port index: ", _timerPortIndexProperty.intValue, _timerPortIndexes);
        }
        
        
        
        EditorGUI.BeginChangeCheck();
        DrawChoiceField(_localizationStringText1, _choiceNode.BaseStatsChoice1Localizations,
            _showStatsChoice1KeyProperty, _choice1PriceProperty, _choice1AdditionaryPriceProperty, "Choice 1", "_baseStatsChoice1", 0);
        DrawChoiceField(_localizationStringText2, _choiceNode.BaseStatsChoice2Localizations,
            _showStatsChoice2KeyProperty, _choice2PriceProperty, _choice2AdditionaryPriceProperty,"Choice 2", "_baseStatsChoice2", 1);
        EditorGUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Show choice3: ");
        _showChoice3Property.boolValue = EditorGUILayout.Toggle(_showChoice3Property.boolValue);
        EditorGUILayout.EndHorizontal();
        if (_showChoice3Property.boolValue)
        {
            DrawChoiceField(_localizationStringText3, _choiceNode.BaseStatsChoice3Localizations,
                _showStatsChoice3KeyProperty, _choice3PriceProperty, _choice3AdditionaryPriceProperty, "Choice 3", "_baseStatsChoice3", 2);
        }
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        
            if (_privateMethod == null)
            {
                _privateMethod = _choiceNode.GetType().GetMethod("SetInfoToView", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            _privateMethod.Invoke(_choiceNode, null);
        }
    }

    private void DrawChoiceField(LocalizationString textProperty, IReadOnlyList<ILocalizationString> baseStatsChoiceLocalizations,
        SerializedProperty showStatsChoiceProperty, SerializedProperty choicePriceProperty, SerializedProperty choiceAdditionaryPriceProperty,
        string label, string nameBaseStatsChoice, int indexNamePort)
    {
        EditorGUILayout.Space(10f);
        _localizationStringTextDrawer.DrawTextField(textProperty, label,false);
        choicePriceProperty.floatValue = EditorGUILayout.FloatField("Choice price: ", choicePriceProperty.floatValue, GUILayout.Width(120f));
        choiceAdditionaryPriceProperty.floatValue = EditorGUILayout.FloatField("Choice additionary price: ", choiceAdditionaryPriceProperty.floatValue, GUILayout.Width(120f));

        showStatsChoiceProperty.boolValue = EditorGUILayout.Toggle("Show stats: ", showStatsChoiceProperty.boolValue);
        if (showStatsChoiceProperty.boolValue == true)
        {
            DrawStats(baseStatsChoiceLocalizations, serializedObject.FindProperty(nameBaseStatsChoice));
        }
        if (NamesNotEmpty())
        {
            DrawPort(_choiceNode.NamesPorts[indexNamePort]);
        }
        _lineDrawer.DrawHorizontalLine(Color.green);
    }
    private void DrawPort(string name)
    {
        if (_choiceNode.NamesPorts.Count > 0 && _showOutputProperty.boolValue == false)
        {
            NodeEditorGUILayout.PortField(new GUIContent(name), target.GetOutputPort(name));
        }
    }

    private void DrawStats(IReadOnlyList<ILocalizationString> baseStatsChoiceLocalizations, SerializedProperty gameStatsFormsSerializedProperty)
    {
        SerializedProperty statFormSerializedProperty;
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(10f);
        for (int i = 0; i < gameStatsFormsSerializedProperty.arraySize; i++)
        {
            statFormSerializedProperty = gameStatsFormsSerializedProperty.GetArrayElementAtIndex(i);
            DrawField(statFormSerializedProperty.FindPropertyRelative("_value"),
                baseStatsChoiceLocalizations[i].LocalizationName.DefaultText);
        }
        EditorGUILayout.EndVertical();
    }
    private void DrawField(SerializedProperty serializedProperty, string nameField)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(nameField, GUILayout.Width(150f));
        serializedProperty.intValue = EditorGUILayout.IntField(serializedProperty.intValue, GUILayout.Width(30f));
        EditorGUILayout.EndHorizontal();
    }

    private void TryCreatePortIndexes()
    {
        if (_timerPortIndexes == null)
        {
            _timerPortIndexes = new[] {"Port 1", "Port 2"};
        }
        else if(_showChoice3Property.boolValue == false && _timerPortIndexes.Length == _maxPortCount)
        {
            _timerPortIndexes = new[] {"Port 1", "Port 2"};
        }
        else if (_showChoice3Property.boolValue && _timerPortIndexes.Length < _maxPortCount)
        {
            _timerPortIndexes = new[] {"Port 1", "Port 2", "Port 3"};
        }
    }
}