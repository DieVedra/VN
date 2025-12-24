using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressDetector
{
    private readonly InputAction _pressInput;
    private readonly InputAction _positionInput;
    private bool _isPress = false;
    private bool _isFirstPress = false;
    private Vector2 _previousPos = new Vector2();
    private Vector2 _currentPos = new Vector2();
    private Action _onPressOperation;
    public bool IsActive { get; private set; }
    public PressDetector(InputAction pressInput, InputAction positionInput)
    {
        _pressInput = pressInput;
        _positionInput = positionInput;
        IsActive = false;
    }
    public void Enable(Action operation)
    {
        _pressInput.Enable();
        _positionInput.Enable();
        _positionInput.performed += CheckPosition;
        _pressInput.started += SetPress;
        _pressInput.canceled += SetUnpress;
        _onPressOperation = operation;
        IsActive = true;

    }
    public void Disable()
    {
        _positionInput.performed -= CheckPosition;
        _pressInput.started -= SetPress;
        _pressInput.canceled -= SetUnpress;
        _pressInput.Disable();
        _positionInput.Disable();
        _onPressOperation = null;
        IsActive = false;
    }
    private void SetPress(InputAction.CallbackContext context)
    {
        _isFirstPress = true;
        _isPress = true;
    }
    private void SetUnpress(InputAction.CallbackContext context)
    {
        _isPress = false;

        if (_currentPos == _previousPos)
        {
            _onPressOperation?.Invoke();
        }
    }

    private void CheckPosition(InputAction.CallbackContext context)
    {
        if (_isPress == true)
        {
            if (_isFirstPress == true)
            {
                _isFirstPress = false;
                _previousPos = context.ReadValue<Vector2>();
                _currentPos = _previousPos;
            }
            _currentPos = context.ReadValue<Vector2>();
        }
    }
}