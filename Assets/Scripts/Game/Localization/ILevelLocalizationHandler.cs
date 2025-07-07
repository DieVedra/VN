
using System;
using Cysharp.Threading.Tasks;

public interface ILevelLocalizationHandler
{
    public UniTask Test();
    public event Action OnEndLoadLocalization;
    
    public bool IsLocalizationHasBeenChanged();
}