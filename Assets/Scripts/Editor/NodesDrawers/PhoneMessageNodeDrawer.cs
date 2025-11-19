using MyProject;
using UnityEditor;
using XNodeEditor;

[CustomNodeEditor(typeof(PhoneMessageNode))]
public class PhoneMessageNodeDrawer : NodeEditor
{
    private MyProject.EnumPopupDrawer _enumPopupDrawer;
    private PhoneMessageNode _phoneMessageNode;
    private SerializedProperty _inputPortProperty;
    private SerializedProperty _outputPortProperty;
    private SerializedProperty _textProperty;
    private SerializedProperty _messageTypeProperty;
    private LocalizationStringTextDrawer _localizationStringTextDrawer;

    public override void OnBodyGUI()
    {
        if (_phoneMessageNode == null)
        {
            _localizationStringTextDrawer = new LocalizationStringTextDrawer();
            _enumPopupDrawer = new EnumPopupDrawer();
            _phoneMessageNode = target as PhoneMessageNode;
            _inputPortProperty = serializedObject.FindProperty("Input");
            _outputPortProperty = serializedObject.FindProperty("Output");
            _textProperty = serializedObject.FindProperty("_localizationString");
            _messageTypeProperty = serializedObject.FindProperty("_type");
        }
        else
        {
            serializedObject.Update();
            NodeEditorGUILayout.PropertyField(_inputPortProperty);
            NodeEditorGUILayout.PropertyField(_outputPortProperty);
            
            _localizationStringTextDrawer.DrawTextField(_localizationStringTextDrawer.GetLocalizationStringFromProperty(_textProperty), "Text: ", true, false);
            _enumPopupDrawer.DrawEnumPopup<PhoneMessageType>(_messageTypeProperty, "Type Message: ");

            serializedObject.ApplyModifiedProperties();
        }
    }
}