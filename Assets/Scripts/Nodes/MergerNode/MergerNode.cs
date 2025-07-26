using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using XNode;

public class MergerNode : BaseNode
{
    [SerializeField, HideInInspector] private bool _autoSwitchToNextSlide;
    
    private const int _maxDynamicPortsCount = 4;
    private const string _port = "Port ";
    private TaskRunner _taskRunner;

    private List<string> _names;
    private Dictionary<Type, BaseNode> _mergerObjects;

    private bool _choiceNodeConnected => _mergerObjects.ContainsKey(typeof(ChoiceNode));
    private bool _customizationNodeConnected => _mergerObjects.ContainsKey(typeof(CustomizationNode));
    private bool _switchNodeConnected => _mergerObjects.ContainsKey(typeof(SwitchNode));
    private bool _smoothTransitionNodeConnected => _mergerObjects.ContainsKey(typeof(SmoothTransitionNode));

    

    public void ConstructMyMergerNode()
    {
        _taskRunner = new TaskRunner();
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        _mergerObjects = new Dictionary<Type, BaseNode>();
        foreach (NodePort port in DynamicOutputs)
        {
            if (port.IsConnected)
            {
                var keyValue = MergedNodesDeterminator.TryDetermineNode(port, SetNodeFirstItem);
                if (_mergerObjects.ContainsKey(keyValue.Key) == false)
                {
                    _mergerObjects.Add(keyValue.Key, keyValue.Value);
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
                
                if (_smoothTransitionNodeConnected == false && GetKeySmoothTransitionBackgroundNode() == false)
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
            SetNextNode(GetNextNodeFrom<ChoiceNode>());
        }
        else if (_switchNodeConnected == true)
        {
            SetNextNode(GetNextNodeFrom<SwitchNode>());
        }
        else if (_customizationNodeConnected == true)
        {
            SetNextNode(GetNextNodeFrom<CustomizationNode>());
        }

        ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipExitTransition);
        await _taskRunner.TryRunTasks(CreateTasksExitedList());
    }

    private List<Func<UniTask>> CreateTasksExitedList()
    {
        List<Func<UniTask>> tasksExited = new List<Func<UniTask>>(_maxDynamicPortsCount);

        foreach (var mergerObject in _mergerObjects)
        {
            tasksExited.Add(() => mergerObject.Value.Exit());
        }
        return tasksExited;
    }

    private List<Func<UniTask>> CreateTasksEnteredList()
    {
        List<Func<UniTask>> tasksEntered = new List<Func<UniTask>>(_maxDynamicPortsCount);
        foreach (var mergerObject in _mergerObjects)
        {
            tasksEntered.Add(() => mergerObject.Value.Enter(true));
        }

        return tasksEntered;
    }
    public override void SkipEnterTransition()
    {
        foreach (var mergerObject in _mergerObjects)
        {
            mergerObject.Value.SkipEnterTransition();
        }
    }

    public override void SkipExitTransition()
    {
        foreach (var mergerObject in _mergerObjects)
        {
            mergerObject.Value.SkipExitTransition();
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
        foreach (var mergerObject in _mergerObjects)
        {
            portOutputNextNode = mergerObject.Value.OutputPortBaseNode;
            if (portOutputNextNode != null && portOutputNextNode.IsConnected == true)
            {
                SetNextNode(portOutputNextNode.Connection.node as BaseNode);
                break;
            }
        }
    }

    private void SetNodeFirstItem(Type type)
    {
        var newDic = new Dictionary<Type, BaseNode>();
        if (_mergerObjects.TryGetValue(type, out BaseNode baseNode))
        {
            newDic.Add(type, baseNode);
            _mergerObjects.Remove(type);
        }
        foreach (var obj in _mergerObjects)
        {
            newDic.Add(obj.Key, obj.Value);
        }
    }

    private bool GetKeySmoothTransitionBackgroundNode()
    {
        bool result = false;
        if (_mergerObjects.TryGetValue(typeof(BackgroundNode), out BaseNode baseNode))
        {
            if (baseNode is BackgroundNode backgroundNode)
            {
                result = backgroundNode.IsSmoothCurtain;
            }
        }

        return result;
    }

    private BaseNode GetNextNodeFrom<T>()
    {
        if (_mergerObjects.TryGetValue(typeof(T), out BaseNode baseNode))
        {
            return baseNode.GetNextNode();
        }
        else
        {
            return null;
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
            string name = $"{_port}{DynamicOutputs.Count()}";
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
}