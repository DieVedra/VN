
using UnityEditor;
using XNodeEditor;

[CustomNodeEditor(typeof(SwitchToNextSeriaNode))]
public class SwitchToNextSeriaNodeDrawer : NodeEditor
{
    private SwitchToNextSeriaNode _switchToNextSeriaNode;
    private SerializedProperty _serializedPropertyPutOnSwimsuitKey;
    private SerializedProperty _serializedPropertyInput;
    public override void OnBodyGUI()
    {
        _switchToNextSeriaNode = target as SwitchToNextSeriaNode;
        if (_switchToNextSeriaNode != null)
        {
            if (_serializedPropertyPutOnSwimsuitKey == null)
            {
                _serializedPropertyPutOnSwimsuitKey = serializedObject.FindProperty("_putOnSwimsuit");
                _serializedPropertyInput = serializedObject.FindProperty("Input");
            }
            NodeEditorGUILayout.PropertyField(_serializedPropertyInput);
            _serializedPropertyPutOnSwimsuitKey.boolValue = EditorGUILayout.Toggle("PutOnSwimsuit: ", _serializedPropertyPutOnSwimsuitKey.boolValue);
        }
    }
}