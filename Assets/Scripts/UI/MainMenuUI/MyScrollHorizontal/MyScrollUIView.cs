
using UnityEngine;
using UnityEngine.InputSystem;

public class MyScrollUIView : MonoBehaviour
{
    [SerializeField] private RectTransform _transform;
    [SerializeField] private RectTransform _content;
    [SerializeField] private RectTransform _swipeAreaRect;
    [SerializeField] private RectTransform _swipeProgressIndicatorsParent;

    [SerializeField] private float _moveStep;
    [SerializeField] private float _moveStepIndicator;
    [SerializeField] private AnimationCurve _scaleCurveHide;
    [SerializeField] private AnimationCurve _scaleCurveUnhide;
    [SerializeField, Range(0.5f, 1.5f)] private float _sensitivitySlider = 1f;
    [SerializeField] private InputAction _pressInputAction;
    [SerializeField] private InputAction _positionInputAction;
    
    public RectTransform Content => _content;
    public RectTransform Transform => _transform;
    public RectTransform SwipeAreaRect => _swipeAreaRect;
    public RectTransform SwipeProgressIndicatorsParent => _swipeProgressIndicatorsParent;
    public AnimationCurve ScaleCurveHide => _scaleCurveHide;
    public AnimationCurve ScaleCurveUnhide => _scaleCurveUnhide;
    public InputAction PressInputAction => _pressInputAction;
    public InputAction PositionInputAction => _positionInputAction;
    public float MoveStep => _moveStep;
    public float MoveStepIndicator => _moveStepIndicator;
    public float SensitivitySlider => _sensitivitySlider;
}