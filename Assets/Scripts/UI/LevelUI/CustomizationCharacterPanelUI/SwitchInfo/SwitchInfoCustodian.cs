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
        var skinPrices = GetPrice((int) ArrowSwitchMode.SkinColor);
        BodySwitchInfo = new SwitchInfo
        {
            Price  = skinPrices.Item1,
            AdditionalPrice = skinPrices.Item2,
            Stats = calculateStatsHandler.PreliminaryStats,
            Mode = ArrowSwitchMode.SkinColor
        };
        var hairstylesPrices = GetPrice((int) ArrowSwitchMode.Hairstyle);
        HairstyleSwitchInfo = new SwitchInfo
        {
            Price  = hairstylesPrices.Item1,
            AdditionalPrice = hairstylesPrices.Item2,
            Stats = calculateStatsHandler.PreliminaryStats,
            Mode = ArrowSwitchMode.Hairstyle
        };
        var clothesPrices = GetPrice((int) ArrowSwitchMode.Clothes);
        ClothesSwitchInfo = new SwitchInfo
        {
            Price  = clothesPrices.Item1,
            AdditionalPrice = clothesPrices.Item2,
            Stats = calculateStatsHandler.PreliminaryStats,
            Mode = ArrowSwitchMode.Clothes
        };
        CurrentSwitchInfo = new SwitchInfo
        {
            Stats = calculateStatsHandler.PreliminaryStats
        };
    
        (int,int) GetPrice(int index)
        {
            IReadOnlyList<ICustomizationSettings> indexes = selectedCustomizationContentIndexes.IndexesSpriteIndexes[index];
            if (indexes.Count > 0)
            {
                return (indexes[0].Price, indexes[0].PriceAdditional);
            }
            else
            {
                return (0,0);
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
    public void SetAdditionalPriceToCurrentSwitchInfo()
    {
        CurrentSwitchInfo.AdditionalPrice = _customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].PriceAdditional;
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