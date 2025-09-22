using System.Collections.Generic;
using UnityEngine;

public class PhoneProviderInEditMode : MonoBehaviour
{
    [SerializeField] private List<PhoneDataProvider> _dataProviders;
    [SerializeField] private List<PhoneContactsProvider> _contactsToSeriaProviders;

    private List<Phone> _phones;

    private PhoneCreator _phoneCreator;
    private PhoneContactCombiner _phoneContactCombiner;
    public IReadOnlyList<Phone> GetPhones(int seriaIndex)
    {
        List<Phone> phones = new List<Phone>();
        
        _phoneCreator.CreatePhones(seriaIndex);
        
        
        return phones;
    }
    public IReadOnlyList<PhoneContactData> GetContactsToSeria(int seriaIndex)
    {
        return _phoneContactCombiner.GetContactsToSeria(seriaIndex);
    }
    public void Construct()
    {
        _phoneCreator = new PhoneCreator(_dataProviders);
        _phoneContactCombiner = new PhoneContactCombiner(_contactsToSeriaProviders);
    }



    private void ContactCombiner()
    {
        
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
//
//
//
//
//