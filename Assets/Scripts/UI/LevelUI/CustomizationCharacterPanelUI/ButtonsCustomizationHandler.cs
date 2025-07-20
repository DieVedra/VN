using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsCustomizationHandler
{
    private const float _transparentMax = 1f;
    private const float _transparentMin = 0.5f;
    private readonly CustomizationCharacterPanelUI _customizationCharacterPanelUI;
    private readonly ArrowSwitch _arrowSwitch;
    private readonly ButtonsModeSwitch _buttonsModeSwitch;
    private readonly CalculateStatsHandler _calculateStatsHandler;
    private readonly PriceViewHandler _priceViewHandler;
    private readonly SwitchInfoCustodian _switchInfoCustodian;

    public ButtonsCustomizationHandler(
        CustomizationCharacterPanelUI customizationCharacterPanelUI, ArrowSwitch arrowSwitch, ButtonsModeSwitch buttonsModeSwitch,
        CalculateStatsHandler calculateStatsHandler, PriceViewHandler priceViewHandler,
        SwitchInfoCustodian switchInfoCustodian)
    {
        _customizationCharacterPanelUI = customizationCharacterPanelUI;
        _arrowSwitch = arrowSwitch;
        _buttonsModeSwitch = buttonsModeSwitch;
        _calculateStatsHandler = calculateStatsHandler;
        _priceViewHandler = priceViewHandler;
        _switchInfoCustodian = switchInfoCustodian;
    }

    public void ActivateButtonsCustomization(SelectedCustomizationContentIndexes selectedCustomizationContentIndexes, CustomizationEndEvent<CustomizationResult> customizationEndEvent)
    {
        SubscribeButtonModeCustomization(
            _customizationCharacterPanelUI.SkinColorButton,
            ArrowSwitchMode.SkinColor,
            selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int)ArrowSwitchMode.SkinColor].Count);
        
        SubscribeButtonModeCustomization(
            _customizationCharacterPanelUI.ClothesButton,
            ArrowSwitchMode.Clothes,
            selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int) ArrowSwitchMode.Clothes].Count == 0
                ? selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int) ArrowSwitchMode.Swimsuits].Count
            : selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int) ArrowSwitchMode.Clothes].Count);
        
        SubscribeButtonModeCustomization(
            _customizationCharacterPanelUI.HairstyleButton,
            ArrowSwitchMode.Hairstyle,
            selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int)ArrowSwitchMode.Hairstyle].Count);
        
        SetButtonTransparency(_customizationCharacterPanelUI.LeftArrowCanvasGroup, true);
        
        _customizationCharacterPanelUI.LeftArrow.onClick.AddListener(() =>
        {
            if (_arrowSwitch.PressLeftArrow() == false)
            {
                SetButtonTransparencyAndBlockRaycasts(_customizationCharacterPanelUI.LeftArrowCanvasGroup, true);
                SetButtonTransparencyAndBlockRaycasts(_customizationCharacterPanelUI.RightArrowCanvasGroup, false);
            }
            else
            {
                SetButtonTransparencyAndBlockRaycasts(_customizationCharacterPanelUI.RightArrowCanvasGroup, false);
            }
        });
        
        _customizationCharacterPanelUI.RightArrow.onClick.AddListener(() =>
        {
            if (_arrowSwitch.PressRightArrow() == false)
            {
                SetButtonTransparencyAndBlockRaycasts(_customizationCharacterPanelUI.RightArrowCanvasGroup, true);
                SetButtonTransparencyAndBlockRaycasts(_customizationCharacterPanelUI.LeftArrowCanvasGroup, false);
            }
            else
            {
                SetButtonTransparencyAndBlockRaycasts(_customizationCharacterPanelUI.LeftArrowCanvasGroup, false);
            }
        });
        
        _customizationCharacterPanelUI.PlayButton.onClick.AddListener(() =>
        {
            DeactivateButtonsCustomization();
            customizationEndEvent.Execute(new CustomizationResult(GetStatsToResult(), 
                _priceViewHandler.CalculateBalanceHandler.MonetsToShow, _priceViewHandler.CalculateBalanceHandler.HeartsToShow));
        });

        SetStartModeCustomization(selectedCustomizationContentIndexes.StartMode);
    }

    public void DeactivateButtonsCustomization()
    {
        _customizationCharacterPanelUI.SkinColorButton.onClick.RemoveAllListeners();
        _customizationCharacterPanelUI.ClothesButton.onClick.RemoveAllListeners();
        _customizationCharacterPanelUI.HairstyleButton.onClick.RemoveAllListeners();
        _customizationCharacterPanelUI.LeftArrow.onClick.RemoveAllListeners();
        _customizationCharacterPanelUI.RightArrow.onClick.RemoveAllListeners();
        _customizationCharacterPanelUI.PlayButton.onClick.RemoveAllListeners();
    }

    private void SubscribeButtonModeCustomization(Button button, ArrowSwitchMode mode, int contentCount)
    {
        bool bodyModeButtonTransparency = true;
        bool clothesModeButtonTransparency = true;
        bool hairstyleModeButtonTransparency = true;
        
        switch (mode)
        {
            case ArrowSwitchMode.SkinColor:
                bodyModeButtonTransparency = false;
                break;
            case ArrowSwitchMode.Hairstyle:
                hairstyleModeButtonTransparency = false;
                break;
            case ArrowSwitchMode.Clothes:
                clothesModeButtonTransparency = false;
                break;
        }
        
        if (contentCount > 0)
        {
            button.onClick.AddListener(() =>
            {
                SetButtonTransparency(_customizationCharacterPanelUI.SkinColorModeCanvasGroup, bodyModeButtonTransparency);
                SetButtonTransparency(_customizationCharacterPanelUI.ClothesModeCanvasGroup, clothesModeButtonTransparency);
                SetButtonTransparency(_customizationCharacterPanelUI.HairstyleModeCanvasGroup, hairstyleModeButtonTransparency);
                _buttonsModeSwitch.SetMode(mode);
                SetArrowsActive();
            });
        }
    }

    private void SetArrowsActive()
    {
        if (_arrowSwitch.CurrentSwitchIndex == 0)
        {
            SetButtonTransparencyAndBlockRaycasts(_customizationCharacterPanelUI.LeftArrowCanvasGroup, true);
            SetButtonTransparencyAndBlockRaycasts(_customizationCharacterPanelUI.RightArrowCanvasGroup, false);
        }
        else if(_arrowSwitch.CurrentSwitchIndex == _arrowSwitch.CurrentCustomizationSettingsesCount -1)
        {
            SetButtonTransparencyAndBlockRaycasts(_customizationCharacterPanelUI.RightArrowCanvasGroup, true);
            SetButtonTransparencyAndBlockRaycasts(_customizationCharacterPanelUI.LeftArrowCanvasGroup, false);
        }
        else
        {
            SetButtonTransparencyAndBlockRaycasts(_customizationCharacterPanelUI.LeftArrowCanvasGroup, false);
            SetButtonTransparencyAndBlockRaycasts(_customizationCharacterPanelUI.RightArrowCanvasGroup, false);
        }
    }

    private void SetStartModeCustomization(ArrowSwitchMode startMode)
    {
        SetButtonTransparency(_customizationCharacterPanelUI.SkinColorModeCanvasGroup, true);
        SetButtonTransparency(_customizationCharacterPanelUI.ClothesModeCanvasGroup, true);
        SetButtonTransparency(_customizationCharacterPanelUI.HairstyleModeCanvasGroup, true);

        switch (startMode)
        {
            case ArrowSwitchMode.SkinColor:
                SetButtonTransparency(_customizationCharacterPanelUI.SkinColorModeCanvasGroup, false);
                break;
            case ArrowSwitchMode.Hairstyle:
                SetButtonTransparency(_customizationCharacterPanelUI.HairstyleModeCanvasGroup, false);
                break;
            case ArrowSwitchMode.Clothes:
                SetButtonTransparency(_customizationCharacterPanelUI.ClothesModeCanvasGroup, false);
                break;
            case ArrowSwitchMode.Swimsuits:
                SetButtonTransparency(_customizationCharacterPanelUI.ClothesModeCanvasGroup, false);
                break;
        }
    }

    private void SetButtonTransparency(CanvasGroup canvasGroup, bool isTransparent)
    {
        if (isTransparent == true)
        {
            canvasGroup.alpha = _transparentMin;
        }
        else
        {
            canvasGroup.alpha = _transparentMax;
        }
    }

    private void SetButtonTransparencyAndBlockRaycasts(CanvasGroup canvasGroup, bool isTransparent)
    {
        SetButtonTransparency(canvasGroup, isTransparent);
        
        if (isTransparent == true)
        {
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            canvasGroup.blocksRaycasts = true;
        }
    }

    private List<BaseStat> GetStatsToResult()
    {
        _calculateStatsHandler.PreliminaryStatsCalculation(_switchInfoCustodian.GetAllInfo);
        return _calculateStatsHandler.PreliminaryStats;
    }
}