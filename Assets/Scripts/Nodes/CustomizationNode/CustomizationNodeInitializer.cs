using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        BaseInit(ref settings, sprites, false, skipFirstWordsInLabel, skipEndWordsInLabel);
    }
    
    public void ReInitCustomizationSettings(ref List<CustomizationSettings> settings, IReadOnlyList<MySprite> sprites, int skipFirstWordsInLabel = 2, int skipEndWordsInLabel = 0)
    {
        if (settings != null && settings.Count > 0)
        {
            BaseInit(ref settings, sprites, true, skipFirstWordsInLabel, skipEndWordsInLabel);
        }
        else
        {
            BaseInit(ref settings, sprites, false, skipFirstWordsInLabel, skipEndWordsInLabel);
        }
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

    private void BaseInit(ref List<CustomizationSettings> settings, IReadOnlyList<MySprite> sprites, bool keyReinit, int skipFirstWordsInLabel = 2, int skipEndWordsInLabel = 0)
    {
        if (sprites == null)
        {
            return;
        }

        if (settings == null)
        {
            settings = new List<CustomizationSettings>();
        }
        List<CustomizationSettings> newSettings = new List<CustomizationSettings>();
        List<Stat> gameStats = null;
        if (keyReinit == false)
        {
            gameStats = _gameStatsHandler.GetGameStatsForm();
        }
        CustomizationSettings customizationSetting;
        string name;
        for (int i = 0; i < sprites.Count; i++)
        {
            if (CheckListElementContents(settings, i))
            {
                Debug.Log($"1");

                for (int j = 0; j < sprites.Count; j++)
                {
                    Debug.Log($"90  {settings[i].Name}    {sprites[j].Name}");
                    name = sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_');
                    if (settings[i].Name == name)
                    {
                        Debug.Log($"1000");

                        customizationSetting = new CustomizationSettings(
                            keyReinit == false ? gameStats : _gameStatsHandler.ReinitStats(settings[i].GameStats),
                            name,
                            i, sprites[i].Price, settings[i].KeyAdd, settings[i].KeyShowParams, settings[i].KeyShowStats);
                        newSettings.Insert(i, customizationSetting);
                        Debug.Log($"newSettings0 {newSettings.Count}");

                        break;
                    }
                }
                Debug.Log($"newSettings1 {newSettings.Count}");
                continue;
            }
            Debug.Log($"2");

            customizationSetting = new CustomizationSettings(
                gameStats,
                sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                i, sprites[i].Price);
            newSettings.Insert(i, customizationSetting);
        }

        Debug.Log($"newSettings2 {newSettings.Count}");
        settings = newSettings;
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

    private bool CheckListElementContents(List<CustomizationSettings> settings, int index)
    {
        if (settings.Count > index && settings[index] != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}