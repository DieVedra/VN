using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UniRx;
using UnityEngine;

[NodeTint("#006946"), NodeWidth(200)]
public class HeaderNode : BaseNode, ILocalizable
{
    [SerializeField] private LocalizationString _localizationText1;
    [SerializeField] private Color _colorField1 = Color.white;
    [SerializeField] private int _textSize1 = 140;
    [SerializeField] private LocalizationString _localizationText2;
    [SerializeField] private Color _colorField2 = Color.white;
    [SerializeField] private int _textSize2 = 80;
    [SerializeField] private int _indexBackground;
    [SerializeField] private float _backgroundPositionValue;
    
    
    private int _previousIndexBackground;
    private HeaderSeriesPanelHandlerUI _headerSeriesPanelHandlerUI;
    private CurtainUIHandler _curtainUIHandler;
    private ButtonSwitchSlideUIHandler _buttonSwitchSlideUIHandler;
    private Background _background;
    private CompositeDisposable _compositeDisposable;
    public IReadOnlyList<BackgroundContent> Backgrounds { get; private set; }

    public void Construct(IReadOnlyList<BackgroundContent> backgrounds, Background background, HeaderSeriesPanelHandlerUI headerSeriesPanelHandlerUI,
        CurtainUIHandler curtainUIHandler, ButtonSwitchSlideUIHandler buttonSwitchSlideUIHandler)
    {
        Backgrounds = backgrounds;
        _background = background;
        _headerSeriesPanelHandlerUI = headerSeriesPanelHandlerUI;
        _curtainUIHandler = curtainUIHandler;
        _buttonSwitchSlideUIHandler = buttonSwitchSlideUIHandler;
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        _previousIndexBackground = _background.CurrentIndexBackgroundContent;
        CancellationTokenSource = new CancellationTokenSource();
        _compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetHeaderTexts);
        SetInfoToView();
        IsMerged = isMerged;
        if (isMerged == false)
        {
            _buttonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
        }
        await _curtainUIHandler.CurtainOpens(CancellationTokenSource.Token);
        _buttonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
    }

    public override async UniTask Exit()
    {
        CancellationTokenSource = new CancellationTokenSource();
        if (IsMerged == false)
        {
            _buttonSwitchSlideUIHandler.ActivateSkipTransition(SkipExitTransition);
        }
        await _curtainUIHandler.CurtainCloses(CancellationTokenSource.Token);
        _background.SetBackgroundPosition(_background.CurrentBackgroundPosition, _previousIndexBackground);
        _headerSeriesPanelHandlerUI.OffHeader();
        _compositeDisposable.Dispose();
    }

    protected override void SetInfoToView()
    {
        _background.SetBackgroundPositionFromSlider(_backgroundPositionValue, _indexBackground);
        SetHeaderTexts();
    }

    private void SetHeaderTexts()
    {
        _headerSeriesPanelHandlerUI.SetHeader(_localizationText1, _localizationText2,
            _colorField1, _colorField2,
            _textSize1, _textSize2);
    }

    public override void SkipEnterTransition()
    {
        CancellationTokenSource.Cancel();
        _buttonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
        _curtainUIHandler.SkipAtOpens();
    }

    public override void SkipExitTransition()
    {
        CancellationTokenSource.Cancel();
        _curtainUIHandler.SkipAtCloses();
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[]
        {
            _localizationText1, _localizationText2
        };;
    }
}