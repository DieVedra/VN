using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeDetector
{
    private readonly float _calculateMultiplier = 0.05f;
    private readonly float _timeValueForClamp = 0.03f;
    private readonly float _sensitivity;
    private readonly InputAction _pressInput;
    private readonly InputAction _positionInput;
    private readonly RectTransform _areaRectTransform;
    private readonly Camera _mainCamera;

    private bool _isPress;
    private bool _isFirstPress;
    private Vector2 _currentPos;
    private Vector2 _previousPos;
    private Vector2 _direction;

    public event Action<bool, float> OnSwipeDirection;
    public event Action OnPress;
    public event Action OnUnpress;
    
    public SwipeDetector(InputAction pressInput, InputAction positionInput, RectTransform areaRectTransform, float sensitivity)
    {
        _isFirstPress = true;
        _isPress = false;
        _pressInput = pressInput;
        _positionInput = positionInput;
        _areaRectTransform = areaRectTransform;
        _mainCamera = Camera.main;
        _sensitivity = sensitivity * _calculateMultiplier;
    }
    public void Enable()
    {
        _pressInput.Enable();
        _positionInput.Enable();
        _positionInput.performed += SetPosition;
        _pressInput.started += SetPress;
        _pressInput.canceled += SetUnpress;
    }

    public void Disable()
    {
        _positionInput.performed -= SetPosition;
        _pressInput.started -= SetPress;
        _pressInput.canceled -= SetUnpress;
        _pressInput.Disable();
        _positionInput.Disable();
    }

    private void SetPress(InputAction.CallbackContext context)
    {
        _isFirstPress = true;
        OnPress?.Invoke();
        _isPress = true;
    }
    private void SetUnpress(InputAction.CallbackContext context)
    {
        OnUnpress?.Invoke();
        _isPress = false;
    }
    private void SetPosition(InputAction.CallbackContext context)
    {
        _currentPos = context.ReadValue<Vector2>();
        if (RectTransformUtility.RectangleContainsScreenPoint(_areaRectTransform, _currentPos, _mainCamera) && _isPress == true)
        {
            CalculateSwipeDirection(_currentPos);
        }
    }
    private float CalculateAddTimeValue(float posX)
    {
        float speedValue = posX * _sensitivity * Time.deltaTime;
        
        return Mathf.Clamp(speedValue, -_timeValueForClamp, _timeValueForClamp);
    }

    private void CalculateSwipeDirection(Vector2 position)
    {
        if (_isFirstPress == true)
        {
            _isFirstPress = false;
            _previousPos = position;
        }
        _direction = _previousPos - position;
        if (Mathf.Abs(_direction.x) > Mathf.Abs(_direction.y))
        {
            if (_direction.x > 0)
            {
                OnSwipeDirection?.Invoke(true, CalculateAddTimeValue(_direction.x)); //+
            }
            else if(_direction.x < 0)
            {
                OnSwipeDirection?.Invoke(false, CalculateAddTimeValue(_direction.x)); //-
            }
        }
        _previousPos = position;
    }
}