using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PhoneProviderInEditMode : MonoBehaviour, IPhoneProvider
{
    [SerializeField, Expandable] private List<PhoneDataProvider> _dataProviders;
    [SerializeField, Expandable] private List<PhoneContactsProvider> _contactsToSeriaProviders;

    private Dictionary<string, Phone> _phones1;
    private List<Phone> _phones2;

    private PhoneCreatorEditMode _phoneCreator;
    private PhoneContactCombiner _phoneContactCombiner;
    //в лайтайме в эдитор режиме телефоны поставляются  в каждую серию отдельные со своим контентом
    //в рантайме в эдитор режиме телефоны поставляются только в активную серию и их дата дополняется по мере перехода в след серии
    public IReadOnlyList<Phone> GetPhones(int currentSeriaIndex)
    {
        if (Application.isPlaying)
        {
            _phoneCreator.TryCreatePhones(_phones2, currentSeriaIndex);
            return _phones2;
        }
        else
        {
            
            return _phoneCreator.TryCreatePhonesForNonPlayMode(currentSeriaIndex);
        }
    }
    
    //придумать проверку если контакт был добавлен в телефон ранее то контакт будет дополнен датой текущей серии
    //а если контакт не был добавлен в телефон ранее а был добавлен в текущей серии то его надо добавить без контента прошлых серий
    //
    public IReadOnlyList<PhoneContactDataLocalizable> GetContactsToSeria(int seriaIndex)
    {
        List<PhoneContactDataLocalizable> contactDatas = new List<PhoneContactDataLocalizable>();
        for (int i = 0; i < _contactsToSeriaProviders.Count; i++)
        {
            if (_contactsToSeriaProviders[i].SeriaIndex < seriaIndex)
            {
                contactDatas.AddRange(_phoneContactCombiner.CreateNewPhoneContactData(_contactsToSeriaProviders[i].PhoneContactDatas, true));
                    
            }else if (_contactsToSeriaProviders[i].SeriaIndex == seriaIndex)
            {
                contactDatas.AddRange(_phoneContactCombiner.CreateNewPhoneContactData(_contactsToSeriaProviders[i].PhoneContactDatas, false));
            }
        }
        return contactDatas;
    }
    public void Construct()
    {
        _phoneContactCombiner = new PhoneContactCombiner();
        _phoneCreator = new PhoneCreatorEditMode(_dataProviders, _phoneContactCombiner);
        _phones1 = new Dictionary<string, Phone>();
        _phones2 = new List<Phone>();
    }
}
//глобальное хранилище контактов для каждой серии и у каждого персонажа с телефоном есть свои контакты
//
//при сохранении игры сохраняются ключи или индексы добавленных в телефон контактов
//у каждого добавленного в телефон контакта текущей серии сохраняются ключи "прочитано" прочитанные сохраняются 
//
//из глобального хранилища можно выбрать контакт который можно добавить в конкретный телефон по ходу сюжета

//телефон создается если в серии есть ноды телефона или есть контент для телефона 
//
//телефон включается нодой в которой можно выбрать экран
//при добавлении контакта срабатывает нода оповещения