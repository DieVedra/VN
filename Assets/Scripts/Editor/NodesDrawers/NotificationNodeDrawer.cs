using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditorAttribute(typeof(NotificationNode))]
public class NotificationNodeDrawer : NodeEditor
{
    private readonly int _maxCountSymbols = 120;

    private NotificationNode _notificationNode;
    private MethodInfo _privateMethod;
    private TextNodeDrawer _textNodeDrawer;
    private SerializedProperty _colorSerializedProperty;
    private SerializedProperty _showTimeSerializedProperty;
    private SerializedProperty _delayDisplayTimeSerializedProperty;
    private SerializedProperty _overrideDataKeySerializedProperty;
    private SerializedProperty _notificationNodeDataSerializedProperty;
    private SerializedProperty _awaitKeySerializedProperty;
    private SerializedProperty _inputPortSerializedProperty;
    private SerializedProperty _outputPortSerializedProperty;
    public override void OnBodyGUI()
    {
        _notificationNode = target as NotificationNode;
        if (_notificationNode != null)
        {
            if (_textNodeDrawer == null)
            {
                _notificationNode = target as NotificationNode;
                _overrideDataKeySerializedProperty = serializedObject.FindProperty("_overrideDataKey");
                _notificationNodeDataSerializedProperty = serializedObject.FindProperty("_notificationNodeData");
                _awaitKeySerializedProperty = serializedObject.FindProperty("_awaitKey");
                _colorSerializedProperty = _notificationNodeDataSerializedProperty.FindPropertyRelative("_color");
                _showTimeSerializedProperty = _notificationNodeDataSerializedProperty.FindPropertyRelative("_showTime");
                _delayDisplayTimeSerializedProperty = _notificationNodeDataSerializedProperty.FindPropertyRelative("_delayDisplayTime");
                
                _inputPortSerializedProperty = serializedObject.FindProperty("Input");
                _outputPortSerializedProperty = serializedObject.FindProperty("Output");

                _privateMethod = _notificationNode.GetType().GetMethod("SetInfoToView", BindingFlags.NonPublic | BindingFlags.Instance);
                _textNodeDrawer = new TextNodeDrawer(
                    serializedObject.FindProperty("_localizationText"),
                    serializedObject,
                    ()=> { _privateMethod.Invoke(_notificationNode, null); },
                    "Notification text: ",
                    _maxCountSymbols);
            }

            NodeEditorGUILayout.PropertyField(_inputPortSerializedProperty);
            NodeEditorGUILayout.PropertyField(_outputPortSerializedProperty);
            _textNodeDrawer.DrawGUI();
            _overrideDataKeySerializedProperty.boolValue =
                EditorGUILayout.Toggle("Override Data: ", _overrideDataKeySerializedProperty.boolValue);
            _awaitKeySerializedProperty.boolValue = EditorGUILayout.Toggle("Await end: ", _awaitKeySerializedProperty.boolValue);
            if (_overrideDataKeySerializedProperty.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Color Text: ", GUILayout.Width(100f));
                _colorSerializedProperty.colorValue = EditorGUILayout.ColorField(_colorSerializedProperty.colorValue, GUILayout.Width(80f));
                EditorGUILayout.EndHorizontal();
                DrawFloatField(_showTimeSerializedProperty, "Show Time: ");
                DrawFloatField(_delayDisplayTimeSerializedProperty, "Delay Display Time: ");
            }
        }
    }

    private void DrawFloatField(SerializedProperty serializedProperty, string name)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name, GUILayout.Width(100f));
        serializedProperty.floatValue = EditorGUILayout.FloatField(serializedProperty.floatValue);
        EditorGUILayout.EndHorizontal();
    }
}