using System.Collections.Generic;

public class SwitchInfoCustodian
{
    private readonly CustomizationSettingsCustodian _customizationSettingsCustodian;
    private readonly SwitchInfo[] _allInfo;
    public SwitchInfo CurrentSwitchInfo { get; private set; }
    public SwitchInfo BodySwitchInfo { get; private set; }
    public SwitchInfo HairstyleSwitchInfo { get; private set; }
    public SwitchInfo ClothesSwitchInfo { get; private set; }

    public IReadOnlyList<SwitchInfo> GetAllInfo => _allInfo;
    public (int price, int additionalPrice)[] GetAllPriceInfo => new[]
    {
        (BodySwitchInfo.Price, BodySwitchInfo.AdditionalPrice),
        (HairstyleSwitchInfo.Price, HairstyleSwitchInfo.AdditionalPrice),
        (ClothesSwitchInfo.Price, ClothesSwitchInfo.AdditionalPrice)
    };
    public int CurrentSwitchIndex => CurrentSwitchInfo.Index;

    public SwitchInfoCustodian(SelectedCustomizationContentIndexes selectedCustomizationContentIndexes, CustomizationSettingsCustodian customizationSettingsCustodian,
        CalculateStatsHandler calculateStatsHandler)
    {
        _customizationSettingsCustodian = customizationSettingsCustodian;
        var skinPrices = GetPrice(ArrowSwitchMode.SkinColor);
        BodySwitchInfo = new SwitchInfo
        {
            Price  = skinPrices.Item1,
            AdditionalPrice = skinPrices.Item2,
            Stats = calculateStatsHandler.PreliminaryStats,
            Mode = ArrowSwitchMode.SkinColor
        };
        var hairstylesPrices = GetPrice(ArrowSwitchMode.Hairstyle);
        HairstyleSwitchInfo = new SwitchInfo
        {
            Price  = hairstylesPrices.Item1,
            AdditionalPrice = hairstylesPrices.Item2,
            Stats = calculateStatsHandler.PreliminaryStats,
            Mode = ArrowSwitchMode.Hairstyle
        };
        var clothesPrices = GetPrice(ArrowSwitchMode.Clothes);
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
        _allInfo = new[] {BodySwitchInfo, HairstyleSwitchInfo, ClothesSwitchInfo};
        (int,int) GetPrice(ArrowSwitchMode mode)
        {
            IReadOnlyList<ICustomizationSettings> indexes = selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int)mode];
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