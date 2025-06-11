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
    public override void OnBodyGUI()
    {
        if (_textNodeDrawer == null)
        {
            _notificationNode = target as NotificationNode;
            _colorSerializedProperty = serializedObject.FindProperty("_color");
            _showTimeSerializedProperty = serializedObject.FindProperty("_showTime");
            _delayDisplayTimeSerializedProperty = serializedObject.FindProperty("_delayDisplayTime");
            _privateMethod = _notificationNode.GetType().GetMethod("SetInfoToView", BindingFlags.NonPublic | BindingFlags.Instance);
            _textNodeDrawer = new TextNodeDrawer(
                serializedObject.FindProperty("_localizationText"),
                serializedObject,
                ()=> { _privateMethod.Invoke(_notificationNode, null); },
                "Notification text: ",
                _maxCountSymbols);
        }
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));

        _textNodeDrawer.DrawGUI();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Color Text: ", GUILayout.Width(100f));
        _colorSerializedProperty.colorValue = EditorGUILayout.ColorField(_colorSerializedProperty.colorValue, GUILayout.Width(80f));
        EditorGUILayout.EndHorizontal();

        DrawFloatField(_showTimeSerializedProperty, "Show Time: ");
        DrawFloatField(_delayDisplayTimeSerializedProperty, "Delay Display Time: ");
    }

    private void DrawFloatField(SerializedProperty serializedProperty, string name)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name, GUILayout.Width(100f));
        serializedProperty.floatValue = EditorGUILayout.FloatField(serializedProperty.floatValue);
        EditorGUILayout.EndHorizontal();
    }
}