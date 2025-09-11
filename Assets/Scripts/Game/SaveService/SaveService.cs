using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveService
{
    private const string _fileName = "/Save";
    private readonly string _savePath;
    private ISaveMetod _saveMethod;
    public SaveService(ISaveMetod saveMethod)
    {
        _saveMethod = saveMethod;
        
        _savePath = Path.Combine(Application.dataPath + _fileName + _saveMethod.FileFormat);


        // _savePath = Path.Combine(Application.persistentDataPath, _fileName + _saveMethod.FileFormat);
        
    }

    public (bool, SaveData) LoadData()
    {
        if (_saveMethod.Load(_savePath) is SaveData saveData)
        {
            return (true, saveData);
        }
        else
        {
            return (false, null);
        }
    }

    public void Save(SaveData saveData)
    {
        _saveMethod.Save(_savePath, saveData);
    }

    public static WardrobeSaveData[] CreateWardrobeSaveDatas(IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians)
    {
        return customizableCharacterIndexesCustodians.Select(x => x.Value.GetWardrobeSaveData()).ToArray();
    }
}
