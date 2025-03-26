
using UnityEngine;

[System.Serializable]
public class LocalizableString
{
    [TextArea] public string Text;
    [SerializeField, TextArea] private string _text;
}