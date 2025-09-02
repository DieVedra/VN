using Cysharp.Threading.Tasks;
using UniRx;

public interface ILevelLocalizationHandler
{
    public UniTaskVoid TrySwitchLanguageFromSettingsChange();
    public ReactiveCommand OnEndSwitchLocalization { get; }
    
    public bool IsLocalizationHasBeenChanged();
}