using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

public class CustomizationNodeInitializer : MyNodeInitializer
{
    public CustomizationNodeInitializer(GameStatsHandler gameStatsHandler) : base(gameStatsHandler) { }

    public void InitCustomizationSettings(ref List<CustomizationSettings> settings, IReadOnlyList<MySprite> sprites, int skipFirstWordsInLabel = 2, int skipEndWordsInLabel = 0)
    {
        if (sprites == null)
        {
            return;
        }
        List<CustomizationSettings> newSettingsList = new List<CustomizationSettings>(sprites.Count);
        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i].SpriteToWardrobeKey == false)
            {
                continue;
            }
            newSettingsList.Add(GetNewCustomizationSettings(sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                i, sprites[i].Price, sprites[i].PriceAdditional));
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
            if (sprites[i].SpriteToWardrobeKey == false)
            {
                continue;
            }
            newName = sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_');
            if (dictionaryOldSettings.TryGetValue(newName, out CustomizationSettings customizationOldSetting) == true)
            {
                var customizationSetting = new CustomizationSettings(
                    GameStatsHandler.ReinitStats(customizationOldSetting.GameStats),
                    sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                    i, sprites[i].Price, sprites[i].PriceAdditional, customizationOldSetting.KeyAdd, customizationOldSetting.KeyShowParams, customizationOldSetting.KeyShowStats);
                newSettingsList.Add(customizationSetting);
                continue;
            }
            newSettingsList.Add(GetNewCustomizationSettings(sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                i, sprites[i].Price, sprites[i].PriceAdditional));
        }
        settings = newSettingsList;
    }

    public static SelectedCustomizationContentIndexes CreateCustomizationContent(
        IReadOnlyList<CustomizationSettings> settingsBodies,
        IReadOnlyList<CustomizationSettings> settingsHairstyles,
        IReadOnlyList<CustomizationSettings> settingsClothes,
        IReadOnlyList<CustomizationSettings> settingsSwimsuits,
        CustomizableCharacter customizableCharacter)
    {
        List<CustomizationSettings> spriteIndexesBodies = settingsBodies.Where(x=>x.KeyAdd == true).ToList();
        List<CustomizationSettings> spriteIndexesHairstyles = settingsHairstyles.Where(x=>x.KeyAdd == true).ToList();
        List<CustomizationSettings> spriteIndexesClothes = GetRenamedFieldsToView(settingsClothes, customizableCharacter.ClothesData);
        List<CustomizationSettings> spriteIndexesSwimsuits = GetRenamedFieldsToView(settingsSwimsuits, customizableCharacter.SwimsuitsData);
        return new SelectedCustomizationContentIndexes(
            spriteIndexesBodies, 
            spriteIndexesHairstyles,
            spriteIndexesClothes, 
            spriteIndexesSwimsuits,
            customizableCharacter);
    }

    private static List<CustomizationSettings> GetRenamedFieldsToView(
        IReadOnlyList<CustomizationSettings> customizationSettings, IReadOnlyList<MySprite> mySprites)
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
                    customizationSettings[i].PriceAdditional,
                    customizationSettings[i].KeyShowStats));
            }
        }

        return spriteIndexesClothes;
    }

    private CustomizationSettings GetNewCustomizationSettings(string name, int index, int price, int priceAdditional)
    {
        return new CustomizationSettings(GameStatsHandler.GetGameStatsForm(), name, index, price, priceAdditional);
    }
}