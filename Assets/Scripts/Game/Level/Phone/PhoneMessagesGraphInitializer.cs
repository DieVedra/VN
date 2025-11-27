using System.Collections.Generic;
using UnityEngine;
using XNode;

public class PhoneMessagesGraphInitializer
{
    private readonly IGameStatsProvider _gameStatsProvider;
    private readonly LevelUIProviderEditMode _levelUIProvider;
    private readonly SetLocalizationChangeEvent _setLocalizationChangeEvent;

    public PhoneMessagesGraphInitializer(IGameStatsProvider gameStatsProvider, LevelUIProviderEditMode levelUIProvider,
        SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        _gameStatsProvider = gameStatsProvider;
        _levelUIProvider = levelUIProvider;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
    }
    // public void InitMessagesGraphs(IReadOnlyList<PhoneDataProvider> dataProviders, IReadOnlyList<PhoneContactsProvider> contactsToSeriaProviders)
    // {
    //     Debug.Log($"   InitMessagesGraphs");
    //
    //     int seriaIndex;
    //     PhoneContact phoneContact;
    //     for (int i = 0; i < dataProviders.Count; i++)
    //     {
    //         // PhoneDataProvider phoneDataProvider = dataProviders[i];
    //         // seriaIndex = phoneDataProvider.SeriaIndex;
    //         // for (int j = 0; j < phoneDataProvider.PhoneDatas.Count; j++)
    //         // {
    //         //     PhoneData phoneData = phoneDataProvider.PhoneDatas[j];
    //         //     for (int k = 0; k < phoneData.PhoneContactDatas.Count; k++)
    //         //     {
    //         //         phoneContact = phoneDataProvider.PhoneDatas[j].PhoneContactDatas[k];
    //         //         // if (phoneContact.PhoneMessagesGraph != null)
    //         //         // {
    //         //         //     for (int l = 0; l < phoneContact.PhoneMessagesGraph.nodes.Count; l++)
    //         //         //     {
    //         //         //         TryInitPhoneMessagesNode(phoneContact.PhoneMessagesGraph.nodes[l], seriaIndex);
    //         //         //     }
    //         //         // }
    //         //     }
    //         // }
    //     }
    //
    //     for (int i = 0; i < contactsToSeriaProviders.Count; i++)
    //     {
    //         for (int j = 0; j < contactsToSeriaProviders[i].PhoneContacts.Count; j++)
    //         {
    //             phoneContact = contactsToSeriaProviders[i].PhoneContacts[j];
    //             // if (phoneContact.PhoneMessagesGraph != null)
    //             // {
    //             //     seriaIndex = contactsToSeriaProviders[i].SeriaIndex;
    //             //     for (int l = 0; l < phoneContact.PhoneMessagesGraph.nodes.Count; l++)
    //             //     {
    //             //         TryInitPhoneMessagesNode(phoneContact.PhoneMessagesGraph.nodes[l], seriaIndex);
    //             //     }
    //             // }
    //         }
    //
    //     }
    // }
    public void TryInitPhoneMessagesNode(Node node, int seriaIndex)
    {
        Debug.Log($"   node {node}");

        if (node is BaseNode baseNode)
        {
            baseNode.ConstructBaseNode(null, null, null,
                _setLocalizationChangeEvent);
        }

        switch (node)
        {
            case ChoicePhoneNode choicePhoneNode:
                choicePhoneNode.ConstructMyChoicePhoneNode(_gameStatsProvider, _levelUIProvider.ChoicePanelUIHandler,
                    _levelUIProvider.NotificationPanelUIHandler, _levelUIProvider.CustomizationCurtainUIHandler, seriaIndex);
                return;
            
            case PhoneSwitchNode phoneSwitchNode:
                phoneSwitchNode.ConstructMyPhoneSwitchNode(_gameStatsProvider, seriaIndex);
                return;
            
            case PhoneNarrativeMessageNode phoneNarrativeMessageNode:
                phoneNarrativeMessageNode.ConstructMyPhoneNarrativeNode(_levelUIProvider.NarrativePanelUIHandler, _levelUIProvider.CustomizationCurtainUIHandler);
                return;
        }
    }
}