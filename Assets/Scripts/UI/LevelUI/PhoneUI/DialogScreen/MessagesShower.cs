using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using XNode;

public class MessagesShower
{
    private const float _startPositionX = 0f;
    private const float _startPositionY = 0f;
    private const float _offset = 30f;
    private const float _multiplier = 1.6f;
    private readonly float _contentStartPosY;
    private Vector2 _size;
    private PoolBase<MessageView> _incomingMessagePool;
    private PoolBase<MessageView> _outcomingMessagePool;
    private readonly PressDetector _readDialogButton;
    private readonly RectTransform _dialogTransform;
    private readonly ContactPrintStatusHandler _contactPrintStatusHandler;
    private readonly PhoneMessagesExtractor _phoneMessagesExtractor;
    private readonly PhoneMessagesCustodian _phoneMessagesCustodian;
    private readonly ReactiveCommand _tryShowReactiveCommand;

    private CancellationTokenSource _cancellationTokenSource;
    private SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private readonly List<MessageView> _messageViewed;
    private readonly Queue<Func<UniTask>> _queueShowMessages;
    private Vector2 _startPosition = new Vector2(_startPositionX, _startPositionY);
    private Vector2 _pos = new Vector2();
    private CompositeDisposable _compositeDisposable;
    private string _keyPhone;
    private string _keyContact;
    private Action _onMessagesIsOut;
    private bool _inProgress;
    public MessagesShower(RectTransform dialogTransform, ContactPrintStatusHandler contactPrintStatusHandler,
        PhoneMessagesExtractor phoneMessagesExtractor, PhoneMessagesCustodian phoneMessagesCustodian, PressDetector readDialogButton, ReactiveCommand tryShowReactiveCommand)
    {
        _dialogTransform = dialogTransform;
        _contentStartPosY = dialogTransform.anchoredPosition.y;
        _contactPrintStatusHandler = contactPrintStatusHandler;
        _phoneMessagesExtractor = phoneMessagesExtractor;
        _phoneMessagesCustodian = phoneMessagesCustodian;
        _messageViewed = new List<MessageView>();
        _readDialogButton = readDialogButton;
        _queueShowMessages = new Queue<Func<UniTask>>();
        _tryShowReactiveCommand = tryShowReactiveCommand;
        _tryShowReactiveCommand.Subscribe(_=>
        {
            TryShowAll(ShowNext).Forget();
        });
        IncreaseDialogTransform(_offset);
        _cancellationTokenSource = new CancellationTokenSource();
    }
    
    public void InitFromBlockScreen(NodePort nodePort, Action onMessagesIsOut)
    {
        _inProgress = false;
        _phoneMessagesExtractor.Init(nodePort);
        _onMessagesIsOut = onMessagesIsOut;
        _tryShowReactiveCommand.Execute();
    }
    public void InitFromDialogScreen(string keyPhone, string keyContact, ContactNodeCase contactNodeCase,
        PoolBase<MessageView> incomingMessagePool, PoolBase<MessageView> outcomingMessagePool,
        SetLocalizationChangeEvent setLocalizationChangeEvent, Action onMessagesIsOut)
    {
        _keyPhone = keyPhone;
        _keyContact = keyContact;
        _onMessagesIsOut = onMessagesIsOut;
        _compositeDisposable = new CompositeDisposable();
        _incomingMessagePool = incomingMessagePool;
        _outcomingMessagePool = outcomingMessagePool;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _inProgress = false;
        SetDialogToDefaultPos();
        _dialogTransform.sizeDelta = Vector2.zero;
        var readedMessages = _phoneMessagesCustodian.GetMessagesHistory(_keyPhone, _keyContact);
        TryGenerateFromHistory(readedMessages);

        if (contactNodeCase == null)
        {
            _onMessagesIsOut.Invoke();
        }
        else
        {
            _phoneMessagesExtractor.Init(contactNodeCase.Port);
        }
        
        _tryShowReactiveCommand.Execute();
    }

    private void SetDialogToDefaultPos()
    {
        _pos.x = _dialogTransform.anchoredPosition.x;
        _pos.y = _contentStartPosY;
        _dialogTransform.anchoredPosition = _pos;
    }

    public void Shutdown()
    {
        _cancellationTokenSource?.Cancel();
        _messageViewed.Clear();
        _compositeDisposable?.Clear();
        _incomingMessagePool?.ReturnAll();
        _outcomingMessagePool?.ReturnAll();
        _readDialogButton.Shutdown();
    }

    private async UniTask TryShowAll(Func<UniTask> operation)
    {
        _queueShowMessages.Enqueue(operation);
        if (_inProgress == false)
        {
            while (_queueShowMessages.Count > 0)
            {
                await _queueShowMessages.Dequeue().Invoke();
            }
        }
    }
    private async UniTask ShowNext()
    {
        _inProgress = true;
        PhoneMessage phoneMessage = await _phoneMessagesExtractor.GetMessageText();
        if (phoneMessage != null)
        {
            switch (phoneMessage.MessageType)
            {
                case PhoneMessageType.Outcoming:
                    
                    SetOutcomingMessage(phoneMessage);
                    break;
                case PhoneMessageType.Incoming:

                    if (phoneMessage.IsReaded == false)
                    {
                        await _contactPrintStatusHandler.IndicateOnPrint(_setLocalizationChangeEvent, phoneMessage.TextMessage.DefaultText.Length);
                    }
                    SetIncomingMessage(phoneMessage);
                    break;
            }
        }

        if (_phoneMessagesExtractor.MessagesIsOut == false)
        {
            if (_queueShowMessages.Count == 0)
            {
                SubscribeReadButton();
            }
        }
        else
        {
            _onMessagesIsOut?.Invoke();
        }

        _inProgress = false;
    }

    private void SetIncomingMessage(PhoneMessage phoneMessage, bool addToMessageHistoryKey = true)
    {
        SetMessage(_incomingMessagePool.Get(), phoneMessage, addToMessageHistoryKey);
    }

    private void SetOutcomingMessage(PhoneMessage phoneMessage, bool addToMessageHistoryKey = true)
    {
        SetMessage(_outcomingMessagePool.Get(), phoneMessage, addToMessageHistoryKey);
    }

    private void SetMessage(MessageView view, PhoneMessage phoneMessage, bool addToMessageHistoryKey)
    {
        SetDialogToDefaultPos();
        view.Text.text = phoneMessage.TextMessage;
        _setLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            view.Text.text = phoneMessage.TextMessage;
            ResizePanel(view);
        }, _compositeDisposable);
        view.gameObject.SetActive(true);
        
        ResizePanel(view);
        
        _startPosition.x = view.ViewRectTransform.anchoredPosition.x;
        float sizeDeltaY = view.ImageRectTransform.sizeDelta.y;
        float offset = sizeDeltaY + _offset;
        _startPosition.y = _startPositionY + offset;
        
        view.ViewRectTransform.anchoredPosition = _startPosition;
        
        for (int i = 0; i < _messageViewed.Count; i++)
        {
            _pos.x = _messageViewed[i].ViewRectTransform.anchoredPosition.x;
            _pos.y = _messageViewed[i].ViewRectTransform.anchoredPosition.y + offset;
            _messageViewed[i].ViewRectTransform.anchoredPosition = _pos;
        }

        _phoneMessagesCustodian.AddMessageHistory(_keyPhone, _keyContact, phoneMessage);
        if (addToMessageHistoryKey)
        {
            IncreaseDialogTransform(offset);
        }

        _messageViewed.Add(view);
    }

    private void IncreaseDialogTransform(float offset)
    {
        _pos = _dialogTransform.sizeDelta;
        _pos.y += offset;
        _dialogTransform.sizeDelta = _pos;
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
        if (_readDialogButton.IsActive == false)
        {
            _readDialogButton.Enable(() =>
            {
                _readDialogButton.Shutdown();
                _tryShowReactiveCommand.Execute();
            });
        }
    }

    private void TryGenerateFromHistory(IReadOnlyList<PhoneMessage> list)
    {
        if (list != null && list.Count > 0)
        {
            foreach (var item in list)
            {
                switch (item.MessageType)
                {
                    case PhoneMessageType.Outcoming:
                        SetOutcomingMessage(item, false);
                        break;
                    case PhoneMessageType.Incoming:
                        SetIncomingMessage(item, false);
                        break;
                }
            }
        }
    }
}