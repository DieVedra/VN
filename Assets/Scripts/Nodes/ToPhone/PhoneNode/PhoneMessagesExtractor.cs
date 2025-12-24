using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using XNode;

public class PhoneMessagesExtractor
{
	private readonly ReactiveCommand _tryShowNextReactiveCommand;
	private Node _nextNode;
	private Node _nodeToSave;
	private CancellationTokenSource _cancellationTokenSource;
	public bool MessagesIsOut { get; private set; }

	public PhoneMessagesExtractor(ReactiveCommand tryShowNextReactiveCommand)
	{
		_tryShowNextReactiveCommand = tryShowNextReactiveCommand;
	}

	public void Init(NodePort nodePort)
	{
		_nodeToSave = _nextNode = nodePort.Connection.node;
		MessagesIsOut = false;
	}

	public int GetCurrentNodeIndex()
	{
		if (_nodeToSave == null)
		{
			return PhoneSaveHandler.NoneIndex;
		}
		else
		{
			return _nodeToSave.graph.nodes.IndexOf(_nodeToSave);
		}
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
				_nodeToSave = _nextNode;
				await choicePhoneNode.Enter();
				_cancellationTokenSource = new CancellationTokenSource();
				await UniTask.WaitUntil(() => choicePhoneNode.IsOver == true, cancellationToken: _cancellationTokenSource.Token);
				_nodeToSave = _nextNode = choicePhoneNode.GetNextNode();
				_tryShowNextReactiveCommand.Execute();
				return choicePhoneNode.GetMessage();

			case PhoneSwitchNode phoneSwitchNode:
				_nodeToSave = _nextNode.GetPort(GameSeriesHandler.InputPortName).Connection.node;
				await phoneSwitchNode.Enter();
				_cancellationTokenSource = new CancellationTokenSource();
				await UniTask.WaitUntil(() => phoneSwitchNode.IsOver == true, cancellationToken: _cancellationTokenSource.Token);
				_nodeToSave = _nextNode = phoneSwitchNode.GetNextNode();
				_tryShowNextReactiveCommand.Execute();
				break;

			case PhoneNarrativeMessageNode phoneNarrativeMessageNode:
				_nodeToSave = _nextNode;
				if (phoneNarrativeMessageNode.IsEntered == false)
				{
					await phoneNarrativeMessageNode.Enter();
				}
				else
				{
					_nextNode = phoneNarrativeMessageNode.GetNextNode();
					_nodeToSave = _nextNode;
					await phoneNarrativeMessageNode.Exit();
					_tryShowNextReactiveCommand.Execute();
				}
				break;
			
			case PhoneMessageNode phoneMessageNode:
				_nodeToSave = _nextNode;
				_nextNode = phoneMessageNode.GetNextNode();
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
				return GetMessage(phoneMessageNode);
			
			case NotificationNode notificationNode:
				_nodeToSave = _nextNode.GetPort(GameSeriesHandler.InputPortName).Connection.node;
				notificationNode.Enter().Forget();
				_nodeToSave = _nextNode = notificationNode.GetNextNode();
				_tryShowNextReactiveCommand.Execute();
				break;
			
			case EndNode endNode:
				MessagesIsOut = true;
				break;
		}
		return default;
	}

	private PhoneMessage GetMessage(PhoneMessageNode phoneMessageNode)
	{
		return new PhoneMessage
		{
			TextMessage = phoneMessageNode.GetLocalizationString,
			MessageType = phoneMessageNode.Type,
			IsReaded = phoneMessageNode.IsReaded
		};
	}
}