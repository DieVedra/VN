using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "PhoneNodeGraph", menuName = "NodeGraphs/PhoneNodeGraph", order = 51)]
public class PhoneMessagesGraph : NodeGraph, ILocalizable
{
	private Node _nextNode;
	public bool MessagesIsOut { get; private set; }
	public void InitPhoneMessagesGraph()
	{
		MessagesIsOut = false;
		for (int i = 0; i < nodes.Count; i++)
		{
			if (nodes[i] is StartNode startNode)
			{
				_nextNode = startNode.OutputPortBaseNode.Connection.node;
				break;
			}
		}
	}

	private void Dispose()
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			if (nodes[i] is BaseNode baseNode)
			{
				baseNode.Dispose();
			}
		}
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
	public IReadOnlyList<LocalizationString> GetLocalizableContent()
	{
		List<LocalizationString> strings = new List<LocalizationString>(nodes.Count);
		for (int i = 0; i < nodes.Count; i++)
		{
			if (nodes[i] is ILocalizable localizable)
			{
				strings.AddRange(localizable.GetLocalizableContent());
			}
		}
		return strings;
	}

	private PhoneMessageLocalization GetPhoneMessage(PhoneMessageNode phoneMessageNode)
	{
		return new PhoneMessageLocalization(phoneMessageNode.GetLocalizationString, phoneMessageNode.Type);
	}
}