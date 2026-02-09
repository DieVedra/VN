using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using XNode;

public class MergerNode : BaseNode
{
    [SerializeField] private bool _autoSwitchToNextSlide;
    [SerializeField] private AsyncMode _enterAsyncMode;
    [SerializeField] private AsyncMode _exitAsyncMode;
    
    private const int _maxDynamicPortsCount = 4;
    private const string _port = "Port ";
    private MergedNodeSharedStorage _mergedNodeSharedStorage;
    private MergedNodesDeterminator _mergedNodesDeterminator;
    private List<string> _names;
    private bool _choiceNodeConnected => _mergedNodeSharedStorage.MergerObjects.ContainsKey(typeof(ChoiceNode));
    private bool _customizationNodeConnected => _mergedNodeSharedStorage.MergerObjects.ContainsKey(typeof(CustomizationNode));
    private bool _switchNodeConnected => _mergedNodeSharedStorage.MergerObjects.ContainsKey(typeof(SwitchNode));
    private bool _smoothTransitionNodeConnected => _mergedNodeSharedStorage.MergerObjects.ContainsKey(typeof(SmoothTransitionNode));

    

    public void ConstructMyMergerNode(MergedNodeSharedStorage mergedNodeSharedStorage, MergedNodesDeterminator mergedNodesDeterminator)
    {
        _mergedNodeSharedStorage = mergedNodeSharedStorage;
        _mergedNodesDeterminator = mergedNodesDeterminator;
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        ButtonSwitchSlideUIHandler.DeactivatePushOption();
        _mergedNodeSharedStorage.Clear();
        foreach (NodePort port in DynamicOutputs)
        {
            if (port.IsConnected)
            {
                var keyValue = _mergedNodesDeterminator.TryDetermineNode(port);
                if (_mergedNodeSharedStorage.MergerObjects.ContainsKey(keyValue.Key) == false)
                {
                    _mergedNodeSharedStorage.MergerObjects.Add(keyValue.Key, keyValue.Value);
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

        await RunTasksEntered();
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
        foreach (var mergerObject in _mergedNodeSharedStorage.MergerObjects)
        {
            _mergedNodeSharedStorage.TaskList.Add(GetBaseNode(mergerObject.Value).Exit());
        }
        await KeepTasks(_exitAsyncMode);
        _mergedNodeSharedStorage.TaskList.Clear();
    }

    private async UniTask RunTasksEntered()
    {
        foreach (var mergerObject in _mergedNodeSharedStorage.MergerObjects)
        {
            _mergedNodeSharedStorage.TaskList.Add(GetBaseNode(mergerObject.Value).Enter(true));
        }
        await KeepTasks(_enterAsyncMode);
        _mergedNodeSharedStorage.TaskList.Clear();
    }

    private async UniTask KeepTasks(AsyncMode asyncMode)
    {
        switch (asyncMode)
        {
            case AsyncMode.WhenAll:
                await UniTask.WhenAll(_mergedNodeSharedStorage.TaskList);
                break;
            case AsyncMode.WhenAny:
                await UniTask.WhenAny(_mergedNodeSharedStorage.TaskList);
                break;
        }
    }

    public override void SkipEnterTransition()
    {
        foreach (var mergerObject in _mergedNodeSharedStorage.MergerObjects)
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
        foreach (var mergerObject in _mergedNodeSharedStorage.MergerObjects)
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
        if (_mergedNodeSharedStorage.MergerObjects.TryGetValue(typeof(BackgroundNode), out Node baseNode))
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
        if (_mergedNodeSharedStorage.MergerObjects.TryGetValue(typeof(T), out Node baseNode))
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