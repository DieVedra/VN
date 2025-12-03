using Cysharp.Threading.Tasks;
using XNode;

public class PhoneMessagesExtractor
{
	private Node _nextNode;
	public PhoneMessagesExtractor()
	{
		
	}

    public bool MessagesIsOut { get; private set; }
    
	public void Init(ContactNodeCase contactNodeCase)
	{
		MessagesIsOut = false;
		_nextNode = contactNodeCase.Port.Connection.node;
	}
	public async UniTask<PhoneMessageLocalization> GetMessageText()
	{
		switch (_nextNode)
		{
			case ChoicePhoneNode choicePhoneNode:
				await choicePhoneNode.Enter();
				await UniTask.WaitUntil(() => choicePhoneNode.IsOver == true);
				_nextNode = choicePhoneNode.PhoneMessageNode.GetNextNode();
				return GetPhoneMessage(choicePhoneNode.PhoneMessageNode);

			case PhoneSwitchNode phoneSwitchNode:
				await phoneSwitchNode.Enter();
				await UniTask.WaitUntil(() => phoneSwitchNode.IsOver == true);
				_nextNode = phoneSwitchNode.PhoneMessageNode.GetNextNode();
				return GetPhoneMessage(phoneSwitchNode.PhoneMessageNode);

			case PhoneNarrativeMessageNode phoneNarrativeMessageNode:
				if (phoneNarrativeMessageNode.IsEntered == false)
				{
					await phoneNarrativeMessageNode.Enter();
				}
				else
				{
					await phoneNarrativeMessageNode.Exit();
					_nextNode = phoneNarrativeMessageNode.PhoneMessageNode.GetNextNode();
					return GetPhoneMessage(phoneNarrativeMessageNode.PhoneMessageNode);
				}
				break;
			
			case PhoneMessageNode phoneMessageNode:
				_nextNode = phoneMessageNode.GetNextNode();
				return GetPhoneMessage(phoneMessageNode);
			
			case EndNode endNode:
				MessagesIsOut = true;
				break;
		}
		return default;
	}

	private PhoneMessageLocalization GetPhoneMessage(PhoneMessageNode phoneMessageNode)
	{
		return new PhoneMessageLocalization(phoneMessageNode.GetLocalizationString, phoneMessageNode.Type);
	}
}