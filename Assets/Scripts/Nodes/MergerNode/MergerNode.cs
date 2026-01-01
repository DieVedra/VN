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
    private MergedNodesDeterminator _mergedNodesDeterminator;
    private List<string> _names;
    private List<Func<UniTask>> _tasks;
    private Dictionary<Type, Node> _mergerObjects;

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
        ButtonSwitchSlideUIHandler.DeactivatePushOption();
        _mergerObjects = new Dictionary<Type, Node>();
        _mergedNodesDeterminator = new MergedNodesDeterminator(ref _mergerObjects);
        foreach (NodePort port in DynamicOutputs)
        {
            if (port.IsConnected)
            {
                var keyValue = _mergedNodesDeterminator.TryDetermineNode(port);
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
        ButtonSwitchSlideUIHandler.DeactivatePushOption();
        TryActivateButtonSwitchToNextSlide();
    }

    public override BaseNode GetNextNode()
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
        var a = base.GetNextNode();
        return a;
    }

    public override async UniTask Exit()
    {
        await _taskRunner.TryRunTasks(CreateTasksExitedList());
        _tasks = null;
    }

    private List<Func<UniTask>> CreateTasksExitedList()
    {
        if (_tasks == null)
        {
            _tasks = new List<Func<UniTask>>(_maxDynamicPortsCount);
        }
        else
        {
            _tasks.Clear();
        }
        foreach (var mergerObject in _mergerObjects)
        {
            _tasks.Add(() => GetBaseNode(mergerObject.Value).Exit());
        }
        return _tasks;
    }

    private List<Func<UniTask>> CreateTasksEnteredList()
    {
        _tasks = new List<Func<UniTask>>(_maxDynamicPortsCount);
        foreach (var mergerObject in _mergerObjects)
        {
            _tasks.Add(() => GetBaseNode(mergerObject.Value).Enter(true));
        }
        return _tasks;
    }
    public override void SkipEnterTransition()
    {
        foreach (var mergerObject in _mergerObjects)
        {
            GetBaseNode(mergerObject.Value).SkipEnterTransition();
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
            portOutputNextNode = GetBaseNode(mergerObject.Value).OutputPortBaseNode;
            if (portOutputNextNode != null && portOutputNextNode.IsConnected == true)
            {
                SetNextNode(portOutputNextNode.Connection.node as BaseNode);
                break;
            }
        }
    }
    private bool GetKeySmoothTransitionBackgroundNode()
    {
        bool result = false;
        if (_mergerObjects.TryGetValue(typeof(BackgroundNode), out Node baseNode))
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
        if (_mergerObjects.TryGetValue(typeof(T), out Node baseNode))
        {
            return GetBaseNode(baseNode).GetNextNode();
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

    private BaseNode GetBaseNode(Node node)
    {
        return node as BaseNode;
    }
}