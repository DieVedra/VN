
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private List<CustomizationSettings> _customizationSettingses;
    [SerializeField] private WardrobeSeriaData _wardrobeSeriaData;
    [SerializeField] private SeriaStatProvider _seriaStatsProvider;
    private GameStatsHandler _gameStatsHandler;
    private IReadOnlyList<MySprite> _sprites;

    [Button()]
    private void Init()
    {
        _sprites = _wardrobeSeriaData.GetBodiesSprites();
        _gameStatsHandler = new GameStatsHandler(_seriaStatsProvider.Stats.ToList());
    }

    [Button()]
    public void initsett()
    {
        InitCustomizationSettings(ref _customizationSettingses, _sprites, 1,1);

    }
    [Button()]
    public void reinitsettt()
    {
        // IReadOnlyList<MySprite> sprites = _wardrobeSeriaData.GetBodiesSprites();
        // _gameStatsHandler = new GameStatsHandler(_seriaStatsProvider.Stats.ToList());
        
        InitCustomizationSettings(ref _customizationSettingses, _sprites, 1,1);


        Dictionary<string, CustomizationSettings> dictionaryOldSettings = _customizationSettingses.ToDictionaryDistinct(setting => setting.Name);
        List<CustomizationSettings> newSettingsList = new List<CustomizationSettings>(_sprites.Count);
        string newName;
        for (int i = 0; i < _sprites.Count; i++)
        {
            Debug.Log(2020);

            newName = _sprites[i].Name.MyCutString(1,1, '_');

            if (dictionaryOldSettings.TryGetValue(newName, out CustomizationSettings customizationOldSetting) == true)
            {
                var customizationSetting = new CustomizationSettings(
                    _gameStatsHandler.ReinitStats(customizationOldSetting.GameStats),
                    _sprites[i].Name.MyCutString(1, 1, '_'),
                    i, _sprites[i].Price, customizationOldSetting.KeyAdd, customizationOldSetting.KeyShowParams, customizationOldSetting.KeyShowStats);
                
                newSettingsList.Add(customizationSetting);
                continue;
            }
            newSettingsList.Add(GetNewCustomizationSettings(_sprites[i].Name.MyCutString(1, 1, '_'),
                i, _sprites[i].Price));
        }

        foreach (var VARIABLE in newSettingsList)
        {
            Debug.Log($"{VARIABLE.Name}");
        }
        // foreach (var VARIABLE in dictionaryOldSettings)
        // {
        //     Debug.Log($"{VARIABLE.Value.Name}");
        // }
    }
    private CustomizationSettings GetNewCustomizationSettings(string name, int index, int price)
    {
        return new CustomizationSettings(_gameStatsHandler.GetGameStatsForm(), name, index, price);
    }
    public void InitCustomizationSettings(ref List<CustomizationSettings> settings, IReadOnlyList<MySprite> sprites, int skipFirstWordsInLabel = 2, int skipEndWordsInLabel = 0)
    {
        if (sprites == null)
        {
            return;
        }
        List<CustomizationSettings> newSettingsList = new List<CustomizationSettings>(sprites.Count);
        for (int i = 0; i < sprites.Count; i++)
        {
            newSettingsList.Add(GetNewCustomizationSettings(sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                i, sprites[i].Price));
        }
        settings = newSettingsList;
    }
}