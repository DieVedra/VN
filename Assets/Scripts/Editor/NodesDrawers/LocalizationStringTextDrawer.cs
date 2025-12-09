using UnityEditor;
using UnityEngine;

public class LocalizationStringTextDrawer
{
    private const string _errorText = "Размер строки превышен ";
    private readonly SimpleTextValidator _simpleTextValidator;
    private readonly ObjectProviderFromProperty _objectProviderFromProperty;

    public LocalizationStringTextDrawer(SimpleTextValidator simpleTextValidator)
    {
        _simpleTextValidator = simpleTextValidator;
        _objectProviderFromProperty = new ObjectProviderFromProperty();
    }

    public LocalizationStringTextDrawer()
    {
        _simpleTextValidator = new SimpleTextValidator(100);
        _objectProviderFromProperty = new ObjectProviderFromProperty();
    }
    public void DrawTextField(LocalizationString localizationString, string label, bool drawTextArea = true, bool validateText = true)
    {
        if (_simpleTextValidator == null)
        {
            return;
        }
        _simpleTextValidator.ValidText = localizationString.DefaultText;
        if (drawTextArea)
        {
            EditorGUILayout.LabelField(label);
            _simpleTextValidator.ValidText = EditorGUILayout.TextArea(_simpleTextValidator.ValidText, GUILayout.Height(50f), GUILayout.Width(150f));
        }
        else
        {
            _simpleTextValidator.ValidText = EditorGUILayout.TextField(label, _simpleTextValidator.ValidText, GUILayout.Width(450f));
        }

        if (validateText)
        {
            if (_simpleTextValidator.TryValidate())
            {
                localizationString.SetText(_simpleTextValidator.ValidText);
                localizationString.TryRegenerateKey();
            }
            else
            {
                EditorGUILayout.HelpBox(_errorText , MessageType.Error);
            }
        }
        else
        {
            localizationString.SetText(_simpleTextValidator.ValidText);
            localizationString.TryRegenerateKey();
        }
    }
    public LocalizationString GetLocalizationStringFromProperty(SerializedProperty property)
    {
        return _objectProviderFromProperty.GetPropertyObject<LocalizationString>(property);
    }
}