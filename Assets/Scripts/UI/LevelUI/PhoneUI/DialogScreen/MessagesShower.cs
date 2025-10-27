using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class MessagesShower : PanelUIHandler
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
    private CompositeDisposable _narrativeCompositeDisposable;
    private int _blackoutFrameSublingIndexBufer;
    private bool _inProgress;
    private bool _resultShowText;
    private bool _characterOnlineKey;
    private Color _colorHide = new Color(_fadeEndValue,_fadeEndValue,_fadeEndValue,_fadeEndValue);
    public MessagesShower(CustomizationCurtainUIHandler curtainUIHandler, NarrativePanelUIHandler narrativePanelUI)
    {
        _curtainUIHandler = curtainUIHandler;
        _narrativePanelUI = narrativePanelUI;
        _messageViewed = new List<MessageView>();
    }

    public void Init(IReadOnlyList<PhoneMessageLocalization> phoneMessagesLocalization,
        PoolBase<MessageView> incomingMessagePool, PoolBase<MessageView> outcomingMessagePool,
        SetLocalizationChangeEvent setLocalizationChangeEvent, Action setOnlineStatus, Action disableReadButton, bool characterOnlineKey)
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
        _characterOnlineKey = characterOnlineKey;
        if (phoneMessagesLocalization.Count == 0)
        {
            disableReadButton.Invoke();
        }
        for (int i = 0; i < phoneMessagesLocalization.Count; i++)
        {
            
            if (phoneMessagesLocalization[i].IsReaded == true)
            {
                ShowNext();
                TryDisableReadButtonAndEnableBack(i);
            }
            else
            {
                if (phoneMessagesLocalization[i].MessageType == PhoneMessageType.Incoming)
                {
                    ShowNext();
                    TryDisableReadButtonAndEnableBack(i);
                }
                break;
            }
        }

        void TryDisableReadButtonAndEnableBack(int i)
        {
            if (i == phoneMessagesLocalization.Count - 1)
            {
                disableReadButton.Invoke();
            }
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _narrativeCompositeDisposable?.Clear();
        _messageViewed.Clear();
        _compositeDisposable?.Clear();
        _incomingMessagePool?.ReturnAll();
        _outcomingMessagePool?.ReturnAll();
        TryHideNarrativeMessage();
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
                        SetNarrativeMessage(phoneMessageLocalization).Forget();
                        _lastMessageType = PhoneMessageType.Narrative;
                    }
                    break;
            }

            _index++;
            _resultShowText = true;
            _inProgress = false;
        }
        else
        {
            ResultFalse();
        }

        void ResultFalse()
        {
            _resultShowText = false;
            _setOnlineStatus.Invoke();
        }
        return _resultShowText;
    }
    
    private void SetIncomingMessage(PhoneMessageLocalization phoneMessageLocalization)
    {
        SetMessage(_incomingMessagePool.Get(), phoneMessageLocalization);
    }

    private void SetOutcomingMessage(PhoneMessageLocalization phoneMessageLocalization)
    {
        SetMessage(_outcomingMessagePool.Get(), phoneMessageLocalization);
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
        view.Text.text = phoneMessageLocalization.TextMessage;
        _setLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            view.Text.text = phoneMessageLocalization.TextMessage;
            ResizePanel(view);
        }, _compositeDisposable);
        view.gameObject.SetActive(true);
        
        ResizePanel(view);
        phoneMessageLocalization.IsReaded = true;
    }
    private async UniTaskVoid SetNarrativeMessage(PhoneMessageLocalization phoneMessageLocalization)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        phoneMessageLocalization.IsReaded = true;
        
        if (_lastMessageType == PhoneMessageType.Narrative)
        {
            await _narrativePanelUI.DisappearanceNarrativePanelInPlayMode(_cancellationTokenSource.Token);
        }
        else
        {
            _narrativeCompositeDisposable?.Clear();
            _narrativeCompositeDisposable = new CompositeDisposable();
            _setLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
            {
                _narrativePanelUI.SetText(phoneMessageLocalization.TextMessage);
            }, _narrativeCompositeDisposable);
            
            _blackoutFrameSublingIndexBufer = _curtainUIHandler.Transform.GetSiblingIndex();
            _curtainUIHandler.Transform.SetSiblingIndex(_sublingIndexBlackoutFrame);
            _curtainUIHandler.CurtainImage.raycastTarget = false;
            _curtainUIHandler.CurtainImage.color = _colorHide;
            _curtainUIHandler.CurtainImage.gameObject.SetActive(true);
            await UniTask.WhenAll(
                _curtainUIHandler.CurtainImage.DOFade(_fadeEndValue, _duration).WithCancellation(_cancellationTokenSource.Token),
                _narrativePanelUI.EmergenceNarrativePanelInPlayMode(phoneMessageLocalization.TextMessage, _cancellationTokenSource.Token));
        }
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
                _narrativePanelUI.DisappearanceNarrativePanelInPlayMode(_cancellationTokenSource.Token));
        }
    }
    private void ResizePanel(MessageView view)
    {
        view.Text.ForceMeshUpdate();
        Size = view.Text.GetRenderedValues(true);
        Size.x = view.ImageRectTransform.sizeDelta.x;
        Size.y = Size.y + view.HeightOffset * Multiplier;
        view.ImageRectTransform.sizeDelta = Size;
    }
}