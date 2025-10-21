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

    public bool LoadData(out SaveData outSaveData)
    {
        outSaveData = _saveMethod.Load<SaveData>(_savePath);
        if (outSaveData != null)
        {
            return true;
        }
        else
        {
            return false;
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
