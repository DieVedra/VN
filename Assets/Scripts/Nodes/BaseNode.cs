using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using XNode;

using UnityEngine;

public class BaseNode : Node
{
    [Input] public Empty Input;
    [Output] public Empty Output;

    protected ButtonSwitchSlideUIHandler ButtonSwitchSlideUIHandler;
    protected SwitchToNextNodeEvent SwitchToNextNodeEvent;
    protected bool IsMerged;

    protected DisableNodesContentEvent DisableNodesContentEvent;
    protected CancellationTokenSource CancellationTokenSource;
    public string namenode;
    protected LocalizationString[] StringsToLocalization;
    public NodePort OutputPortBaseNode => GetOutputPort("Output");
    public IReadOnlyList<LocalizationString> StringsLocalization => StringsToLocalization;
    private BaseNode _nextNode;

    public void ConstructBaseNode(ButtonSwitchSlideUIHandler buttonSwitchSlideUIHandler, SwitchToNextNodeEvent switchToNextNodeEvent,
        DisableNodesContentEvent disableNodesContentEvent)
    {
        ButtonSwitchSlideUIHandler = buttonSwitchSlideUIHandler;
        DisableNodesContentEvent = disableNodesContentEvent;
        SwitchToNextNodeEvent = switchToNextNodeEvent;
        _nextNode = null;
    }

    public virtual void Dispose()
    {
        CancellationTokenSource?.Cancel();
    }
    public void SetNextNode(BaseNode nextNode)
    {
        _nextNode = nextNode;
    }
    public BaseNode GetNextNode()
    {
        if (_nextNode == null)
        {
            NodePort nodePort = OutputPortBaseNode;
            if (nodePort.IsConnected == true)
            {
                _nextNode = nodePort.Connection.node as BaseNode;
                if (_nextNode == null)
                {
                    _nextNode = this;
                }
            }
            else
            {
                _nextNode = this;
            }
        }

        return _nextNode;
    }

    protected override void Init()
    {
        if (IsPlayMode() == false)
        {
            base.Init();
            OnSelectNode += DisableAllNodeView;
            OnSelectNode += SetInfoToView;
            OnShowMergerNodeContent += SetInfoToView;
        }
    }

    protected virtual void SetInfoToView() { }

    protected virtual void TryActivateButtonSwitchToNextSlide(){}
    public virtual void SkipEnterTransition(){}
    public virtual void SkipExitTransition(){}

    public virtual async UniTask Enter(bool isMerged = false)
    {
        SwitchToNextNodeEvent.Execute();
    }

    public virtual async UniTask Exit() { }

    private void DisableAllNodeView()
    {
        DisableNodesContentEvent?.Execute();
    }

    protected bool IsPlayMode()
    {
        if (Application.isPlaying)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void InitStringsToLocalization(params LocalizationString[] strings)
    {
        StringsToLocalization = strings;
    }
}
