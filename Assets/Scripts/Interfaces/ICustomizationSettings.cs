using System.Collections.Generic;

public interface ICustomizationSettings
{
    public string Name { get; }
    public int Index { get; }
    public int Price { get; }

    public List<Stat> GameStats { get; }
}