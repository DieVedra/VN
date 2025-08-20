using System;
using System.Collections.Generic;
using XNode;

public class MergedNodesDeterminator
{
    private Dictionary<Type, Node> _mergerObjects;

    public MergedNodesDeterminator(ref Dictionary<Type, Node> mergerObjects)
    {
        _mergerObjects = mergerObjects;
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
        var newDic = new Dictionary<Type, Node>();
        if (_mergerObjects.TryGetValue(type, out Node baseNode))
        {
            newDic.Add(type, baseNode);
            _mergerObjects.Remove(type);
        }
        foreach (var obj in _mergerObjects)
        {
            newDic.Add(obj.Key, obj.Value);
        }
        _mergerObjects = newDic;
    }
}