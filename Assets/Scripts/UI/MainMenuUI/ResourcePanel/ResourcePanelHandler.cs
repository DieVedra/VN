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
    private const float _showAlphaValue = 1f;
    private Transform _parentDefault;
    private RectTransform _panelTransform;
    private RectTransform _textTransform;
    private ResourcePanelView _panelView;
    private CompositeDisposable _compositeDisposable;
    private CancellationTokenSource _cancellationTokenSource;
    private ResourcePanelMode _currentMode;
    private Vector3 _defaultPosition;
    private float _buferAlpha = _showAlphaValue; 
    private bool _buferActiveKey;
    public RectTransform PanelTransform => _panelTransform;
    public bool IsInited { get; private set; }
    public event Action<ResourcePanelMode, int> OnResize;

    public void Init(ResourcePanelView panelView, int value, Color panelColor, Color panelButtonColor, IReactiveCommand<int> resourceChanged = null)
    {
        if (IsInited == false)
        {
            IsInited = true;
            _panelView = panelView;
            _panelTransform = _panelView.transform as RectTransform;
            _defaultPosition = _panelTransform.localPosition;
            _parentDefault = _panelTransform.parent;
            _textTransform = _panelView.Text.transform as RectTransform;
            _panelView.Button.image.color = panelButtonColor;
            _panelView.Panel.color = panelColor;
            SetParentDefault();
            SetValue(value);
            _compositeDisposable = new CompositeDisposable();
            _cancellationTokenSource = new CancellationTokenSource();
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
        _buferAlpha = _panelView.CanvasGroup.alpha;
        _buferActiveKey = _panelView.gameObject.activeSelf;
        DoPanel(_showAlphaValue, true);
    }
    public void SetParentDefault()
    {
        _panelView.transform.SetParent(_parentDefault);
        
        SwitchMode(ResourcePanelMode.WithAddButton);
        Resize();
        DoPanel(_buferAlpha, _buferActiveKey);
    }
    public void Shutdown()
    {
        _compositeDisposable?.Clear();
        _cancellationTokenSource?.Cancel();
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
        _panelTransform.localPosition = _defaultPosition;
        _panelTransform.localScale = Vector3.one;
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
        _panelView.Button.onClick.AddListener(()=>
        {
            _panelView.Button.onClick.RemoveAllListeners();
            onPress.Invoke();
        });
    }
    public async UniTask DoAnimPanel(float duration, float startAlpha, float endValue, bool keyActive)
    {
        _panelView.CanvasGroup.alpha = startAlpha;
        if (keyActive == true)
        {
            _panelView.gameObject.SetActive(keyActive);
            _panelView.CanvasGroup.blocksRaycasts = true;
        }
        await _panelView.CanvasGroup.DOFade(endValue, duration).WithCancellation(_cancellationTokenSource.Token);
        if (keyActive == false)
        {
            _panelView.gameObject.SetActive(keyActive);
            _panelView.CanvasGroup.blocksRaycasts = false;
        }
    }
    public void DoPanel(float alpha, bool keyActive)
    {
        _panelView.CanvasGroup.alpha = alpha;
        if (keyActive == true)
        {
            _panelView.CanvasGroup.blocksRaycasts = true;
        }
        else
        {
            _panelView.CanvasGroup.blocksRaycasts = false;
        }
        _panelView.gameObject.SetActive(keyActive);
    }
}