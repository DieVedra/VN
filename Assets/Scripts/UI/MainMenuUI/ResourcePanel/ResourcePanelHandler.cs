using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class ResourcePanelHandler
{
    private const float _posXValueWithAddButtonMode = -112f;
    private const float _posXValueWithoutAddButtonMode = -29f;
    private Transform _parentDefault;
    private RectTransform _panelTransform;
    private RectTransform _textTransform;
    private ResourcePanelView _panelView;
    private CompositeDisposable _compositeDisposable;
    private ResourcePanelMode _currentMode;
    public RectTransform PanelTransform => _panelTransform;
    public bool IsInited { get; private set; }
    public event Action<ResourcePanelMode, int> OnResize;

    public void Init(ResourcePanelView panelView, int value, Color panelColor, Color panelButtonColor, IReactiveCommand<int> resourceChanged = null)
    {
        if (IsInited == false)
        {
            IsInited = true;
            _parentDefault = panelView.transform.parent;
            _panelView = panelView;
            _panelTransform = _panelView.transform as RectTransform;
            _textTransform = _panelView.Text.transform as RectTransform;
            _panelView.Button.image.color = panelButtonColor;
            _panelView.Panel.color = panelColor;
            
            SetParentDefault();
            SetValue(value);
            _compositeDisposable = new CompositeDisposable();
            _panelView.gameObject.SetActive(true);
            _parentDefault.gameObject.SetActive(true);
            resourceChanged?.Subscribe(_=>
            {
                SetValue(_);
                Resize();
            }).AddTo(_compositeDisposable);
        }
    }

    public void SetSprite(Sprite icon)
    {
        _panelView.Icon.sprite = icon;
    }
    public void SetParent(Transform parent)
    {
        _panelView.transform.SetParent(parent);
        SwitchMode(ResourcePanelMode.WithoutAddButton);
        Resize();
    }
    public void SetParentDefault()
    {
        _panelView.transform.SetParent(_parentDefault);
        SwitchMode(ResourcePanelMode.WithAddButton);
        Resize();
    }
    public void Shutdown()
    {
        _compositeDisposable?.Clear();
        _panelView.Button.onClick.RemoveAllListeners();
    }
    private void Resize()
    {
        Vector2 vector = new Vector2 {y = _panelTransform.sizeDelta.y};
        switch (_currentMode)
        {
            case ResourcePanelMode.WithAddButton:
                vector.x = _panelView.CurveWithAddButton.Evaluate(_panelView.Text.text.Length);
                break;
            case ResourcePanelMode.WithoutAddButton:
                vector.x = _panelView.CurveWithoutAddButton.Evaluate(_panelView.Text.text.Length);
                break;
        }
        _panelTransform.sizeDelta = vector;
        OnResize?.Invoke(_currentMode, _panelView.Text.text.Length);
    }

    private void SetValue(int value)
    {
        _panelView.Text.text = value.ToString();
    }
    private void SwitchMode(ResourcePanelMode mode)
    {
        switch (mode)
        {
            case ResourcePanelMode.WithAddButton:
                _textTransform.anchoredPosition = new Vector2(_posXValueWithAddButtonMode, _textTransform.anchoredPosition.y);
                _panelView.Button.gameObject.SetActive(true);
                break;
            case ResourcePanelMode.WithoutAddButton:
                _panelView.Button.onClick.RemoveAllListeners();
                _panelView.Button.gameObject.SetActive(false);
                _textTransform.anchoredPosition = new Vector2(_posXValueWithoutAddButtonMode, _textTransform.anchoredPosition.y);
                break;
        }
        _currentMode = mode;
    }
    public void SubscribeButton(Action onPress)
    {
        SwitchMode(ResourcePanelMode.WithAddButton);
        Resize();
        _panelView.Button.onClick.AddListener(onPress.Invoke);
    }
    public async UniTask DoAnimPanel(CancellationToken cancellationToken, 
        float duration, float alpha, float endValue)
    {
        _panelView.CanvasGroup.alpha = alpha;
        await _panelView.CanvasGroup.DOFade(endValue, duration)
            .WithCancellation(cancellationToken);
    }
    public void DoPanel(float alpha, bool key)
    {
        _panelView.CanvasGroup.alpha = alpha;
        _panelView.gameObject.SetActive(key);
    }
}