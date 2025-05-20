
using System.Collections.Generic;
using UniRx;

public interface ILocalizationChanger
{
    public IReactiveProperty<int> CurrentLanguageKeyIndex { get; }
    public string GetName { get; }
    public int GetMyLanguageNamesCount { get; }
}