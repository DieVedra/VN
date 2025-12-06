using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using XNode;

public class PhoneMessagesExtractor
{
	private readonly ReactiveCommand _tryShowNextReactiveCommand;
	private Node _nextNode;
	private CancellationTokenSource _cancellationTokenSource;
	private PhoneMessage _phoneMessage;
	public bool MessagesIsOut { get; private set; }

	public PhoneMessagesExtractor(ReactiveCommand tryShowNextReactiveCommand)
	{
		_tryShowNextReactiveCommand = tryShowNextReactiveCommand;
	}

	public void Init(ContactNodeCase contactNodeCase)
	{
		_phoneMessage = new PhoneMessage();
		MessagesIsOut = false;
		_nextNode = contactNodeCase.Port.Connection.node;
	}
	public void Dispose()
	{
		_cancellationTokenSource?.Cancel();
	}
	public async UniTask<PhoneMessage> GetMessageText()
	{
		switch (_nextNode)
		{
			case ChoicePhoneNode choicePhoneNode:
				await choicePhoneNode.Enter();
				_cancellationTokenSource = new CancellationTokenSource();
				await UniTask.WaitUntil(() => choicePhoneNode.IsOver == true, cancellationToken: _cancellationTokenSource.Token);
				choicePhoneNode.SetMessage(_phoneMessage);
				_nextNode = choicePhoneNode.GetNextNode();
				_tryShowNextReactiveCommand.Execute();
				return _phoneMessage;

			case PhoneSwitchNode phoneSwitchNode:
				await phoneSwitchNode.Enter();
				_cancellationTokenSource = new CancellationTokenSource();
				await UniTask.WaitUntil(() => phoneSwitchNode.IsOver == true, cancellationToken: _cancellationTokenSource.Token);
				_nextNode = phoneSwitchNode.GetNextNode();
				_tryShowNextReactiveCommand.Execute();
				break;

			case PhoneNarrativeMessageNode phoneNarrativeMessageNode:

				if (phoneNarrativeMessageNode.IsEntered == false)
				{
					await phoneNarrativeMessageNode.Enter();
				}
				else
				{
					await phoneNarrativeMessageNode.Exit();
					
					_nextNode = phoneNarrativeMessageNode.GetNextNode();
					if (_nextNode is PhoneNarrativeMessageNode castNode)
					{
						await castNode.Enter();
					}
					else
					{
						_tryShowNextReactiveCommand.Execute();
					}
				}
				break;
			
			case PhoneMessageNode phoneMessageNode:
				_nextNode = phoneMessageNode.GetNextNode();
				SetMessage(phoneMessageNode);
				// if (_nextNode is PhoneNarrativeMessageNode)
				// {
				// 	_tryShowNextReactiveCommand.Execute();
				// }
				return _phoneMessage;
			
			case NotificationNode notificationNode:
				notificationNode.Enter().Forget();
				_nextNode = notificationNode.GetNextNode();
				if (_nextNode is PhoneMessageNode nextPhoneMessageNode)
				{
					_nextNode = nextPhoneMessageNode.GetNextNode();
					SetMessage(nextPhoneMessageNode);
				}
				return _phoneMessage;
			
			case EndNode endNode:
				MessagesIsOut = true;
				break;
		}
		return default;
	}

	private void SetMessage(PhoneMessageNode phoneMessageNode)
	{
		_phoneMessage.TextMessage = phoneMessageNode.GetLocalizationString;
		_phoneMessage.MessageType = phoneMessageNode.Type;
	}
}