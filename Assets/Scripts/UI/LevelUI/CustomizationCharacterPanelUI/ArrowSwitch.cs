using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;

public class ArrowSwitch
{
    private readonly ICharacterCustomizationView _characterCustomizationView;
    private readonly SelectedCustomizationContentIndexes _selectedCustomizationContentIndexes;
    private readonly TextMeshProUGUI _titleTextComponent;
    private readonly ButtonPlayHandler _buttonPlayHandler;
    private readonly ReactiveProperty<ArrowSwitchMode> _switchModeCustodian;
    private readonly CustomizationSettingsCustodian _customizationSettingsCustodian;
    private readonly SwitchInfoCustodian _switchInfoCustodian;
    private readonly CustomizationDataProvider _customizationDataProvider;
    private readonly PreliminaryBalanceCalculator _preliminaryBalanceCalculator;
    private readonly ReactiveProperty<bool> _isNuClothesReactiveProperty;
    private readonly StatViewHandler _statViewHandler;
    private readonly PriceViewHandler _priceViewHandler;
    private CompositeDisposable _compositeDisposable;
    private bool _isSwitched;

    private Queue<TaskRunner> _tasksQueue;
    public int CurrentSwitchIndex => _switchInfoCustodian.CurrentSwitchInfo.Index;
    public int CurrentCustomizationSettingsesCount => _customizationSettingsCustodian.CurrentCustomizationSettings.Count;
    public ArrowSwitch(ICharacterCustomizationView characterCustomizationView, SelectedCustomizationContentIndexes selectedCustomizationContentIndexes,
        StatViewHandler statViewHandler, PriceViewHandler priceViewHandler,
        TextMeshProUGUI titleTextComponent, ButtonPlayHandler buttonPlayHandler,
        ReactiveProperty<ArrowSwitchMode> switchModeCustodian, CustomizationSettingsCustodian customizationSettingsCustodian,
        SwitchInfoCustodian switchInfoCustodian, CustomizationDataProvider customizationDataProvider,
        PreliminaryBalanceCalculator preliminaryBalanceCalculator,
        ReactiveProperty<bool> isNuClothesReactiveProperty, SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        _characterCustomizationView = characterCustomizationView;
        _selectedCustomizationContentIndexes = selectedCustomizationContentIndexes;
        _statViewHandler = statViewHandler;
        _priceViewHandler = priceViewHandler;
        _titleTextComponent = titleTextComponent;
        _buttonPlayHandler = buttonPlayHandler;
        _switchModeCustodian = switchModeCustodian;
        _customizationSettingsCustodian = customizationSettingsCustodian;
        _switchInfoCustodian = switchInfoCustodian;
        _customizationDataProvider = customizationDataProvider;
        _preliminaryBalanceCalculator = preliminaryBalanceCalculator;
        _isNuClothesReactiveProperty = isNuClothesReactiveProperty;
        _isSwitched = false;
        _tasksQueue = new Queue<TaskRunner>();
        _compositeDisposable = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTitle);
    }

    public void Dispose()
    {
        _isSwitched = false;
        _compositeDisposable.Dispose();
    }
    public bool PressLeftArrow()
    {
        if (_switchInfoCustodian.CurrentSwitchIndex == 1)
        {
            _switchInfoCustodian.CurrentSwitchInfo.Index--;
            PressArrow(DirectionType.Left);
            return false;
        }
        else if (_switchInfoCustodian.CurrentSwitchIndex > 0)
        {
            _switchInfoCustodian.CurrentSwitchInfo.Index--;
            PressArrow(DirectionType.Left);
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public bool PressRightArrow()
    {
        if (_switchInfoCustodian.CurrentSwitchIndex == _customizationSettingsCustodian.CurrentCustomizationSettings.Count - 2)
        {
            _switchInfoCustodian.CurrentSwitchInfo.Index++;
            PressArrow(DirectionType.Right);
            return false;
        }
        else if(_switchInfoCustodian.CurrentSwitchIndex < _customizationSettingsCustodian.CurrentCustomizationSettings.Count - 1)
        {
            _switchInfoCustodian.CurrentSwitchInfo.Index++;
            PressArrow(DirectionType.Right);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void PressArrow(DirectionType directionType)
    {
        SetChangedCustomizationIndexes();
        CustomizationData customizationData = _customizationDataProvider.CreateCustomizationData(CurrentSwitchIndex);
        ICustomizationSettings customizationSettings = _customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex];
        int price = _customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].Price;
        int additionalPrice = _customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].PriceAdditional;
        _switchInfoCustodian.SetStatsToCurrentSwitchInfo();
        _switchInfoCustodian.SetPriceToCurrentSwitchInfo();
        _switchInfoCustodian.SetAdditionalPriceToCurrentSwitchInfo();

        SetTitle();
        _tasksQueue.Enqueue(CreateOperationToQueue(customizationSettings, customizationData, directionType, price, additionalPrice));
        TrySwitch().Forget();
    }

    private void SetTitle()
    {
        _titleTextComponent.text = _customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].Name;
    }

    private TaskRunner CreateOperationToQueue(ICustomizationSettings customizationSettings, CustomizationData customizationData,
        DirectionType directionType, int price, int additionalPrice)
    {
        TaskRunner taskRunner = new TaskRunner();
        switch (directionType)
        {
            case DirectionType.Right:
                
                taskRunner.AddOperationToList(() => _characterCustomizationView.SetCharacterCustomizationFromRightArrow(customizationData));
                break;
            
            case DirectionType.Left:
                taskRunner.AddOperationToList(() => _characterCustomizationView.SetCharacterCustomizationFromLeftArrow(customizationData));
                break;
        }
        
        if (_statViewHandler.CheckViewStatToShow(customizationSettings))
        {
            Stat stat = _statViewHandler.GetStatInfo(customizationSettings.GameStats);
            if (_statViewHandler.IsShowed == true)
            {
                taskRunner.AddOperationToList(() => _statViewHandler.HideToShowAnim(stat.ColorField, _statViewHandler.CreateLabel(stat)));
            }
            else
            {
                taskRunner.AddOperationToList(() => _statViewHandler.ShowAnim(stat.ColorField, _statViewHandler.CreateLabel(stat)));
            }
        }
        else
        {
            if (_statViewHandler.IsShowed == true)
            {
                taskRunner.AddOperationToList(() => _statViewHandler.HideAnim());
            }
        }

        if (CheckShowPrices(price, additionalPrice))
        {
            if (_priceViewHandler.PanelIsShowed == true)
            {
                taskRunner.AddOperationToList(() => _priceViewHandler.HideToShowAnim(price, additionalPrice));
            }
            else
            {
                taskRunner.AddOperationToList(() => _priceViewHandler.ShowAnim(price, additionalPrice));
            }
        }
        else
        {
            if (_priceViewHandler.PanelIsShowed == true)
            {
                taskRunner.AddOperationToList(() => _priceViewHandler.HideAnim());
            }
        }

        if (_preliminaryBalanceCalculator.CheckAvailableMoneyAndHearts(_switchInfoCustodian.GetAllPriceInfo))
        {
            if (_buttonPlayHandler.IsActive == false)
            {
                taskRunner.AddOperationToList(() => _buttonPlayHandler.TryOnAnim());
            }
        }
        else
        {
            if (_buttonPlayHandler.IsActive == true)
            {
                taskRunner.AddOperationToList(() => _buttonPlayHandler.OffAnim());
            }
        }
        return taskRunner;
    }
    private void SetChangedCustomizationIndexes()
    {
        switch (_switchModeCustodian.Value)
        {
            case ArrowSwitchMode.SkinColor:
                _selectedCustomizationContentIndexes.CustomizableCharacter.SetBodyIndex(_customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].Index);
                _switchInfoCustodian.SetToBodyInfoCurrentSwitchInfo();
                break;
            case ArrowSwitchMode.Hairstyle:
                _selectedCustomizationContentIndexes.CustomizableCharacter.SetHairstyleIndex(_customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].Index);
                _switchInfoCustodian.SetToHairstyleInfoCurrentSwitchInfo();
                break;
            case ArrowSwitchMode.Clothes:
                if (_isNuClothesReactiveProperty.Value)
                {
                    _selectedCustomizationContentIndexes.CustomizableCharacter.SetSwimsuitsIndex(_customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].Index);
                }
                else
                {
                    _selectedCustomizationContentIndexes.CustomizableCharacter.SetClothesIndex(_customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].Index);
                }
                _switchInfoCustodian.SetToClothesInfoCurrentSwitchInfo();
                break;
        }
    }
    private async UniTask TrySwitch()
    {
        if (_isSwitched == false)
        {
            _isSwitched = true;
            while (_isSwitched == true)
            {
                await _tasksQueue.Dequeue().TryRunTasks();

                if (_tasksQueue.Count == 0)
                {
                    _isSwitched = false;
                }
            }
        }
    }

    private bool CheckShowPrices(int price, int additionalPrice)
    {
        if (price  > 0 || additionalPrice > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}