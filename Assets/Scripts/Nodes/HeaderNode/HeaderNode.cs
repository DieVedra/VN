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
    [SerializeField] private string _headerAudioKey;
    [SerializeField] private string _headerAmbientAudioKey;
    [SerializeField] private bool _playHeaderAudio;
    [SerializeField] private bool _playHeaderAmbientAudio;
    
    private string _previousKeyBackground;
    private HeaderSeriesPanelHandlerUI _headerSeriesPanelHandlerUI;
    private CurtainUIHandler _curtainUIHandler;
    private ButtonSwitchSlideUIHandler _buttonSwitchSlideUIHandler;
    private IBackgroundsProviderToHeaderNode _background;
    private CompositeDisposable _compositeDisposable;
    private NewTaskRunner _soundTaskRunner;
    public ISoundProviderToHeaderNode Sound { get; private set; }
    public IReadOnlyDictionary<string, BackgroundContentValues> BackgroundsDictionary =>
        _background?.GetBackgroundContentDictionary;

    public void Construct(IBackgroundsProviderToHeaderNode provider, HeaderSeriesPanelHandlerUI headerSeriesPanelHandlerUI,
        CurtainUIHandler curtainUIHandler,ButtonSwitchSlideUIHandler buttonSwitchSlideUIHandler, ISoundProviderToHeaderNode sound)
    {
        _soundTaskRunner = sound.SmoothAudio.SoundTaskRunner;
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

        var list = _soundTaskRunner.GetFreeList();
        list.Add(()=> _curtainUIHandler.CurtainOpens(CancellationTokenSource.Token));
        if (_playHeaderAudio)
        {
            list.Add(()=> Sound.SmoothPlayHeaderAudio(_headerAudioKey, CancellationTokenSource.Token));
        }

        if (_playHeaderAmbientAudio)
        {
            list.Add(()=> Sound.SmoothPlayHeaderAudio(_headerAmbientAudioKey, CancellationTokenSource.Token));
        }
        _soundTaskRunner.AddToQueue(list);
        await _soundTaskRunner.TryRun();
        _buttonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
    }

    public override async UniTask Exit()
    {
        CancellationTokenSource = new CancellationTokenSource();
        var list = _soundTaskRunner.GetFreeList();
        list.Add(()=> _curtainUIHandler.CurtainCloses(CancellationTokenSource.Token));

        if (_playHeaderAudio)
        {
            list.Add(()=> Sound.SmoothAudio.SmoothStopAudio(CancellationTokenSource.Token, AudioSourceType.Music));
        }

        if (_playHeaderAmbientAudio)
        {
            list.Add(()=> Sound.SmoothAudio.SmoothStopAudio(CancellationTokenSource.Token, AudioSourceType.Ambient));
        }
        _soundTaskRunner.AddToQueue(list);
        await _soundTaskRunner.TryRun();
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