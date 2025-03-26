using System.Collections.Generic;

public class SwitchInfoCustodian
{
    private readonly CustomizationSettingsCustodian _customizationSettingsCustodian;
    public SwitchInfo CurrentSwitchInfo { get; private set; }
    public SwitchInfo BodySwitchInfo { get; private set; }
    public SwitchInfo HairstyleSwitchInfo { get; private set; }
    public SwitchInfo ClothesSwitchInfo { get; private set; }
    public int CurrentSwitchIndex => CurrentSwitchInfo.Index;

    public SwitchInfoCustodian(SelectedCustomizationContentIndexes selectedCustomizationContentIndexes, CustomizationSettingsCustodian customizationSettingsCustodian,
        CalculateStatsHandler calculateStatsHandler)
    {
        _customizationSettingsCustodian = customizationSettingsCustodian;
        BodySwitchInfo = new SwitchInfo
        {
            Price  = GetPrice((int)ArrowSwitchMode.SkinColor),
            Stats = calculateStatsHandler.PreliminaryStats
        };
        HairstyleSwitchInfo = new SwitchInfo
        {
            Price  = GetPrice((int)ArrowSwitchMode.Hairstyle),
            Stats = calculateStatsHandler.PreliminaryStats
        };
        ClothesSwitchInfo = new SwitchInfo
        {
            Price  = GetPrice((int)ArrowSwitchMode.Clothes),
            Stats = calculateStatsHandler.PreliminaryStats
        };
        CurrentSwitchInfo = new SwitchInfo
        {
            Stats = calculateStatsHandler.PreliminaryStats
        };
    
        int GetPrice(int index)
        {
            IReadOnlyList<ICustomizationSettings> indexes = selectedCustomizationContentIndexes.IndexesSpriteIndexes[index];
            if (indexes.Count > 0)
            {
                return indexes[0].Price;
            }
            else
            {
                return 0;
            }
        }
    }

    public SwitchInfo[] GetAllInfo()
    {
        return new[] {BodySwitchInfo, HairstyleSwitchInfo, ClothesSwitchInfo};
    }

    public void SetToCurrentInfo(SwitchInfo info)
    {
        CurrentSwitchInfo = info;
    }
    public void SetToBodyInfoCurrentSwitchInfo()
    {
        BodySwitchInfo = CurrentSwitchInfo;
    }
    public void SetToHairstyleInfoCurrentSwitchInfo()
    {
        HairstyleSwitchInfo = CurrentSwitchInfo;
    }
    public void SetToClothesInfoCurrentSwitchInfo()
    {
        ClothesSwitchInfo = CurrentSwitchInfo;
    }

    public void SetStatsToCurrentSwitchInfo()
    {
        CurrentSwitchInfo.Stats = GetBaseStats(_customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].GameStats);
    }
    public void SetPriceToCurrentSwitchInfo()
    {
        CurrentSwitchInfo.Price = _customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].Price;
    }
    private List<BaseStat> GetBaseStats(List<Stat> stats)
    {
        List<BaseStat> baseStats = new List<BaseStat>(stats.Count);
        for (int i = 0; i < stats.Count; i++)
        {
            baseStats.Add(new BaseStat(stats[i].Name, stats[i].Value));
        }

        return baseStats;
    }
}