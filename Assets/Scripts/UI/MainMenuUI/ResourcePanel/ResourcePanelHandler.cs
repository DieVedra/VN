using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class ResourcePanelHandler
{
    private const float _posXValueWithAddButtonMode = -112f;
    private const float _posXValueWithoutAddButtonMode = -29f;
    private readonly ResourcePanelPrefabProvider _resourcePanelPrefabProvider;
    private Transform _parentDefault;
    private RectTransform _panelTransform;
    private RectTransform _textTransform;
    private ResourcePanelView _panelView;
    private CompositeDisposable _compositeDisposable;
    private ResourcePanelMode _currentMode;
    public RectTransform PanelTransform => _panelTransform;
    public bool IsInited { get; private set; }
    public event Action<ResourcePanelMode, int> OnResize;

    public ResourcePanelHandler(ResourcePanelPrefabProvider resourcePanelPrefabProvider)
    {
        _resourcePanelPrefabProvider = resourcePanelPrefabProvider;
    }

    public async UniTask Init(Transform parentDefault, IReactiveCommand<int> resourceChanged, int value, Color panelColor, Color panelButtonColor)
    {
        if (IsInited == false)
        {
            IsInited = true;
            _parentDefault = parentDefault;
            _panelView = await _resourcePanelPrefabProvider.CreateAsset(parentDefault);
            _panelTransform = _panelView.transform as RectTransform;
            _textTransform = _panelView.Text.transform as RectTransform;
            _panelView.Button.image.color = panelButtonColor;
            _panelView.Panel.color = panelColor;
            
            SetParentDefault();
            SetValue(value);
            _compositeDisposable = new CompositeDisposable();
            _panelView.gameObject.SetActive(true);
            _parentDefault.gameObject.SetActive(true);
            resourceChanged.Subscribe(_=>
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
}