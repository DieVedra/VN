using System;
using System.Collections.Generic;
using XNode;

public class MergedNodesDeterminator
{
    public static KeyValuePair<Type, BaseNode> TryDetermineNode(NodePort port, Action<Type> operation)
    {
        var node = port.Connection.node;
        KeyValuePair<Type, BaseNode> result; 
        switch (node)
        {
            case ChoiceNode choiceNode:
                result = new KeyValuePair<Type, BaseNode>(typeof(ChoiceNode), choiceNode);
                break;
            
            case SwitchNode switchNode:
                result = new KeyValuePair<Type, BaseNode>(typeof(SwitchNode), switchNode);
                break;
            
            case SmoothTransitionNode smoothTransitionNode:
                result = new KeyValuePair<Type, BaseNode>(typeof(SmoothTransitionNode), smoothTransitionNode);
                break;
            
            case CustomizationNode customizationNode:
                result = new KeyValuePair<Type, BaseNode>(typeof(CustomizationNode), customizationNode);
                break;
            
            case BackgroundNode backgroundNode:
                var type = typeof(BackgroundNode);
                result = new KeyValuePair<Type, BaseNode>(type, backgroundNode);
                operation.Invoke(type);
                break;
            
            default:
                result = new KeyValuePair<Type, BaseNode>(typeof(BaseNode), node as BaseNode);
                break;
        }

        return result;
    }
}