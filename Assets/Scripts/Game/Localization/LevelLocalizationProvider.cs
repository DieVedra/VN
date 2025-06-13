
using Cysharp.Threading.Tasks;
using UniRx;

public class LevelLocalizationProvider
{
    public LevelLocalizationProvider(ILocalizationChanger localizationChanger)
    {
        CompositeDisposable compositeDisposable = new CompositeDisposable();
        localizationChanger.CurrentLanguageKeyIndex.Subscribe(_ =>
        {
            
        }).AddTo(compositeDisposable);
    }

    public async UniTask LoadLocalization(int seriaIndex, string language)
    {
        
    }
}