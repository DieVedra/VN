using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeTint("#DB620A")]
public class BackgroundNode : BaseNode
{
    [SerializeField, HideInInspector] private int _index;
    [SerializeField, HideInInspector] private bool _isSmoothCurtain;
    [SerializeField, HideInInspector] private BackgroundPosition _backgroundPosition;
    private Background _background;
    private List<BackgroundContent> _backgrounds;
    public List<BackgroundContent> Backgrounds => _backgrounds;
    public bool IsSmoothCurtain => _isSmoothCurtain;
    public void ConstructBackgroundNode(List<BackgroundContent> backgrounds, Background background)
    {
        _backgrounds = backgrounds;
        _background = background;
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        IsMerged = isMerged;
        if (_isSmoothCurtain == true)
        {
            if (isMerged == false)
            {
                ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
            }
            await _background.SmoothBackgroundChangePosition(CancellationTokenSource.Token, _backgroundPosition, _index);
        }
        else
        {
            SetInfoToView();
        }
        
        
        // Debug.Log($"1");

        TrySwitchToNextNode();
    }

    public override void SkipEnterTransition()
    {
        if (_isSmoothCurtain == true)
        {
            CancellationTokenSource.Cancel();
            SetInfoToView();
        }
        // Debug.Log($"2");

        // TrySwitchToNextNode();

    }

    protected override void SetInfoToView()
    {
        _background.SetBackgroundPosition(_backgroundPosition, _index);
    }
    private void TrySwitchToNextNode()
    {
        if (IsMerged == false)
        {
            SwitchToNextNodeEvent.Execute();
        }
    }
}