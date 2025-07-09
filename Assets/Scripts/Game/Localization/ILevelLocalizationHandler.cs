
using System;
using Cysharp.Threading.Tasks;

public interface ILevelLocalizationHandler
{
    public UniTaskVoid TrySwitchLanguageFromSettingsChange();
    public event Action OnTrySwitchLocalization;
    public event Action OnEndSwitchLocalization;
    
    public bool IsLocalizationHasBeenChanged();
}