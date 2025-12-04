using System.Threading;
using Cysharp.Threading.Tasks;
using XNode;

public class PhoneMessagesExtractor
{
	private Node _nextNode;
	private CancellationTokenSource _cancellationTokenSource;
	private PhoneMessage _phoneMessage;
    public bool MessagesIsOut { get; private set; }
    
	public void Init(ContactNodeCase contactNodeCase)
	{
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
				_nextNode = choicePhoneNode.PhoneMessageNode.GetNextNode();
				SetMessage(choicePhoneNode.PhoneMessageNode);
				return _phoneMessage;

			case PhoneSwitchNode phoneSwitchNode:
				await phoneSwitchNode.Enter();
				_cancellationTokenSource = new CancellationTokenSource();
				await UniTask.WaitUntil(() => phoneSwitchNode.IsOver == true, cancellationToken: _cancellationTokenSource.Token);
				_nextNode = phoneSwitchNode.PhoneMessageNode.GetNextNode();
				SetMessage(phoneSwitchNode.PhoneMessageNode);
				return _phoneMessage;

			case PhoneNarrativeMessageNode phoneNarrativeMessageNode:
				if (phoneNarrativeMessageNode.IsEntered == false)
				{
					await phoneNarrativeMessageNode.Enter();
				}
				else
				{
					await phoneNarrativeMessageNode.Exit();
					_nextNode = phoneNarrativeMessageNode.PhoneMessageNode.GetNextNode();
					SetMessage(phoneNarrativeMessageNode.PhoneMessageNode);
					return _phoneMessage;
				}
				break;
			
			case PhoneMessageNode phoneMessageNode:
				_nextNode = phoneMessageNode.GetNextNode();
				SetMessage(phoneMessageNode);
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