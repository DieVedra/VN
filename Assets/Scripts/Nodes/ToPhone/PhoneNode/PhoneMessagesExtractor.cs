using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
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

	public void Init(NodePort nodePort)
	{
		if (_phoneMessage == null)
		{
			_phoneMessage = new PhoneMessage();
		}
		_phoneMessage.IsReaded = false;
		_phoneMessage.TextMessage = null;
		MessagesIsOut = false;
		_nextNode = nodePort.Connection.node;
	}
	public void Shutdown()
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
					_tryShowNextReactiveCommand.Execute();
				}
				break;
			
			case PhoneMessageNode phoneMessageNode:
				_nextNode = phoneMessageNode.GetNextNode();
				SetMessage(phoneMessageNode);
				
				
				if (_nextNode is EndNode)
				{
					_tryShowNextReactiveCommand.Execute();
				}
				else if (_nextNode is PhoneMessageNode nextPhoneMessageNode)
				{
					if (nextPhoneMessageNode.IsReaded == true)
					{
						_tryShowNextReactiveCommand.Execute();
					}
					else if (nextPhoneMessageNode.Type == PhoneMessageType.Incoming)
					{
						_tryShowNextReactiveCommand.Execute();
					}
				}
				else if (_nextNode is PhoneSwitchNode && phoneMessageNode.IsReaded == true)
				{
					_tryShowNextReactiveCommand.Execute();
				}
				return _phoneMessage;
			
			case NotificationNode notificationNode:
				notificationNode.Enter().Forget();
				_nextNode = notificationNode.GetNextNode();
				_tryShowNextReactiveCommand.Execute();
				break;
			
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
		_phoneMessage.IsReaded = phoneMessageNode.IsReaded;
	}
}