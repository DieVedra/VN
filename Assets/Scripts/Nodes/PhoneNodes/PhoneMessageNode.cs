

using UnityEngine;

[NodeWidth(200),NodeTint("#325A38")]
public class PhoneMessageNode : BaseNode
{
    [SerializeField] private LocalizationString _localizationString;
    [SerializeField] private PhoneMessageType _type;

    public bool IsReaded;
}