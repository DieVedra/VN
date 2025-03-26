

using System.Collections.Generic;
using System.Linq;

public class CustomizationNodeInitializer
{
    private readonly GameStatsCustodian _gameStatsCustodian;

    public CustomizationNodeInitializer(GameStatsCustodian gameStatsCustodian)
    {
        _gameStatsCustodian = gameStatsCustodian;
    }

    public List<CustomizationSettings> InitCustomizationSettings(List<MySprite> sprites, int skipFirstWordsInLabel = 2, int skipEndWordsInLabel = 0)
    {
        List<CustomizationSettings> settings = new List<CustomizationSettings>();
        for (int i = 0; i < sprites.Count; i++)
        {
            settings.Add(new CustomizationSettings(
                _gameStatsCustodian.GetGameStatsForm(),
                sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                i));
        }
        return settings;
    }
    
    public List<CustomizationSettings> ReInitCustomizationSettings(ref List<CustomizationSettings> settings, List<MySprite> sprites, int skipFirstWordsInLabel = 2, int skipEndWordsInLabel = 0)
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
                    // CustomizationSettings customizationSettings = new CustomizationSettings(
                    //     _gameStatsCustodian.GetGameStatsForm(),
                    //     sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                    //     i);
                    newSettings.Add(new CustomizationSettings(
                        _gameStatsCustodian.GetGameStatsForm(),
                        sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
                        i));
                }
                else
                {
                    newSettings.Add(new CustomizationSettings(
                        _gameStatsCustodian.GetGameStatsForm(),
                        sprites[i].Name.MyCutString(skipFirstWordsInLabel, skipEndWordsInLabel, '_'),
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
            GetRenamedFieldsToView(settingsClothes, customizableCharacter.ClothesData.MySprites), 
            GetRenamedFieldsToView(settingsSwimsuits, customizableCharacter.SwimsuitsData.MySprites),
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
}