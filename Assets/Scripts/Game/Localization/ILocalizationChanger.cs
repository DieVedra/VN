
using Cysharp.Threading.Tasks;
using UniRx;

public interface ILocalizationChanger
{
    public IReactiveProperty<int> CurrentLanguageKeyIndex { get; }
    public string GetName { get; }
    public string GetKey { get; }
    public int GetMyLanguageNamesCount { get; }
    public UniTask LoadAllLanguagesForPanels();
}