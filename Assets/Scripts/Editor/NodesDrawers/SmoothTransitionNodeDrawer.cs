using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(SmoothTransitionNode))]
public class SmoothTransitionNodeDrawer : NodeEditor
{
    private const string _labelStartCurtain = "Is Start : ";
    private const string _labelCustomDelay = "Custom Delay: ";
    private const string _labelEndCurtain = "Is End : ";
    private const string _labelStartImmediatlyCurtain = "Is Start Immediatly: ";
    private const string _labelEndImmediatlyCurtain = "Is End Immediatly: ";
    private const string _trueKey = "true";
    
    private SmoothTransitionNode _smoothTransitionNode;
    private SerializedProperty _isStartCurtainSerializedProperty;
    private SerializedProperty _isEndCurtainSerializedProperty;
    private SerializedProperty _customDelaySerializedProperty;
    private SerializedProperty _delaySerializedProperty;
    private SerializedProperty _inputSerializedProperty;
    private SerializedProperty _outputSerializedProperty;
    
    private SerializedProperty _isStartImmediatlyCurtainSerializedProperty;
    private SerializedProperty _isEndImmediatlyCurtainSerializedProperty;
    
    private LineDrawer _lineDrawer;
    public override void OnBodyGUI()
    {
        if (_smoothTransitionNode == null)
        {
            _smoothTransitionNode = target as SmoothTransitionNode;
            _lineDrawer = new LineDrawer();
            _customDelaySerializedProperty = serializedObject.FindProperty("_customDelay");
            _delaySerializedProperty = serializedObject.FindProperty("_delay");
            _isStartCurtainSerializedProperty = serializedObject.FindProperty("_isStartCurtain");
            _isEndCurtainSerializedProperty = serializedObject.FindProperty("_isEndCurtain");
            
            _isStartImmediatlyCurtainSerializedProperty = serializedObject.FindProperty("_isStartImmediatlyCurtain");
            _isEndImmediatlyCurtainSerializedProperty = serializedObject.FindProperty("_isEndImmediatlyCurtain");
            _inputSerializedProperty = serializedObject.FindProperty("Input");
            _outputSerializedProperty = serializedObject.FindProperty("Output");
        }
        serializedObject.Update();

        NodeEditorGUILayout.PropertyField(_inputSerializedProperty);
        NodeEditorGUILayout.PropertyField(_outputSerializedProperty);
        
        DrawToggle(_customDelaySerializedProperty, _labelCustomDelay);
        if (_customDelaySerializedProperty.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Time: ");
            _delaySerializedProperty.floatValue = EditorGUILayout.FloatField(_delaySerializedProperty.floatValue);
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUI.BeginChangeCheck();
        _lineDrawer.DrawHorizontalLine(Color.green);

        EditorGUILayout.Space(20f);

        if (_isStartCurtainSerializedProperty.boolValue == false &&
            _isEndCurtainSerializedProperty.boolValue == false &&
            _isStartImmediatlyCurtainSerializedProperty.boolValue == false &&
            _isEndImmediatlyCurtainSerializedProperty.boolValue == false)
        {
            DrawToggle(_isStartCurtainSerializedProperty, _labelStartCurtain);
            DrawToggle(_isEndCurtainSerializedProperty, _labelEndCurtain);
            DrawToggle(_isStartImmediatlyCurtainSerializedProperty, _labelStartImmediatlyCurtain);
            DrawToggle(_isEndImmediatlyCurtainSerializedProperty, _labelEndImmediatlyCurtain);
        }
        else
        {
            if (_isStartCurtainSerializedProperty.boolValue == true)
            {
                EditorGUILayout.LabelField($"{_labelStartCurtain} {_trueKey}");
            }
            if (_isEndCurtainSerializedProperty.boolValue == true)
            {
                EditorGUILayout.LabelField($"{_labelEndCurtain} {_trueKey}");
            }
            if (_isStartImmediatlyCurtainSerializedProperty.boolValue == true)
            {
                EditorGUILayout.LabelField($"{_labelStartImmediatlyCurtain} {_trueKey}");
            }
            if (_isEndImmediatlyCurtainSerializedProperty.boolValue == true)
            {
                EditorGUILayout.LabelField($"{_labelEndImmediatlyCurtain} {_trueKey}");
            }

            DrawSkip();
        }
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawToggle(SerializedProperty serializedProperty1, string label)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label);
        serializedProperty1.boolValue = EditorGUILayout.Toggle(serializedProperty1.boolValue);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSkip()
    {
        EditorGUILayout.Space(30f);

        if (GUILayout.Button("Skip"))
        {
            _isStartCurtainSerializedProperty.boolValue = false;
            _isEndCurtainSerializedProperty.boolValue = false;
            _isStartImmediatlyCurtainSerializedProperty.boolValue = false;
            _isEndImmediatlyCurtainSerializedProperty.boolValue = false;
        }
    }
}