using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogScreenHandler : PhoneScreenBaseHandler
{
    private readonly Image _contactImage;
    private readonly Image _contactStatusImage;
    private readonly TextMeshProUGUI _contactName;
    private readonly TextMeshProUGUI _contactStatus;
    private readonly Button _backArrow;
    private readonly RectTransform _dialogTransform;
    public DialogScreenHandler(DialogScreenView dialogScreenView, TopPanelHandler topPanelHandler)
        :base(dialogScreenView.gameObject, topPanelHandler, dialogScreenView.GradientImage, dialogScreenView.ColorTopPanel)
    {
        _contactImage = dialogScreenView.ContactImage;
        _contactStatusImage = dialogScreenView.ContactStatusImage;
        _contactName = dialogScreenView.ContactName;
        _contactStatus = dialogScreenView.ContactStatus;
        _backArrow = dialogScreenView.BackArrow;
        _dialogTransform = dialogScreenView.DialogTransform;
    }
}