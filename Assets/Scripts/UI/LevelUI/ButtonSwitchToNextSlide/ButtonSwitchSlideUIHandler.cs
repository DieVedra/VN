
using System;
using UnityEngine;

public class ButtonSwitchSlideUIHandler
{
    private readonly ButtonSwitchSlideUI _buttonSwitchSlideUI;
    private readonly SwitchToNextNodeEvent _switchToNextNodeEvent;

    public ButtonSwitchSlideUIHandler(ButtonSwitchSlideUI buttonSwitchSlideUI, SwitchToNextNodeEvent switchToNextNodeEvent)
    {
        _buttonSwitchSlideUI = buttonSwitchSlideUI;
        _switchToNextNodeEvent = switchToNextNodeEvent;
        buttonSwitchSlideUI.gameObject.SetActive(true);
        Unsubscribe();
    }

    public void ActivateSkipTransition(Action operation)
    {
        Unsubscribe();
        ActivatePushOption();
        _buttonSwitchSlideUI.ButtonSwitchNextSlide.onClick.AddListener(()=>
        {
            DeactivatePushOption();
            operation.Invoke();
        });
    }
    public void ActivateButtonSwitchToNextNode()
    {
        Unsubscribe();
        ActivatePushOption();
        _buttonSwitchSlideUI.ButtonSwitchNextSlide.onClick.AddListener(()=>
        {
            DeactivatePushOption();
            _switchToNextNodeEvent.Execute();
        });
    }

    private void ActivatePushOption()
    {
        _buttonSwitchSlideUI.gameObject.SetActive(true);
    }

    public void DeactivatePushOption()
    {
        Unsubscribe();
        _buttonSwitchSlideUI.gameObject.SetActive(false);
    }

    private void Unsubscribe()
    {
        _buttonSwitchSlideUI.ButtonSwitchNextSlide.onClick.RemoveAllListeners();
    }
}