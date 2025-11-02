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
        var skinValues = GetPriceAndStats(ArrowSwitchMode.SkinColor);
        BodySwitchInfo = new SwitchInfo
        {
            Price  = skinValues.Item1,
            AdditionalPrice = skinValues.Item2,
            Stats = skinValues.Item3,
            Mode = ArrowSwitchMode.SkinColor
        };
        var hairstylesValues = GetPriceAndStats(ArrowSwitchMode.Hairstyle);
        HairstyleSwitchInfo = new SwitchInfo
        {
            Price  = hairstylesValues.Item1,
            AdditionalPrice = hairstylesValues.Item2,
            Stats = hairstylesValues.Item3,
            Mode = ArrowSwitchMode.Hairstyle
        };
        var clothesValues = GetPriceAndStats(ArrowSwitchMode.Clothes);
        ClothesSwitchInfo = new SwitchInfo
        {
            Price  = clothesValues.Item1,
            AdditionalPrice = clothesValues.Item2,
            Stats = clothesValues.Item3,
            Mode = ArrowSwitchMode.Clothes
        };
        CurrentSwitchInfo = new SwitchInfo
        {
            Stats = calculateStatsHandler.PreliminaryStats
        };
        _allInfo = new[] {BodySwitchInfo, HairstyleSwitchInfo, ClothesSwitchInfo};
        (int,int, List<CustomizationStat>) GetPriceAndStats(ArrowSwitchMode mode)
        {
            IReadOnlyList<ICustomizationSettings> indexes = selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int)mode];
            if (indexes.Count > 0)
            {
                return (indexes[0].Price, indexes[0].PriceAdditional, indexes[0].GameStats);
            }
            else
            {
                return (0,0, calculateStatsHandler.PreliminaryStats);
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
        CurrentSwitchInfo.Stats = _customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].GameStats;
    }
    public void SetPriceToCurrentSwitchInfo()
    {
        CurrentSwitchInfo.Price = _customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].Price;
    }
    public void SetAdditionalPriceToCurrentSwitchInfo()
    {
        CurrentSwitchInfo.AdditionalPrice = _customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].PriceAdditional;
    }
}