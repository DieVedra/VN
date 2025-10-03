using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class MessagesShower
{
    private const int _sublingIndexBlackoutFrame = 6;
    private const float _fadeEndValue = 0.3f;
    private const float _unfadeEndValue = 0f;
    private const float _duration = 0.2f;
    private const float _startPositionX = 0f;
    private const float _startPositionY = -666f;
    private const float _offset = 25f;
    private PoolBase<MessageView> _incomingMessagePool;
    private PoolBase<MessageView> _outcomingMessagePool;
    private IReadOnlyList<PhoneMessageLocalization> _phoneMessagesLocalization;
    private PanelSizeHandler _panelSizeHandler;
    private Action _setOnlineStatus;
    private readonly CustomizationCurtainUIHandler _curtainUIHandler;
    private readonly NarrativePanelUIHandler _narrativePanelUI;
    private PhoneMessageType _lastMessageType;
    private CancellationTokenSource _cancellationTokenSource;
    private SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private List<MessageView> _messageViewed;
    private int _index = 0;
    private Vector2 _startPosition = new Vector2(_startPositionX, _startPositionY);
    private Vector2 _pos = new Vector2();
    private CompositeDisposable _compositeDisposable;
    private int _blackoutFrameSublingIndexBufer;
    private bool _inProgress;
    private bool _resultShowText;
    private Color _colorHide = new Color(_fadeEndValue,_fadeEndValue,_fadeEndValue,_fadeEndValue);
    public MessagesShower(CustomizationCurtainUIHandler curtainUIHandler, NarrativePanelUIHandler narrativePanelUI)
    {
        _curtainUIHandler = curtainUIHandler;
        _narrativePanelUI = narrativePanelUI;
        _messageViewed = new List<MessageView>();
        _panelSizeHandler = new PanelSizeHandler(new LineBreaksCountCalculator(), new PhoneMessagePanelSizeCurveProvider());
    }

    public void Init(IReadOnlyList<PhoneMessageLocalization> phoneMessagesLocalization,
        PoolBase<MessageView> incomingMessagePool, PoolBase<MessageView> outcomingMessagePool,
        SetLocalizationChangeEvent setLocalizationChangeEvent, Action setOnlineStatus)
    {
        _compositeDisposable = new CompositeDisposable();
        _phoneMessagesLocalization = phoneMessagesLocalization;
        _incomingMessagePool = incomingMessagePool;
        _outcomingMessagePool = outcomingMessagePool;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _setOnlineStatus = setOnlineStatus;
        _index = 0;
        _inProgress = false;
        _resultShowText = false;
        for (int i = 0; i < phoneMessagesLocalization.Count; i++)
        {
            if (phoneMessagesLocalization[i].IsReaded == true)
            {
                ShowNext();
            }
            else
            {
                if (phoneMessagesLocalization[i].MessageType == PhoneMessageType.Incoming)
                {
                    ShowNext();
                }
                break;
            }
        }
    }

    public void Dispose()
    {
        _messageViewed.Clear();
        _compositeDisposable?.Clear();
        _incomingMessagePool?.ReturnAll();
        _outcomingMessagePool?.ReturnAll();
    }
    public bool ShowNext()
    {
        if (_inProgress == false && _index < _phoneMessagesLocalization.Count)
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
                    if (phoneMessageLocalization.IsReaded == false)
                    {
                        SetNarrativeMessage(phoneMessageLocalization.TextMessageLocalizationString).Forget();
                        _lastMessageType = PhoneMessageType.Narrative;
                    }
                    break;
            }

            _index++;
            _resultShowText = true;
            _inProgress = false;
        }
        else if (_index == _phoneMessagesLocalization.Count)
        {
            _index += _phoneMessagesLocalization.Count;
            _resultShowText = true;
            _setOnlineStatus.Invoke();
        }
        else
        {
            _resultShowText = false;
        }

        return _resultShowText;
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
        _startPosition.x = view.RectTransform.anchoredPosition.x;
        view.RectTransform.anchoredPosition = _startPosition;
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
        _panelSizeHandler.UpdateSize(view.ImageRectTransform, view.Text, phoneMessageLocalization.TextMessageLocalizationString, false);
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
            _blackoutFrameSublingIndexBufer = _curtainUIHandler.Transform.GetSiblingIndex();
            _curtainUIHandler.Transform.SetSiblingIndex(_sublingIndexBlackoutFrame);
            _curtainUIHandler.CurtainImage.raycastTarget = false;
            _curtainUIHandler.CurtainImage.color = _colorHide;
            _curtainUIHandler.CurtainImage.gameObject.SetActive(true);
            await UniTask.WhenAll(
                _curtainUIHandler.CurtainImage.DOFade(_fadeEndValue, _duration).WithCancellation(_cancellationTokenSource.Token),
                _narrativePanelUI.AnimationPanel.UnfadePanel(_cancellationTokenSource.Token));
        }
        
        await _narrativePanelUI.TextConsistentlyViewer.SetTextConsistently(_cancellationTokenSource.Token, text);
    }
    
    private void TryHideNarrativeMessage()
    {
        if (_lastMessageType == PhoneMessageType.Narrative)
        {
            _curtainUIHandler.Transform.SetSiblingIndex(_blackoutFrameSublingIndexBufer);
            _curtainUIHandler.CurtainImage.raycastTarget = true;
            _curtainUIHandler.CurtainImage.gameObject.SetActive(false);
            UniTask.WhenAll(
                _curtainUIHandler.CurtainImage.DOFade(_unfadeEndValue, _duration).WithCancellation(_cancellationTokenSource.Token),
                _narrativePanelUI.AnimationPanel.FadePanel(_cancellationTokenSource.Token));
        }
    }
}