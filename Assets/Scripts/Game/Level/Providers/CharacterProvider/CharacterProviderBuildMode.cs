using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class CharacterProviderBuildMode : ILocalizable
{
    private const string _charactersDataProviderName = "CharactersDataProviderSeria";
    private const string _charactersProviderName = "CharactersProviderSeria";

    private readonly CharactersCreator _charactersCreator;
    private readonly DataProvider<CharactersProvider> _charactersProvider;
    private readonly DataProvider<CharactersDataProvider> _charactersDataProvider;

    private Dictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;

    public IParticipiteInLoad CharactersDataProviderParticipiteInLoad => _charactersDataProvider;
    public IParticipiteInLoad CharactersProviderParticipiteInLoad => _charactersProvider;
    public ICharacterProvider CharacterProvider => _charactersCreator;

    public IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> CustomizableCharacterIndexesCustodians => _customizableCharacterIndexesCustodians;


    public CharacterProviderBuildMode()
    {
        _charactersProvider = new DataProvider<CharactersProvider>();
        _charactersDataProvider = new DataProvider<CharactersDataProvider>();
        _customizableCharacterIndexesCustodians = new Dictionary<string, CustomizableCharacterIndexesCustodian>();
        _charactersCreator = new CharactersCreator(
            _charactersDataProvider.GetDatas, _charactersProvider.GetDatas,
            _customizableCharacterIndexesCustodians);
    }

    public async UniTask Construct()
    {
        await UniTask.WhenAll(_charactersProvider.CreateNames(_charactersProviderName),
            _charactersDataProvider.CreateNames(_charactersDataProviderName));
    }
    public void Shutdown()
    {
        _charactersDataProvider.Shutdown();
        _charactersProvider.Shutdown();
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> strings = new List<LocalizationString>();
        int count;
        IReadOnlyList<CharactersProvider> datas = _charactersProvider.GetDatas;
        for (int i = 0; i < datas.Count; i++)
        {
            count = datas[i].CharactersInfo.Count;
            for (int j = 0; j < count; j++)
            {
                strings.Add(datas[i].CharactersInfo[j].LocalizationString);
            }
        }
        return strings;
    }

    public void CheckMatchNumbersSeriaWithNumberAssets(int nextSeriaNumber, int nextSeriaNameAssetIndex)
    {
        _charactersDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        _charactersProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
    }

    public async UniTask TryLoadDatas(int nextSeriaNameAssetIndex)
    {
        await _charactersProvider.TryLoadData(nextSeriaNameAssetIndex);
        await _charactersDataProvider.TryLoadData(nextSeriaNameAssetIndex);
    }
}