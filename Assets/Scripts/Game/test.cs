using NaughtyAttributes;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private SeriaPartNodeGraph _seriaPartNodeGraph;
    [Button()]
    private void test1()
    {
        foreach (var node in _seriaPartNodeGraph.nodes)
        {
            if (node is MergerNode mergerNode)
            {
                foreach (var port in mergerNode.DynamicOutputs)
                {
                    if (port.Connection == null)
                    {
                        Debug.Log($"port.Connection == null   Index MergerNode: {_seriaPartNodeGraph.nodes.IndexOf(mergerNode)}");

                    }
                    else if(port.Connection.node is SmoothTransitionNode stn)
                    {
                        Debug.Log($"Index MergerNode: {_seriaPartNodeGraph.nodes.IndexOf(mergerNode)}");
                    }
                }
            }
        }
        
    }
    
    [Button()]
    private void test2()
    {
        
    }
}