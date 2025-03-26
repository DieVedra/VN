
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

[NodeTint("#006946"), NodeWidth(200)]
public class HeaderNode : BaseNode
{
    [SerializeField, TextArea] private string _text1;
    [SerializeField] private Color _colorField1 = Color.white;
    [SerializeField] private int _textSize1 = 140;
    [SerializeField, TextArea] private string _text2;
    [SerializeField] private Color _colorField2 = Color.white;
    [SerializeField] private int _textSize2 = 80;
    [SerializeField, ShowAssetPreview(128,128)] private Sprite _sprite;

    private HeaderSeriesPanelHandlerUI _headerSeriesPanelHandlerUI;
    private CurtainUIHandler _curtainUIHandler;
    private ButtonSwitchSlideUIHandler _buttonSwitchSlideUIHandler;
    
    public void Construct(HeaderSeriesPanelHandlerUI headerSeriesPanelHandlerUI, CurtainUIHandler curtainUIHandler,
        ButtonSwitchSlideUIHandler buttonSwitchSlideUIHandler)
    {
        _headerSeriesPanelHandlerUI = headerSeriesPanelHandlerUI;
        _curtainUIHandler = curtainUIHandler;
        _buttonSwitchSlideUIHandler = buttonSwitchSlideUIHandler;
    }

    public override async UniTask Enter(bool isMerged = false)
    {
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
        _headerSeriesPanelHandlerUI.OffHeader();
    }

    protected override void SetInfoToView()
    {
        _headerSeriesPanelHandlerUI.SetHeader(_sprite, _text1, _text2,
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