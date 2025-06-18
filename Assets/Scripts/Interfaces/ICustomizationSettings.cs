using System.Collections.Generic;

public interface ICustomizationSettings : ILocalizationString
{
    public string Name { get; }
    public int Index { get; }
    public int Price { get; }

    public List<Stat> GameStats { get; }
    public IReadOnlyList<ILocalizationString> GameStatsLocalizationStrings { get; }
}