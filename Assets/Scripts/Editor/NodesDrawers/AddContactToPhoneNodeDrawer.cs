
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditorAttribute(typeof(AddContactToPhoneNode))]

public class AddContactToPhoneNodeDrawer : NodeEditor
{
    private AddContactToPhoneNode _addContactToPhoneNode;
    private SerializedProperty _phoneIndexSerializableProperty;
    private SerializedProperty _contactIndexSerializableProperty;
    private SerializedProperty _addContactSerializableProperty;
    private SerializedProperty _inputSerializableProperty;
    private SerializedProperty _outputSerializableProperty;
    private string[] _namesPhones;
    private string[] _namesContacts;
    public override void OnBodyGUI()
    {
        if (_addContactToPhoneNode == null)
        {
            _addContactToPhoneNode = target as AddContactToPhoneNode;
        }
        else
        {
            if (_phoneIndexSerializableProperty == null)
            {
                _phoneIndexSerializableProperty = serializedObject.FindProperty("_phoneIndex");
                _contactIndexSerializableProperty = serializedObject.FindProperty("_contactIndex");
                _addContactSerializableProperty = serializedObject.FindProperty("_addContact");
                _inputSerializableProperty = serializedObject.FindProperty("Input");
                _outputSerializableProperty = serializedObject.FindProperty("Output");
            }
            NodeEditorGUILayout.PropertyField(_inputSerializableProperty);
            NodeEditorGUILayout.PropertyField(_outputSerializableProperty);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Add contact: ", GUILayout.Width(100f));
            _addContactSerializableProperty.boolValue = EditorGUILayout.Toggle(_addContactSerializableProperty.boolValue);
            EditorGUILayout.EndHorizontal();
            if (_addContactSerializableProperty.boolValue == true)
            {
                if (_addContactToPhoneNode.Phones != null && _addContactToPhoneNode.Phones.Count > 0)
                {
                    InitNamesPhones();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("To Phone: ", GUILayout.Width(100f));
                    _phoneIndexSerializableProperty.intValue = EditorGUILayout.Popup(_phoneIndexSerializableProperty.intValue, _namesPhones,  GUILayout.Width(80f));
                    EditorGUILayout.EndHorizontal();
                }

                if (_addContactToPhoneNode.Contacts != null && _addContactToPhoneNode.Contacts.Count > 0)
                {
                    InitNamesContacts();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Contact: ", GUILayout.Width(100f));
                    _contactIndexSerializableProperty.intValue = EditorGUILayout.Popup(_contactIndexSerializableProperty.intValue, _namesContacts,  GUILayout.Width(80f));
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }

    private void InitNamesPhones()
    {
        List<string> names = new List<string>();
        for (int i = 0; i < _addContactToPhoneNode.Phones.Count; i++)
        {
            names.Add(_addContactToPhoneNode.Phones[i].NamePhone);
        }
        _namesPhones = names.ToArray();
    }
    private void InitNamesContacts()
    {
        List<string> names = new List<string>();
        for (int i = 0; i < _addContactToPhoneNode.Contacts.Count; i++)
        {
            names.Add(_addContactToPhoneNode.Contacts[i].NameContactLocalizationString);
        }
        _namesContacts = names.ToArray();
    }
}