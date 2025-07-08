
using System;
using Cysharp.Threading.Tasks;

public interface ILevelLocalizationHandler
{
    public UniTaskVoid TrySwitchLanguageFromSettingsChange();
    public event Action OnEndLoadLocalization;
    
    public bool IsLocalizationHasBeenChanged();
}