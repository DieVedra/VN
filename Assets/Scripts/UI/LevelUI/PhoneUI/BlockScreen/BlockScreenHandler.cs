
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BlockScreenHandler : PhoneScreenBaseHandler
{
    private TextMeshProUGUI _time;
    private TextMeshProUGUI _data;
    private Image _notificationContactIcon;
    private TextMeshProUGUI _contactName;
    private TextMeshProUGUI _notificationText;
    private CompositeDisposable _compositeDisposable;
    public BlockScreenHandler(BlockScreenView blockScreenViewBackground, TopPanelHandler topPanelHandler)
    :base(blockScreenViewBackground.gameObject, topPanelHandler, blockScreenViewBackground.ImageBackground, blockScreenViewBackground.ColorTopPanel)
    {
        _time = blockScreenViewBackground.Time;
        _data = blockScreenViewBackground.Data;
        _notificationContactIcon = blockScreenViewBackground.NotificationContactIcon;
        _contactName = blockScreenViewBackground.ContactName;
        _notificationText = blockScreenViewBackground.NotificationText;
    }
    
    public void Enable(PhoneTime phoneTime, Sprite icon, int butteryPercent)
    {
        BaseEnable(phoneTime, butteryPercent);
        Screen.SetActive(true);
        TopPanelHandler.Init(TopPanelColor, phoneTime, butteryPercent);
        _compositeDisposable = new CompositeDisposable();
        Observable.EveryUpdate().Subscribe(_ =>
        {
            _time.text = phoneTime.GetCurrentTime();
        }).AddTo(_compositeDisposable);
        _notificationContactIcon.sprite = icon;
    }

    public override void Disable()
    {
        base.Disable();
        _compositeDisposable?.Clear();
    }

    private void SetTexts(string data, string notificationText, string contactName)
    {
        _data.text = data;
        _notificationText.text = notificationText;
        _contactName.text = contactName;
    }

}