using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using XNode;

public class MergerNode : BaseNode
{
    [SerializeField, HideInInspector] private bool _autoSwitchToNextSlide;
    
    private readonly int _maxDynamicPortsCount = 4;
    private ChoiceNode _choiceNode;
    private SwitchNode _switchNode;
    private CustomizationNode _customizationNode;
    private TaskRunner _taskRunner;
    private List<BaseNode> _mergedNodes;
    
    private List<string> _names;

    private bool _choiceNodeConnected;
    private bool _switchNodeConnected;
    private bool _smoothTransitionNodeConnected;
    private bool _customizationNodeConnected;

    public void ConstructMyMergerNode()
    {
        _taskRunner = new TaskRunner();
        _choiceNodeConnected = false;
        _switchNodeConnected = false;
        _smoothTransitionNodeConnected = false;
        _customizationNodeConnected = false;
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        _mergedNodes = new List<BaseNode>();
        foreach (NodePort port in DynamicOutputs)
        {
            if (port.IsConnected)
            {
                if (_choiceNodeConnected == false && port.Connection.node is ChoiceNode choiceNode)
                {
                    _choiceNode = choiceNode;
                    _choiceNodeConnected = true;
                }
                else if (_switchNodeConnected == false && port.Connection.node is SwitchNode switchNode)
                {
                    _switchNode = switchNode;
                    _switchNodeConnected = true;
                }
                else if (_smoothTransitionNodeConnected == false && port.Connection.node is SmoothTransitionNode)
                {
                    _smoothTransitionNodeConnected = true;
                }
                else if (_customizationNodeConnected == false && port.Connection.node is CustomizationNode customizationNode)
                {
                    _customizationNode = customizationNode;
                    _customizationNodeConnected = true;
                }
                _mergedNodes.Add(port.Connection.node as BaseNode);
                if (port.Connection.node is BackgroundNode backgroundNode)
                {
                    _mergedNodes.Insert(0, backgroundNode);
                }
            }
        }
        if (_choiceNodeConnected == true)
        {
            ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
        }
        else if (_customizationNodeConnected == false)
        {
            TrySetNextNodeFromFirstConnectedPort();
            ButtonSwitchSlideUIHandler.ActivateSkipTransition(()=>
            {
                SkipEnterTransition();
                if (_smoothTransitionNodeConnected == false)
                {
                    TryActivateButtonSwitchToNextSlide();
                }
            });
        }

        await _taskRunner.TryRunTasks(CreateTasksEnteredList());
        TryActivateButtonSwitchToNextSlide();
    }

    public override async UniTask Exit()
    {
        if (_choiceNodeConnected == true)
        {
            SetNextNode(_choiceNode.GetNextNode());
        }
        else if (_switchNodeConnected == true)
        {
            SetNextNode(_switchNode.GetNextNode());
        }
        else if (_customizationNode == true)
        {
            SetNextNode(_customizationNode.GetNextNode());
        }

        ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipExitTransition);
        await _taskRunner.TryRunTasks(CreateTasksExitedList());
    }

    private List<Func<UniTask>> CreateTasksExitedList()
    {
        List<Func<UniTask>> tasksExited = new List<Func<UniTask>>(_maxDynamicPortsCount);

        foreach (BaseNode mergedNode in _mergedNodes)
        {
            tasksExited.Add(() => mergedNode.Exit());
        }
        return tasksExited;
    }

    private List<Func<UniTask>> CreateTasksEnteredList()
    {
        List<Func<UniTask>> tasksEntered = new List<Func<UniTask>>(_maxDynamicPortsCount);
        foreach (BaseNode mergedNode in _mergedNodes)
        {
            tasksEntered.Add(() => mergedNode.Enter(true));
        }

        return tasksEntered;
    }
    public override void SkipEnterTransition()
    {
        foreach (BaseNode baseNode in _mergedNodes)
        {
            baseNode.SkipEnterTransition();
        }
    }

    public override void SkipExitTransition()
    {
        foreach (BaseNode baseNode in _mergedNodes)
        {
            baseNode.SkipExitTransition();
        }
    }

    protected override void TryActivateButtonSwitchToNextSlide()
    {
        if (_choiceNodeConnected == false && _customizationNodeConnected == false)
        {
            if (_autoSwitchToNextSlide == true)
            {
                SwitchToNextNodeEvent.Execute();
            }
            else
            {
                ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
            }
        }
    }
    protected override void SetInfoToView()
    {
        BaseNode node;
        foreach (NodePort port in DynamicOutputs)
        {
            if (port.IsConnected)
            {
                node = port.Connection.node as BaseNode;
                node.ShowMergerNodeContent();
            }
        }
    }
    private void TrySetNextNodeFromFirstConnectedPort()
    {
        NodePort portOutputNextNode = null;
        foreach (BaseNode node in _mergedNodes)
        {
            portOutputNextNode = node.OutputPortBaseNode;
            if (portOutputNextNode != null && portOutputNextNode.IsConnected == true)
            {
                SetNextNode(portOutputNextNode.Connection.node as BaseNode);
                break;
            }
        }
    }
    private void AddDynamicPort()
    {
        if (DynamicOutputs.Count() < _maxDynamicPortsCount)
        {
            if (_names == null)
            {
                _names = new List<string>();
            }
            string name = $"Port {DynamicOutputs.Count()}";
            _names.Add(name);
            AddDynamicOutput(typeof(Empty), ConnectionType.Override, fieldName: name);
        }
    }

    private void RemovePorts()
    {
        if (DynamicOutputs.Count() > 0)
        {
            RemoveDynamicPort(DynamicOutputs.Last().fieldName);
        }
    }

    // private async UniTask TryRunTasks(List<Func<UniTask>> tasksEntered)
    // {
    //     switch (tasksEntered.Count)
    //     {
    //         case 1 :
    //             await UniTask.WhenAll(tasksEntered[0].Invoke());
    //             break;
    //         case 2 :
    //             await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke());
    //             break;
    //         case 3 :
    //             await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke());
    //             break;
    //         case 4 :
    //             await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke());
    //             break;
    //     }
    // }
}