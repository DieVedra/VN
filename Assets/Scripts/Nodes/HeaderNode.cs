
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

[NodeTint("#006946"), NodeWidth(200)]
public class HeaderNode : BaseNode
{
    [SerializeField] private LocalizationString _localizationText1;
    [SerializeField] private Color _colorField1 = Color.white;
    [SerializeField] private int _textSize1 = 140;
    [SerializeField] private LocalizationString _localizationText2;
    [SerializeField] private Color _colorField2 = Color.white;
    [SerializeField] private int _textSize2 = 80;
    [SerializeField] private int _indexBackground;
    [SerializeField] private float _backgroundPositionValue;
    
    
    [SerializeField, ShowAssetPreview(128,128)] private Sprite _sprite;
    private int _previousIndexBackground;
    private HeaderSeriesPanelHandlerUI _headerSeriesPanelHandlerUI;
    private CurtainUIHandler _curtainUIHandler;
    private ButtonSwitchSlideUIHandler _buttonSwitchSlideUIHandler;
    private Background _background;
    public IReadOnlyList<BackgroundContent> Backgrounds { get; private set; }
    public int hash;

    public void Construct(IReadOnlyList<BackgroundContent> backgrounds, Background background, HeaderSeriesPanelHandlerUI headerSeriesPanelHandlerUI, CurtainUIHandler curtainUIHandler,
        ButtonSwitchSlideUIHandler buttonSwitchSlideUIHandler)
    {
        Backgrounds = backgrounds;
        _background = background;
        _headerSeriesPanelHandlerUI = headerSeriesPanelHandlerUI;
        _curtainUIHandler = curtainUIHandler;
        _buttonSwitchSlideUIHandler = buttonSwitchSlideUIHandler;
        hash = _localizationText1.GetHashCode();
        // TryInitStringsToLocalization(_localizationText1, _localizationText2);
        TryInitStringsToLocalization1(_localizationText1, _localizationText2);
        Debug.Log(666);

    }

    public override async UniTask Enter(bool isMerged = false)
    {
        _previousIndexBackground = _background.CurrentIndexBackgroundContent;
        CancellationTokenSource = new CancellationTokenSource();
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
    }

    protected override void SetInfoToView()
    {
        _background.SetBackgroundPositionFromSlider(_backgroundPositionValue, _indexBackground);
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
}