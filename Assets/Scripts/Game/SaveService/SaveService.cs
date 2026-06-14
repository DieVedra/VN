using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

public class SaveService
{
    private ISaveMethod _saveMethod;
    public SaveService(ISaveMethod saveMethod)
    {
        _saveMethod = saveMethod;
    }
    public async UniTask Construct()
    {
        await _saveMethod.Construct();
    }
    public async UniTask<SaveData> LoadData()
    {
        return await _saveMethod.Load<SaveData>();
    }

    public async UniTask Save(SaveData saveData)
    {
        await _saveMethod.Save(saveData);
    }

    public static WardrobeSaveData[] CreateWardrobeSaveDatas(IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians)
    {
        return customizableCharacterIndexesCustodians.Select(x => x.Value.GetWardrobeSaveData()).ToArray();
    }
}
