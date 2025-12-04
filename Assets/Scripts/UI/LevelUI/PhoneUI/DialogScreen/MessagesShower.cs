using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MessagesShower
{
    private const int _sublingIndexAddebleValue = 1;
    private const int _defaultValue = 0;
    private const float _startPositionX = 0f;
    private const float _startPositionY = -666f;
    private const float _offset = 30f;
    private const float _multiplier = 1.6f;
    private Vector2 _size;
    private PoolBase<MessageView> _incomingMessagePool;
    private PoolBase<MessageView> _outcomingMessagePool;
    private Action _setOnlineStatus;
    private readonly CustomizationCurtainUIHandler _curtainUIHandler;
    private readonly NarrativePanelUIHandler _narrativePanelUI;
    private readonly Button _readDialogButton;
    private readonly PhoneMessagesExtractor _phoneMessagesExtractor;
    private readonly int _phoneSublingIndex;
    private PhoneMessageType _lastMessageType;
    private CancellationTokenSource _cancellationTokenSource;
    private SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private List<MessageView> _messageViewed;
    private int _index = 0;
    private Vector2 _startPosition = new Vector2(_startPositionX, _startPositionY);
    private Vector2 _pos = new Vector2();
    private CompositeDisposable _compositeDisposable;
    private CompositeDisposable _narrativeCompositeDisposable;
    private Action _activateBackButton;
    private bool _inProgress;
    private bool _resultShowText;
    private bool _characterOnlineKey;
    public MessagesShower(PhoneMessagesExtractor phoneMessagesExtractor, Button readDialogButton, CustomizationCurtainUIHandler curtainUIHandler, NarrativePanelUIHandler narrativePanelUI, int phoneSublingIndex)
    {
        _phoneMessagesExtractor = phoneMessagesExtractor;
        _curtainUIHandler = curtainUIHandler;
        _narrativePanelUI = narrativePanelUI;
        _phoneSublingIndex = phoneSublingIndex + _sublingIndexAddebleValue;
        _messageViewed = new List<MessageView>();
        _readDialogButton = readDialogButton;
    }

    public void Init(ContactNodeCase contactNodeCase,
        PoolBase<MessageView> incomingMessagePool, PoolBase<MessageView> outcomingMessagePool,
        SetLocalizationChangeEvent setLocalizationChangeEvent, Action setOnlineStatus, Action activateBackButton, bool characterOnlineKey)
    {
        _activateBackButton = activateBackButton;
        _compositeDisposable = new CompositeDisposable();
        _incomingMessagePool = incomingMessagePool;
        _outcomingMessagePool = outcomingMessagePool;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _setOnlineStatus = setOnlineStatus;
        _index = _defaultValue;
        _inProgress = false;
        _resultShowText = false;
        _characterOnlineKey = characterOnlineKey;

//логика вывода уже прочитанных сообщений
        if (contactNodeCase == null)
        {
            activateBackButton.Invoke();

        }
        else
        {
            _phoneMessagesExtractor.Init(contactNodeCase);
        }

        
        // for (int i = 0; i < phoneMessagesLocalization.Count; i++)
        // {
        //     
        //     if (phoneMessagesLocalization[i].IsReaded == true)
        //     {
        //         ShowNext();
        //         TryDisableReadButtonAndEnableBack(i);
        //     }
        //     else
        //     {
        //         if (phoneMessagesLocalization[i].MessageType == PhoneMessageType.Incoming)
        //         {
        //             ShowNext();
        //             TryDisableReadButtonAndEnableBack(i);
        //         }
        //         break;
        //     }
        // }



        ShowNext().Forget();
        // void TryDisableReadButtonAndEnableBack(int i)
        // {
        //     if (i == phoneMessagesLocalization.Count - 1)
        //     {
        //         activateBackButton.Invoke();
        //     }
        // }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _narrativeCompositeDisposable?.Clear();
        _messageViewed.Clear();
        _compositeDisposable?.Clear();
        _incomingMessagePool?.ReturnAll();
        _outcomingMessagePool?.ReturnAll();
    }

    private async UniTask ShowNext()
    {
        PhoneMessage phoneMessage = await _phoneMessagesExtractor.GetMessageText();
        if (phoneMessage != null)
        {
            switch (phoneMessage.MessageType)
            {
                case PhoneMessageType.Outcoming:
                    SetOutcomingMessage(phoneMessage);
                    _lastMessageType = PhoneMessageType.Outcoming;
                    break;
                case PhoneMessageType.Incoming:
                    SetIncomingMessage(phoneMessage);
                    _lastMessageType = PhoneMessageType.Incoming;
                    break;
            }
        }

        if (_phoneMessagesExtractor.MessagesIsOut == false)
        {
            SubscribeReadButton();
        }
        else
        {
            _activateBackButton.Invoke();
        }
    }

    private void SetIncomingMessage(PhoneMessage phoneMessage)
    {
        SetMessage(_incomingMessagePool.Get(), phoneMessage);
    }

    private void SetOutcomingMessage(PhoneMessage phoneMessage)
    {
        SetMessage(_outcomingMessagePool.Get(), phoneMessage);
    }

    private void SetMessage(MessageView view, PhoneMessage phoneMessageLocalization)
    {
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
        // phoneMessageLocalization.IsReaded = true;
    }
    private void ResizePanel(MessageView view)
    {
        view.Text.ForceMeshUpdate();
        _size = view.Text.GetRenderedValues(true);
        _size.x = view.ImageRectTransform.sizeDelta.x;
        _size.y = _size.y + view.Text.fontSize * _multiplier;
        view.ImageRectTransform.sizeDelta = _size;
    }

    private void SubscribeReadButton()
    {
        _readDialogButton.interactable = true;
        _readDialogButton.onClick.AddListener(() =>
        {
            _readDialogButton.interactable = false;
            _readDialogButton.onClick.RemoveAllListeners();
            ShowNext().Forget();
        } );
    }
}