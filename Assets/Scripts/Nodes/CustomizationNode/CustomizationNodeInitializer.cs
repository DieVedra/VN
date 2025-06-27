using System.Collections.Generic;
using System.Linq;

public class CustomizationNodeInitializer
{
    private GameStatsHandler _gameStatsHandler;
    public GameStatsHandler GameStatsHandler => _gameStatsHandler;

    public CustomizationNodeInitializer(List<Stat> stats)
    {
        _gameStatsHandler = new GameStatsHandler(stats);
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
    
    public void ReInitCustomizationSettings(ref List<CustomizationSettings> settings, IReadOnlyList<MySprite> sprites, int skipFirstWordsInLabel = 2, int skipEndWordsInLabel = 0)
    {
        if (sprites == null)
        {
            return;
        }
        if (settings == null || settings.Count == 0)
        {
            InitCustomizationSettings(ref settings, sprites, skipFirstWordsInLabel, skipEndWordsInLabel);
        }
        Dictionary<string, CustomizationSettings> dictionaryOldSettings = settings.ToDictionaryDistinct(setting => setting.Name);
        List<CustomizationSettings> newSettingsList = new List<CustomizationSettings>(sprites.Count);
        string newName;
        for (int i = 0; i < sprites.Count; i++)
        {
            newName = sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_');
            if (dictionaryOldSettings.TryGetValue(newName, out CustomizationSettings customizationOldSetting) == true)
            {
                var customizationSetting = new CustomizationSettings(
                    _gameStatsHandler.ReinitStats(customizationOldSetting.GameStats),
                    sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                    i, sprites[i].Price, customizationOldSetting.KeyAdd, customizationOldSetting.KeyShowParams, customizationOldSetting.KeyShowStats);
                newSettingsList.Add(customizationSetting);
                continue;
            }
            newSettingsList.Add(GetNewCustomizationSettings(sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                i, sprites[i].Price));
        }
        settings = newSettingsList;
    }

    public SelectedCustomizationContentIndexes CreateCustomizationContent(
        IReadOnlyList<CustomizationSettings> settingsBodies,
        IReadOnlyList<CustomizationSettings> settingsHairstyles,
        IReadOnlyList<CustomizationSettings> settingsClothes,
        IReadOnlyList<CustomizationSettings> settingsSwimsuits,
        CustomizableCharacter customizableCharacter)
    {
        return new SelectedCustomizationContentIndexes(settingsBodies.Where(n => n.KeyAdd == true).ToList(), 
            settingsHairstyles.Where(n => n.KeyAdd == true).ToList(),
            GetRenamedFieldsToView(settingsClothes, customizableCharacter.ClothesData), 
            GetRenamedFieldsToView(settingsSwimsuits, customizableCharacter.SwimsuitsData),
            customizableCharacter);
    }
    private List<CustomizationSettings> GetRenamedFieldsToView(IReadOnlyList<CustomizationSettings> customizationSettings, IReadOnlyList<MySprite> mySprites)
    {
        List<CustomizationSettings> spriteIndexesClothes = new List<CustomizationSettings>();
        for (int i = 0; i < customizationSettings.Count; i++)
        {
            if (customizationSettings[i].KeyAdd == true)
            {
                spriteIndexesClothes.Add(new CustomizationSettings(
                    customizationSettings[i].GameStats.ToList(),
                    mySprites[i].Name.MyCutString(2, separator: '_'),
                    customizationSettings[i].Index,
                    customizationSettings[i].Price,
                    customizationSettings[i].KeyShowStats));
            }
        }
        return spriteIndexesClothes;
    }
    private CustomizationSettings GetNewCustomizationSettings(string name, int index, int price)
    {
        return new CustomizationSettings(_gameStatsHandler.GetGameStatsForm(), name, index, price);
    }
}