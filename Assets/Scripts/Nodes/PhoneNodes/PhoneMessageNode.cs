using System.Collections.Generic;
using UnityEngine;

[NodeWidth(200),NodeTint("#325A38")]
public class PhoneMessageNode : BaseNode, ILocalizable
{
    [SerializeField] private LocalizationString _localizationString;
    [SerializeField] private PhoneMessageType _type;
    
    public LocalizationString GetLocalizationString => _localizationString;
    public PhoneMessageType Type => _type;

    public bool IsReaded;

#if UNITY_EDITOR
    protected override void SetInfoToView()
    {
        Debug.Log($"{_localizationString}");
    }
#endif

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_localizationString};
    }
}