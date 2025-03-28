﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;

public class ArrowSwitch
{
    private readonly ICharacterCustomizationView _characterCustomizationView;
    private readonly SelectedCustomizationContentIndexes _selectedCustomizationContentIndexes;
    private readonly TextMeshProUGUI _titleTextComponent;
    private readonly ButtonPlayHandler _buttonPlayHandler;
    private readonly SwitchModeCustodian _switchModeCustodian;
    private readonly CustomizationSettingsCustodian _customizationSettingsCustodian;
    private readonly SwitchInfoCustodian _switchInfoCustodian;
    private readonly CustomizationDataProvider _customizationDataProvider;
    private readonly ReactiveProperty<bool> _isNuClothesReactiveProperty;
    private readonly StatViewHandler _statViewHandler;
    private readonly PriceViewHandler _priceViewHandler;
    private bool _isSwitched;

    private Queue<Func<UniTask>> _tasksQueue;
    public int CurrentSwitchIndex => _switchInfoCustodian.CurrentSwitchInfo.Index;
    public int CurrentCustomizationSettingsesCount => _customizationSettingsCustodian.CurrentCustomizationSettings.Count;
    public ArrowSwitch(ICharacterCustomizationView characterCustomizationView, SelectedCustomizationContentIndexes selectedCustomizationContentIndexes,
        StatViewHandler statViewHandler, PriceViewHandler priceViewHandler,
        TextMeshProUGUI titleTextComponent, ButtonPlayHandler buttonPlayHandler,
        SwitchModeCustodian switchModeCustodian, CustomizationSettingsCustodian customizationSettingsCustodian,
        SwitchInfoCustodian switchInfoCustodian, CustomizationDataProvider customizationDataProvider,
        ReactiveProperty<bool> isNuClothesReactiveProperty)
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
        _isNuClothesReactiveProperty = isNuClothesReactiveProperty;
        _isSwitched = false;
        _tasksQueue = new Queue<Func<UniTask>>();
    }

    public void Dispose()
    {
        _isSwitched = false;
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
        _switchInfoCustodian.SetStatsToCurrentSwitchInfo();
        _switchInfoCustodian.SetPriceToCurrentSwitchInfo();

        SetTitle();
        _tasksQueue.Enqueue(() => UniTask.WhenAll(
            CreateOperationToQueue(customizationSettings, customizationData, directionType, price)
        ));

        TrySwitch().Forget();
    }

    private void SetTitle()
    {
        _titleTextComponent.text = _customizationSettingsCustodian.CurrentCustomizationSettings[CurrentSwitchIndex].Name;
    }

    private List<UniTask> CreateOperationToQueue(ICustomizationSettings customizationSettings, CustomizationData customizationData, DirectionType directionType, int price)
    {
        List<UniTask> returnOperations = new List<UniTask>();
        switch (directionType)
        {
            case DirectionType.Right:
                returnOperations.Add(_characterCustomizationView.SetCharacterCustomizationFromRightArrow(customizationData));
                break;
            
            case DirectionType.Left:
                returnOperations.Add(_characterCustomizationView.SetCharacterCustomizationFromLeftArrow(customizationData));
                break;
        }
        
        if (_statViewHandler.CheckViewStatToShow(customizationSettings))
        {
            Stat stat = _statViewHandler.GetStatInfo(customizationSettings.GameStats);
            if (_statViewHandler.IsShowed == true)
            {
                returnOperations.Add(_statViewHandler.HideToShowAnim(stat.ColorField, _statViewHandler.CreateLabel(stat)));
            }
            else
            {
                returnOperations.Add( _statViewHandler.ShowAnim(stat.ColorField, _statViewHandler.CreateLabel(stat)));
            }
        }
        else
        {
            if (_statViewHandler.IsShowed == true)
            {
                returnOperations.Add(_statViewHandler.HideAnim());
            }
        }

        if (CheckShowPrice(price))
        {
            if (_priceViewHandler.IsShowed == true)
            {
                returnOperations.Add(_priceViewHandler.HideToShowAnim(price));
            }
            else
            {
                returnOperations.Add(_priceViewHandler.ShowAnim(price));
            }
        }
        else
        {
            if (_priceViewHandler.IsShowed == true)
            {
                returnOperations.Add(_priceViewHandler.HideAnim());
            }
        }

        if (CheckAvailableMoney(price) == true)
        {
            if (_buttonPlayHandler.IsActive == false)
            {
                returnOperations.Add(_buttonPlayHandler.TryOnAnim());
            }
        }
        else
        {
            if (_buttonPlayHandler.IsActive == true)
            {
                returnOperations.Add(_buttonPlayHandler.OffAnim());
            }
        }
        return returnOperations;
    }
    private void SetChangedCustomizationIndexes()
    {
        switch (_switchModeCustodian.Mode)
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
                await _tasksQueue.Dequeue().Invoke();

                if (_tasksQueue.Count == 0)
                {
                    _isSwitched = false;
                }
            }
        }
    }

    private bool CheckShowPrice(int price)
    {
        if (price  > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckAvailableMoney(int price)
    {
        if (_priceViewHandler.CalculatePriceHandler.CheckAvailableMoney(price) == true)
        {
            return true;
        }
        else return false;
    }
}