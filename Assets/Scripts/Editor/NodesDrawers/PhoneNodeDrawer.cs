using System.Collections.Generic;
using UnityEditor;
using XNodeEditor;

[CustomEditor(typeof(PhoneNode))]
public class PhoneNodeDrawer : NodeEditor
{
    private PhoneNode _phoneNode;

    private SerializedProperty _inputSerializedProperty;
    private SerializedProperty _outputSerializedProperty;
    private SerializedProperty _phoneIndexSerializedProperty;
    private SerializedProperty _phoneScreenIndexSerializedProperty;
    private SerializedProperty _butteryPercentSerializedProperty;
    
    
    private const string _blockScreenName = "BlockScreen";
    private const string _contactsScreenName = "ContactsScreen";
    private const string _dialogScreenName = "DialogScreen";
    private string[] _backgroundsNames;
    private string[] _phonesNames;
    public override void OnBodyGUI()
    {
        if (_phoneNode == null)
        {
            _phoneNode = target as PhoneNode;
        }

        if (_phoneIndexSerializedProperty == null)
        {
            _backgroundsNames = new[] {_blockScreenName, _contactsScreenName, _dialogScreenName};
            _phoneIndexSerializedProperty = serializedObject.FindProperty("_phoneIndex");
            _phoneScreenIndexSerializedProperty = serializedObject.FindProperty("_phoneScreenIndex");
            _butteryPercentSerializedProperty = serializedObject.FindProperty("_butteryPercent");
            _inputSerializedProperty = serializedObject.FindProperty("Input");
            _outputSerializedProperty = serializedObject.FindProperty("Output");
        }
        else
        {
            NodeEditorGUILayout.PropertyField(_inputSerializedProperty);
            NodeEditorGUILayout.PropertyField(_outputSerializedProperty);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current phone:");
            _phonesNames = InitNamesPhones();
            _phoneIndexSerializedProperty.intValue = EditorGUILayout.Popup(_phoneIndexSerializedProperty.intValue, _phonesNames);
            EditorGUILayout.EndHorizontal();
            
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current phone screen: ");
            _phoneScreenIndexSerializedProperty.intValue = EditorGUILayout.Popup(_phoneScreenIndexSerializedProperty.intValue, _backgroundsNames);
            EditorGUILayout.EndHorizontal();

            
        }
        
        
    }

    private string[] InitNamesPhones()
    {
        List<string> names = new List<string>();
        for (int i = 0; i < _phoneNode.DataProviders.Count; i++)
        {
            names.Add(_phoneNode.DataProviders[i].NamePhone);
        }
        return names.ToArray();
    }
}