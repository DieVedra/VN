using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsCustomizationHandler
{
    private const float _transparentMax = 1f;
    private const float _transparentMin = 0.5f;
    private const float _inverMultiplier = -1f;
    private const float _halfMultiplier = 0.5f;
    private const float _minValue = 0f;
    private const float _distanceBetweenModeButtons  = 296f;
    private const string _defaultText = "0";

    private readonly CustomizationPreliminaryBalanceCalculator _customizationPreliminaryBalanceCalculator;
    private readonly CustomizationCharacterPanelUI _customizationCharacterPanelUI;
    private readonly ArrowSwitch _arrowSwitch;
    private readonly ButtonsModeSwitch _buttonsModeSwitch;
    private readonly CalculateStatsHandler _calculateStatsHandler;
    private readonly PriceViewHandler _priceViewHandler;
    private readonly SwitchInfoCustodian _switchInfoCustodian;
    private readonly ReactiveCommand<bool> _offArrows;

    public ButtonsCustomizationHandler(CustomizationPreliminaryBalanceCalculator customizationPreliminaryBalanceCalculator,
        CustomizationCharacterPanelUI customizationCharacterPanelUI, ArrowSwitch arrowSwitch, ButtonsModeSwitch buttonsModeSwitch,
        CalculateStatsHandler calculateStatsHandler, PriceViewHandler priceViewHandler,
        SwitchInfoCustodian switchInfoCustodian, ReactiveCommand<bool> offArrows)
    {
        _customizationPreliminaryBalanceCalculator = customizationPreliminaryBalanceCalculator;
        _customizationCharacterPanelUI = customizationCharacterPanelUI;
        _arrowSwitch = arrowSwitch;
        _buttonsModeSwitch = buttonsModeSwitch;
        _calculateStatsHandler = calculateStatsHandler;
        _priceViewHandler = priceViewHandler;
        _switchInfoCustodian = switchInfoCustodian;
        _offArrows = offArrows;
    }

    public void ActivateButtonsCustomization(CustomizationCharacterPanelUI customizationCharacterPanelUI, 
        SelectedCustomizationContentIndexes selectedCustomizationContentIndexes,
        CustomizationEndEvent<CustomizationResult> customizationEndEvent = null, ArrowSwitch arrowSwitch = null)
    {
        var customizationButtons = customizationCharacterPanelUI.CustomizationButtons;
        SetDefaultText(customizationButtons);
        var activeButtonsTransforms = TryActivateButtons(selectedCustomizationContentIndexes, customizationButtons, customizationButtons.Count);
        
        TrySetButtonsPositions(activeButtonsTransforms);
        SetButtonTransparency(customizationCharacterPanelUI.LeftArrowCanvasGroup, true);

        if (arrowSwitch != null)
        {
            _offArrows.Subscribe(_ =>
            {
                if (_ == true)
                {
                    customizationCharacterPanelUI.LeftArrow.gameObject.SetActive(false);
                    customizationCharacterPanelUI.RightArrow.gameObject.SetActive(false);
                }
                else
                {
                    customizationCharacterPanelUI.LeftArrow.gameObject.SetActive(true);
                    customizationCharacterPanelUI.RightArrow.gameObject.SetActive(true);
                }
            });
            
            customizationCharacterPanelUI.LeftArrow.onClick.AddListener(() =>
            {
                if (arrowSwitch.PressLeftArrow() == false)
                {
                    SetButtonTransparencyAndBlockRaycasts(customizationCharacterPanelUI.LeftArrowCanvasGroup, true);
                    SetButtonTransparencyAndBlockRaycasts(customizationCharacterPanelUI.RightArrowCanvasGroup, false);
                }
                else
                {
                    SetButtonTransparencyAndBlockRaycasts(customizationCharacterPanelUI.RightArrowCanvasGroup, false);
                }
            });
            customizationCharacterPanelUI.RightArrow.onClick.AddListener(() =>
            {
                if (arrowSwitch.PressRightArrow() == false)
                {
                    SetButtonTransparencyAndBlockRaycasts(customizationCharacterPanelUI.RightArrowCanvasGroup, true);
                    SetButtonTransparencyAndBlockRaycasts(customizationCharacterPanelUI.LeftArrowCanvasGroup, false);
                }
                else
                {
                    SetButtonTransparencyAndBlockRaycasts(customizationCharacterPanelUI.LeftArrowCanvasGroup, false);
                }
            });
        }

        if (customizationEndEvent != null)
        {
            customizationCharacterPanelUI.PlayButton.onClick.AddListener(() =>
            {
                DeactivateButtonsCustomization(customizationCharacterPanelUI);
                customizationEndEvent.Execute(new CustomizationResult(GetStatsToResult(), 
                    _customizationPreliminaryBalanceCalculator.MonetsToShow, _customizationPreliminaryBalanceCalculator.HeartsToShow));
            });
        }

        SetStartModeCustomization(selectedCustomizationContentIndexes.StartMode);
    }

    public void DeactivateButtonsCustomization(CustomizationCharacterPanelUI customizationCharacterPanelUI)
    {
        customizationCharacterPanelUI.SkinColorButton.onClick.RemoveAllListeners();
        customizationCharacterPanelUI.ClothesButton.onClick.RemoveAllListeners();
        customizationCharacterPanelUI.HairstyleButton.onClick.RemoveAllListeners();
        customizationCharacterPanelUI.LeftArrow.onClick.RemoveAllListeners();
        customizationCharacterPanelUI.RightArrow.onClick.RemoveAllListeners();
        customizationCharacterPanelUI.PlayButton.onClick.RemoveAllListeners();
    }

    private List<RectTransform> TryActivateButtons(SelectedCustomizationContentIndexes selectedCustomizationContentIndexes,
        Dictionary<ArrowSwitchMode, (Button, TextMeshProUGUI, RectTransform)> customizationButtons, int customizationButtonsCount)
    {
        List<RectTransform> activeButtonsTransform = new List<RectTransform>(customizationButtonsCount);
        int count = 0;
        foreach (var button in customizationButtons)
        {
            count = GetCount(selectedCustomizationContentIndexes, button.Key);
            if (count > 0)
            {
                activeButtonsTransform.Add(button.Value.Item3);
                SetText(button.Value.Item2, count);
                SubscribeButtonModeCustomization(button.Value.Item1, button.Key);
                button.Value.Item1.gameObject.SetActive(true);
            }
        }
        return activeButtonsTransform;
    }
    private void SubscribeButtonModeCustomization(Button button, ArrowSwitchMode mode)
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
            case ArrowSwitchMode.Swimsuits:
                clothesModeButtonTransparency = false;
                break;
        }
        
        button.onClick.AddListener(() =>
        {
            SetButtonTransparency(_customizationCharacterPanelUI.SkinColorModeCanvasGroup, bodyModeButtonTransparency);
            SetButtonTransparency(_customizationCharacterPanelUI.ClothesModeCanvasGroup, clothesModeButtonTransparency);
            SetButtonTransparency(_customizationCharacterPanelUI.HairstyleModeCanvasGroup, hairstyleModeButtonTransparency);
            _buttonsModeSwitch.SetMode(mode);
            SetArrowsActive();
        });
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

    private void TrySetButtonsPositions(List<RectTransform> activeButtonsTransform)
    {
        if (activeButtonsTransform.Count == 0)
        {
            return;
        }
        if (activeButtonsTransform.Count == 1)
        {
            activeButtonsTransform[0].anchoredPosition = new Vector2(_minValue, activeButtonsTransform[0].anchoredPosition.y);
            return;
        }
        var firstPos = new Vector2(CalculatePositionXFirstSegment(activeButtonsTransform.Count), activeButtonsTransform[0].anchoredPosition.y);
        activeButtonsTransform[0].anchoredPosition = firstPos;
        for (int i = 1; i < activeButtonsTransform.Count; i++)
        {
            activeButtonsTransform[i].anchoredPosition = new Vector2(
                activeButtonsTransform[i-1].anchoredPosition.x + _distanceBetweenModeButtons,
                activeButtonsTransform[i].anchoredPosition.y);
        }
    }
    private float CalculatePositionXFirstSegment(int count)
    {
        float halfCountSegments = count * _halfMultiplier;
        float indent;
        if (count % 2 == 1)
        {
            indent = (_distanceBetweenModeButtons * ((int) halfCountSegments));
        }
        else
        {
            float value = _distanceBetweenModeButtons * halfCountSegments;
            indent = value - (_distanceBetweenModeButtons * _halfMultiplier);
        }
        indent = indent * _inverMultiplier;
        return indent;
    }

    private void SetDefaultText(Dictionary<ArrowSwitchMode, (Button, TextMeshProUGUI, RectTransform)> customizationButtons)
    {
        foreach (var button in customizationButtons)
        {
            button.Value.Item2.text = _defaultText;
            button.Value.Item1.gameObject.SetActive(false);
        }
    }
    private void SetText(TextMeshProUGUI textComponent, int value)
    {
        if (string.IsNullOrEmpty(textComponent.text) == false && string.IsNullOrWhiteSpace(textComponent.text) == false)
        {
            if (int.TryParse(textComponent.text, out int result))
            {
                textComponent.text = (result + value).ToString();
            }
            else
            {
                Set();
            }
        }
        else
        {
            Set();
        }
        void Set()
        {
            textComponent.text = value.ToString();
        }
    }

    private int GetCount(SelectedCustomizationContentIndexes selectedCustomizationContentIndexes, ArrowSwitchMode mode)
    {
        return selectedCustomizationContentIndexes.IndexesSpriteIndexes[(int)mode].Count;
    }
}