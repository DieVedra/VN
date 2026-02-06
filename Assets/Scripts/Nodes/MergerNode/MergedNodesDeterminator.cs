using System;
using System.Collections.Generic;
using System.Linq;
using XNode;

public class MergedNodesDeterminator
{
    private readonly MergedNodeSharedStorage _mergedNodeSharedStorage;
    private Dictionary<Type, Node> _newDic;
    public MergedNodesDeterminator(MergedNodeSharedStorage mergedNodeSharedStorage)
    {
        _mergedNodeSharedStorage = mergedNodeSharedStorage;
        _newDic = new Dictionary<Type, Node>();
    }

    public KeyValuePair<Type, Node> TryDetermineNode(NodePort port)
    {
        var node = port.Connection.node;
        KeyValuePair<Type, Node> result; 
        switch (node)
        {
            case ChoiceNode choiceNode:
                result = new KeyValuePair<Type, Node>(typeof(ChoiceNode), choiceNode);
                break;
            
            case SwitchNode switchNode:
                result = new KeyValuePair<Type, Node>(typeof(SwitchNode), switchNode);
                break;
            
            case SmoothTransitionNode smoothTransitionNode:
                result = new KeyValuePair<Type, Node>(typeof(SmoothTransitionNode), smoothTransitionNode);
                break;
            
            case CustomizationNode customizationNode:
                result = new KeyValuePair<Type, Node>(typeof(CustomizationNode), customizationNode);
                break;
            
            case BackgroundNode backgroundNode:
                var type = typeof(BackgroundNode);
                result = new KeyValuePair<Type, Node>(type, backgroundNode);
                SetNodeFirstItem(type);
                break;
            
            default:
                result = new KeyValuePair<Type, Node>(node.GetType(), node);
                break;
        }
        return result;
    }
    private void SetNodeFirstItem(Type type)
    {
        if (_mergedNodeSharedStorage.MergerObjects.TryGetValue(type, out Node baseNode))
        {
            _newDic.Add(type, baseNode);
            _mergedNodeSharedStorage.MergerObjects.Remove(type);
        }
        foreach (var obj in _mergedNodeSharedStorage.MergerObjects)
        {
            _newDic.Add(obj.Key, obj.Value);
        }
        _mergedNodeSharedStorage.MergerObjects.Clear();
        foreach (var pair in _newDic)
        {
            _mergedNodeSharedStorage.MergerObjects.Add(pair.Key, pair.Value);
        }
        _newDic.Clear();
    }
}