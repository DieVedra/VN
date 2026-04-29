using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using XNodeEditor;

[CustomNodeEditor(typeof(PhoneBlockContactNode))]

public class PhoneBlockContactNodeDrawer : NodeEditor
{
    private SerializedProperty _phoneIndexSerializedProperty;
    private SerializedProperty _inputSerializedProperty;
    private SerializedProperty _outputSerializedProperty;
    
    private SerializedProperty _contactKeySerializedProperty;
    private SerializedProperty _contactNameSerializedProperty;
    
    private SerializedProperty _blockContactSerializedProperty;
    private SerializedProperty _unblockContactSerializedProperty;
    
    private PhoneBlockContactNode _phoneBlockContactNode;
    private List<string> _phonesNames;
    private List<LocalizationString> _contactsNames;
    private int _contactNamesIndex = 0;

    private int _phonePreviousIndex;

    public override void OnBodyGUI()
    {
        if (_phoneBlockContactNode == null)
        {
            _phoneBlockContactNode = target as PhoneBlockContactNode;
            _phoneIndexSerializedProperty = serializedObject.FindProperty("_phoneIndex");
            _inputSerializedProperty = serializedObject.FindProperty("Input");
            _outputSerializedProperty = serializedObject.FindProperty("Output");
            
            _contactKeySerializedProperty = serializedObject.FindProperty("_contactKey");
            _contactNameSerializedProperty = serializedObject.FindProperty("_contactName");
            
            _blockContactSerializedProperty = serializedObject.FindProperty("_blockContact");
            _unblockContactSerializedProperty = serializedObject.FindProperty("_unblockContact");
        }
        else
        {
            serializedObject.Update();
            NodeEditorGUILayout.PropertyField(_inputSerializedProperty);
            NodeEditorGUILayout.PropertyField(_outputSerializedProperty);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current phone:");
            InitNamesPhones();
            InitNamesContacts();
            if (_phonesNames != null)
            {
                EditorGUI.BeginChangeCheck();
                _phonePreviousIndex = _phoneIndexSerializedProperty.intValue;
                _phoneIndexSerializedProperty.intValue = EditorGUILayout.Popup(_phoneIndexSerializedProperty.intValue, _phonesNames.ToArray());
                if (EditorGUI.EndChangeCheck() && _phonePreviousIndex != _phoneIndexSerializedProperty.intValue)
                {
                    serializedObject.ApplyModifiedProperties();
                    InitNamesContacts();
                    serializedObject.Update();
                }
            }
            EditorGUILayout.EndHorizontal();

            if (_contactsNames != null)
            {
                if (string.IsNullOrEmpty(_contactKeySerializedProperty.stringValue))
                {
                    _contactKeySerializedProperty.stringValue = _contactsNames[0].Key;
                    serializedObject.ApplyModifiedProperties();

                }

                if (string.IsNullOrEmpty(_contactNameSerializedProperty.stringValue))
                {
                    _contactNameSerializedProperty.stringValue = _contactsNames[0].DefaultText;
                    serializedObject.ApplyModifiedProperties();

                }
                EditorGUI.BeginChangeCheck();

                _contactNamesIndex = EditorGUILayout.Popup(_contactNamesIndex, _contactsNames.Select(x=>x.DefaultText).ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    _contactKeySerializedProperty.stringValue = _contactsNames[_contactNamesIndex].Key;
                    _contactNameSerializedProperty.stringValue = _contactsNames[_contactNamesIndex].DefaultText;
                    serializedObject.ApplyModifiedProperties();
                }
                serializedObject.Update();

            }

            if (_blockContactSerializedProperty.boolValue == false && _unblockContactSerializedProperty.boolValue == false)
            {
                DrawToggle(_blockContactSerializedProperty, "Block contact:");
                DrawToggle(_unblockContactSerializedProperty, "Unblock contact:");
            }
            else if(_blockContactSerializedProperty.boolValue == true && _unblockContactSerializedProperty.boolValue == false)
            {
                DrawToggle(_blockContactSerializedProperty, "Block contact:");
            }
            else if(_blockContactSerializedProperty.boolValue == false && _unblockContactSerializedProperty.boolValue == true)
            {
                DrawToggle(_unblockContactSerializedProperty, "Unblock contact:");
            }
        }
    }

    private void DrawToggle(SerializedProperty serializedProperty, string name)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name);
        serializedProperty.boolValue =
            EditorGUILayout.Toggle(serializedProperty.boolValue);
        EditorGUILayout.EndHorizontal();
    }

    private void InitNamesPhones()
    {
        if (_phoneBlockContactNode.Phones != null && _phoneBlockContactNode.Phones.Count > 0)
        {
            if (_phonesNames == null)
            {
                _phonesNames = new List<string>(_phoneBlockContactNode.Phones.Count);
            }
            else
            {
                _phonesNames.Clear();
            }

            foreach (var phone in _phoneBlockContactNode.Phones)
            {
                _phonesNames.Add(phone.NamePhone);
            }
        }
    }
    
    private void InitNamesContacts()
    {
        if (_phoneBlockContactNode.AllContacts != null && _phoneBlockContactNode.AllContacts.Count > 0)
        {
            if (_contactsNames == null)
            {
                _contactsNames = new List<LocalizationString>(_phoneBlockContactNode.AllContacts.Count);
            }
            else
            {
                _contactsNames.Clear();
            }
            
            foreach (var contact in _phoneBlockContactNode.AllContacts)
            {
                _contactsNames.Add(contact.NameLocalizationString);
            }
        }
    }
}