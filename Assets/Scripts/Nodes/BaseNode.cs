using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEditor;
using XNode;
using UnityEngine;

public class BaseNode : Node
{
    [Input] public Empty Input;
    [Output] public Empty Output;
    [SerializeField] public string namenode;

    private const string _nameOutputPort = "Output";
    protected CancellationTokenSource CancellationTokenSource;
    protected ButtonSwitchSlideUIHandler ButtonSwitchSlideUIHandler { get; private set; }
    protected SwitchToNextNodeEvent SwitchToNextNodeEvent { get; private set; }
    protected SetLocalizationChangeEvent SetLocalizationChangeEvent { get; private set; }
    protected DisableNodesContentEvent DisableNodesContentEvent { get; private set; }
    protected bool IsMerged;

    public NodePort OutputPortBaseNode => GetOutputPort(_nameOutputPort);
    private BaseNode _nextNode;

    public void ConstructBaseNode(ButtonSwitchSlideUIHandler buttonSwitchSlideUIHandler, SwitchToNextNodeEvent switchToNextNodeEvent,
        DisableNodesContentEvent disableNodesContentEvent, SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        ButtonSwitchSlideUIHandler = buttonSwitchSlideUIHandler;
        DisableNodesContentEvent = disableNodesContentEvent;
        SwitchToNextNodeEvent = switchToNextNodeEvent;
        SetLocalizationChangeEvent = setLocalizationChangeEvent;
        _nextNode = null;
    }

    public virtual void Dispose()
    {
        CancellationTokenSource?.Cancel();
    }

    protected void SetNextNode(BaseNode nextNode)
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
        base.Init();
#if UNITY_EDITOR
        OnSelectNode += DisableAllNodeView;
        OnSelectNode += SetInfoToView;
        OnShowMergerNodeContent += SetInfoToView;
#endif
        
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
}
