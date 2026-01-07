using System.Collections.Generic;
using System.Linq;

public class CustomizationNodeInitializer : MyNodeInitializer
{
    public CustomizationNodeInitializer(GameStatsHandler gameStatsHandlerNodeInitializer) : base(gameStatsHandlerNodeInitializer) { }

    public void InitCustomizationSettings(List<CustomizationSettings> settings, IReadOnlyList<MySprite> sprites)
    {
        if (sprites == null)
        {
            return;
        }
        settings.Clear();
        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i].SpriteToWardrobeKey == false)
            {
                continue;
            }
            settings.Add(GetNewCustomizationSettings(sprites[i].Name, sprites[i].Name, i, sprites[i].Price, sprites[i].PriceAdditional));
        }
    }
    
    public void ReInitCustomizationSettings(List<CustomizationSettings> settings, IReadOnlyList<MySprite> sprites)
    {
        if (sprites == null)
        {
            return;
        }
        if (settings.Count == 0)
        {
            InitCustomizationSettings(settings, sprites);
        }
        Dictionary<string, CustomizationSettings> dictionaryOldSettings = settings.ToDictionaryDistinct(setting => setting.SpriteName);
        settings.Clear();
        string newName;
        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i].SpriteToWardrobeKey == false)
            {
                continue;
            }
            newName = sprites[i].Name;
            if (dictionaryOldSettings.TryGetValue(newName, out CustomizationSettings customizationOldSetting) == true)
            {
                var customizationSetting = new CustomizationSettings(
                    GameStatsHandlerNodeInitializer.ReinitCustomizationStats(customizationOldSetting.GameStats),
                    sprites[i].Name, customizationOldSetting.Name,i, sprites[i].Price, sprites[i].PriceAdditional, customizationOldSetting.KeyAdd,
                    customizationOldSetting.KeyShowParams, customizationOldSetting.KeyShowStats);
                settings.Add(customizationSetting);
                continue;
            }
            settings.Add(GetNewCustomizationSettings(sprites[i].Name, sprites[i].Name,
                i, sprites[i].Price, sprites[i].PriceAdditional));
        }
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
        List<CustomizationSettings> spriteIndexesClothes = settingsClothes.Where(x=>x.KeyAdd == true).ToList();
        List<CustomizationSettings> spriteIndexesSwimsuits = settingsSwimsuits.Where(x=>x.KeyAdd == true).ToList();
        return new SelectedCustomizationContentIndexes(
            spriteIndexesBodies, 
            spriteIndexesHairstyles,
            spriteIndexesClothes, 
            spriteIndexesSwimsuits,
            customizableCharacter);
    }

    private CustomizationSettings GetNewCustomizationSettings(string spriteName, string localizationNameToGame, int index, int price, int priceAdditional)
    {
        return new CustomizationSettings(GameStatsHandlerNodeInitializer.GetCustomizationStatsForm(), spriteName, localizationNameToGame, index, price, priceAdditional);
    }
}