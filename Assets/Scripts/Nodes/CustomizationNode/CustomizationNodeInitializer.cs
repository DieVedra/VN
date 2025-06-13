using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomizationNodeInitializer
{
    private readonly GameStatsCustodian _gameStatsCustodian;

    public CustomizationNodeInitializer(GameStatsCustodian gameStatsCustodian)
    {
        _gameStatsCustodian = gameStatsCustodian;
    }
    public void InitCustomizationSettings1(ref List<CustomizationSettings> settings, IReadOnlyList<MySprite> sprites, int skipFirstWordsInLabel = 2, int skipEndWordsInLabel = 0)
    {
        if (sprites == null)
        {
            return;
        }
        List<CustomizationSettings> newSettings = new List<CustomizationSettings>();
        CustomizationSettings customizationSetting;
        for (int i = 0; i < sprites.Count; i++)
        {
            if (settings[i] != null && settings[i].Name == sprites[i].Name)
            {
                customizationSetting = new CustomizationSettings(
                    _gameStatsCustodian.GetGameStatsForm(),
                    sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                    i, sprites[i].Price, settings[i].KeyAdd, settings[i].KeyShowParams, settings[i].KeyShowStats);
                continue;
            }
            customizationSetting = new CustomizationSettings(
                _gameStatsCustodian.GetGameStatsForm(),
                sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                i, sprites[i].Price);
            newSettings.Insert(i, customizationSetting);
        }

        settings = newSettings;
    }
    
    public List<CustomizationSettings> ReInitCustomizationSettings(ref List<CustomizationSettings> settings, IReadOnlyList<MySprite> sprites, int skipFirstWordsInLabel = 2, int skipEndWordsInLabel = 0)
    {
        List<CustomizationSettings> newSettings = new List<CustomizationSettings>();
        if (settings.Count > sprites.Count)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                newSettings.Add(new CustomizationSettings(
                    TryReInitGameStats(settings[i].GameStats),
                    settings[i].Name, settings[i].Index, settings[i].Price, settings[i].KeyAdd, settings[i].KeyShowParams, settings[i].KeyShowStats));
            }
            return newSettings;
        }
        else if (settings.Count < sprites.Count)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                if (settings.Count - 1 <= i)
                {
                    newSettings.Add(new CustomizationSettings(
                        _gameStatsCustodian.GetGameStatsForm(),
                        sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                        sprites[i].Price,
                        i));
                }
                else
                {
                    newSettings.Add(new CustomizationSettings(
                        _gameStatsCustodian.GetGameStatsForm(),
                        sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                        sprites[i].Price,
                        i));
                }
            }
            return newSettings;
        }
        else
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                newSettings.Add(new CustomizationSettings(
                    TryReInitGameStats(settings[i].GameStats),
                    sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                    i, settings[i].Price, settings[i].KeyAdd, settings[i].KeyShowParams, settings[i].KeyShowStats));
            }
            return newSettings;
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

    private List<Stat> TryReInitGameStats(List<Stat> oldStats)
    {
        List<Stat> newStats = _gameStatsCustodian.GetGameStatsForm();
        if (oldStats.Count > newStats.Count)
        {
            for (int i = 0; i < newStats.Count; i++)
            {
                ReInitStat(ref newStats, i, oldStats[i]);
            }
        }
        else if (oldStats.Count < newStats.Count)
        {
            for (int i = 0; i < oldStats.Count; i++)
            {
                ReInitStat(ref newStats, i, oldStats[i]);
            }
        }
        return newStats;
    }

    private void ReInitStat(ref List<Stat> newStats, int index, Stat oldStat)
    {
        if (oldStat.Name == newStats[index].Name)
        {
            newStats[index] = new Stat(oldStat.Name, oldStat.Value, oldStat.ShowKey, oldStat.ColorField);
        }
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

    public LocalizationString[] CreateLocalizationArray(params List<CustomizationSettings>[] lists)
    {
        List<LocalizationString> strings = new List<LocalizationString>();
        for (int i = 0; i < lists.Length; i++)
        {
            if (lists[i] != null)
            {
                for (int j = 0; j < lists[i].Count; j++)
                {
                    strings.Add(lists[i][j].LocalizationName);
                }
            }
        }
        return strings.ToArray();
    }
}