using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MessagesShower
{
    private const float _fadeEndValue = 0f;
    private const float _unfadeEndValue = 1f;
    private const float _duration = 0.2f;
    private const float _startPositionX = 0f;
    private const float _startPositionY = -666f;
    private const float _offset = 25f;
    private PoolBase<MessageView> _incomingMessagePool;
    private PoolBase<MessageView> _outcomingMessagePool;
    private IReadOnlyList<PhoneMessageLocalization> _phoneMessagesLocalization;
    private Image _blackoutImage;
    private PanelSizeHandler _panelSizeHandler;
    private readonly NarrativePanelUIHandler _narrativePanelUI;
    private PhoneMessageType _lastMessageType;
    private CancellationTokenSource _cancellationTokenSource;
    private SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private List<MessageView> _messageViewed;
    private int _index = 0;
    private readonly Vector2 _startPosition = new Vector2(_startPositionX, _startPositionY);
    private Vector2 _pos = new Vector2();
    private CompositeDisposable _compositeDisposable;
    private bool _inProgress;
    public MessagesShower(Image blackoutImage, NarrativePanelUIHandler narrativePanelUI)
    {
        _blackoutImage = blackoutImage;
        _narrativePanelUI = narrativePanelUI;
        _messageViewed = new List<MessageView>();
        _panelSizeHandler = new PanelSizeHandler(new LineBreaksCountCalculator(), new PhoneMessagePanelSizeCurveProvider());
    }

    public void Init(IReadOnlyList<PhoneMessageLocalization> phoneMessagesLocalization,
        PoolBase<MessageView> incomingMessagePool, PoolBase<MessageView> outcomingMessagePool, SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        _compositeDisposable = new CompositeDisposable();
        _phoneMessagesLocalization = phoneMessagesLocalization;
        _incomingMessagePool = incomingMessagePool;
        _outcomingMessagePool = outcomingMessagePool;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _index = 0;
        _inProgress = false;
        for (int i = 0; i < phoneMessagesLocalization.Count; i++)
        {
            if (phoneMessagesLocalization[i].IsReaded == true)
            {
                ShowNext();
            }
            else
            {
                ShowNext(); 
                break;
            }
        }
    }

    public void Dispose()
    {
        _messageViewed.Clear();
        _compositeDisposable?.Clear();
        _incomingMessagePool.ReturnAll();
        _outcomingMessagePool.ReturnAll();
    }
    public void ShowNext()
    {
        if (_inProgress == false && _phoneMessagesLocalization.Count >= _index - 1)
        {
            _inProgress = true;
            PhoneMessageLocalization phoneMessageLocalization = _phoneMessagesLocalization[_index];
            switch (phoneMessageLocalization.MessageType)
            {
                case PhoneMessageType.Outcoming:
                    SetOutcomingMessage(phoneMessageLocalization);
                    _lastMessageType = PhoneMessageType.Outcoming;
                    break;
                case PhoneMessageType.Incoming:
                    SetIncomingMessage(phoneMessageLocalization);
                    _lastMessageType = PhoneMessageType.Incoming;
                    break;
                case PhoneMessageType.Narrative:
                    SetNarrativeMessage(phoneMessageLocalization.TextMessageLocalizationString).Forget();
                    _lastMessageType = PhoneMessageType.Narrative;
                    break;
            }
            _index++;
            _inProgress = false;
        }
    }
    
    private void SetIncomingMessage(PhoneMessageLocalization phoneMessageLocalization)
    {
        SetMessage(_outcomingMessagePool.Get(), phoneMessageLocalization);
    }

    private void SetOutcomingMessage(PhoneMessageLocalization phoneMessageLocalization)
    {
        SetMessage(_incomingMessagePool.Get(), phoneMessageLocalization);
    }

    private void SetMessage(MessageView view, PhoneMessageLocalization phoneMessageLocalization)
    {
        TryHideNarrativeMessage();
        view.RectTransform.anchoredPosition = _startPosition;
        _panelSizeHandler.UpdateSize(view.ImageRectTransform, view.Text, phoneMessageLocalization.TextMessageLocalizationString);
        var newPosYOffset = view.ImageRectTransform.sizeDelta.y + _offset;
        for (int i = 0; i < _messageViewed.Count; i++)
        {
            _pos.x = _messageViewed[i].RectTransform.anchoredPosition.x;
            _pos.y = _messageViewed[i].RectTransform.anchoredPosition.y + newPosYOffset;
            _messageViewed[i].RectTransform.anchoredPosition = _pos;
        }
        _messageViewed.Add(view);
        view.Text.text = phoneMessageLocalization.TextMessageLocalizationString;
        _setLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            view.Text.text = phoneMessageLocalization.TextMessageLocalizationString;
        }, _compositeDisposable);
        view.gameObject.SetActive(true);
        phoneMessageLocalization.IsReaded = true;
    }
    private async UniTaskVoid SetNarrativeMessage(string text)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _narrativePanelUI.EmergenceNarrativePanelInPlayMode(text);

        if (_lastMessageType == PhoneMessageType.Narrative)
        {
            await _narrativePanelUI.AnimationPanel.UnfadePanel(_cancellationTokenSource.Token);
        }
        else
        {
            await UniTask.WhenAll(
                _blackoutImage.DOFade(_fadeEndValue, _duration).WithCancellation(_cancellationTokenSource.Token),
                _narrativePanelUI.AnimationPanel.UnfadePanel(_cancellationTokenSource.Token));
        }
        
        await _narrativePanelUI.TextConsistentlyViewer.SetTextConsistently(_cancellationTokenSource.Token, text);
    }
    
    private void TryHideNarrativeMessage()
    {
        if (_lastMessageType == PhoneMessageType.Narrative)
        {
            UniTask.WhenAll(
                _blackoutImage.DOFade(_unfadeEndValue, _duration).WithCancellation(_cancellationTokenSource.Token),
                _narrativePanelUI.AnimationPanel.FadePanel(_cancellationTokenSource.Token));
        }
    }
}