using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
    [SerializeField] private string _keyBackground;
    [SerializeField] private float _backgroundPositionValue;
    [SerializeField] private int _indexHeaderAudio;
    [SerializeField] private bool _playHeaderAudio;
    
    
    private string _previousKeyBackground;
    private HeaderSeriesPanelHandlerUI _headerSeriesPanelHandlerUI;
    private CurtainUIHandler _curtainUIHandler;
    private ButtonSwitchSlideUIHandler _buttonSwitchSlideUIHandler;
    private IBackgroundsProviderToHeaderNode _background;
    private CompositeDisposable _compositeDisposable;
    
    public Sound Sound { get; private set; }
    public IReadOnlyList<BackgroundContent> Backgrounds => _background?.GetBackgroundContent;

    public IReadOnlyDictionary<string, BackgroundContent> BackgroundsDictionary =>
        _background?.GetBackgroundContentDictionary;

    public void Construct(IBackgroundsProviderToHeaderNode provider, HeaderSeriesPanelHandlerUI headerSeriesPanelHandlerUI,
        CurtainUIHandler curtainUIHandler,ButtonSwitchSlideUIHandler buttonSwitchSlideUIHandler, Sound sound)
    {
        _background = provider;
        _headerSeriesPanelHandlerUI = headerSeriesPanelHandlerUI;
        _curtainUIHandler = curtainUIHandler;
        Sound = sound;
        _buttonSwitchSlideUIHandler = buttonSwitchSlideUIHandler;
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        _previousKeyBackground = _background.CurrentKeyBackgroundContent;

        CancellationTokenSource = new CancellationTokenSource();
        _compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetHeaderTexts);
        SetInfoToView();
        IsMerged = isMerged;
        if (isMerged == false)
        {
            _buttonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
        }

        if (_playHeaderAudio)
        {
            await UniTask.WhenAll(
                _curtainUIHandler.CurtainOpens(CancellationTokenSource.Token),
                Sound.SmoothPlayHeaderAudio(_indexHeaderAudio, CancellationTokenSource.Token));
        }
        else
        {
            await _curtainUIHandler.CurtainOpens(CancellationTokenSource.Token);
        }
        _buttonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
    }

    public override async UniTask Exit()
    {
        CancellationTokenSource = new CancellationTokenSource();
        if (_playHeaderAudio)
        {
            await UniTask.WhenAll(_curtainUIHandler.CurtainCloses(CancellationTokenSource.Token),
                Sound.SmoothAudio.SmoothStopAudio(CancellationTokenSource.Token, AudioSourceType.Music));
        }
        else
        {
            await _curtainUIHandler.CurtainCloses(CancellationTokenSource.Token);
        }
        _background.SetBackgroundPosition(_background.CurrentBackgroundPosition, _previousKeyBackground);
        _headerSeriesPanelHandlerUI.OffHeader();
        _compositeDisposable.Dispose();
    }

    protected override void SetInfoToView()
    {
        _background.SetBackgroundPositionFromSlider(_backgroundPositionValue, _keyBackground);
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

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[]
        {
            _localizationText1, _localizationText2
        };;
    }
}