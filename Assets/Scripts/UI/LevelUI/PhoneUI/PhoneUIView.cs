using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneUIView : MonoBehaviour
{
    [SerializeField, BoxGroup("Оформление")] private Image _handImage;
    [SerializeField, BoxGroup("Оформление")] private Image _curtainImage;
    [SerializeField, BoxGroup("Оформление")] private Image _frameImage;

    [SerializeField, BoxGroup("Top panel")] private RectTransform _signalIndicatorRectTransform;
    [SerializeField, BoxGroup("Top panel")] private List<Image> _signalIndicatorImage;
    [SerializeField, BoxGroup("Top panel")] private TextMeshProUGUI _timeText;
    [SerializeField, BoxGroup("Top panel")] private TextMeshProUGUI _butteryText;
    [SerializeField, BoxGroup("Top panel")] private Image _butteryImage;
    [SerializeField, BoxGroup("Top panel")] private Image _butteryIndicatorImage;
    
    [SerializeField, BoxGroup("Backgrounds")] private BlockScreenView _blockScreenViewBackground;
    [SerializeField, BoxGroup("Backgrounds")] private DialogScreenView _dialogScreenViewBackground;
    [SerializeField, BoxGroup("Backgrounds")] private ContactsScreenView _contactsScreenViewBackground;
    
    public RectTransform SignalIndicatorRectTransform => _signalIndicatorRectTransform;
    public IReadOnlyList<Image> SignalIndicatorImage => _signalIndicatorImage;
    public TextMeshProUGUI TimeText => _timeText;
    public TextMeshProUGUI ButteryText => _butteryText;
    public Image ButteryImage => _butteryImage;
    public Image ButteryIndicatorImage => _butteryIndicatorImage;
    public Image CurtainImage => _curtainImage;
    public Image HandImage => _handImage;
    public Image FrameImage => _frameImage;
    
    public BlockScreenView BlockScreenViewBackground => _blockScreenViewBackground;
    public DialogScreenView DialogScreenViewBackground => _dialogScreenViewBackground;
    public ContactsScreenView ContactsScreenViewBackground => _contactsScreenViewBackground;
}