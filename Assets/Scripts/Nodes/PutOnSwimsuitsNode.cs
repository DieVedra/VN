using System.Collections.Generic;
using UnityEngine;
using XNode;

public class PutOnSwimsuitsNode : BaseNode
{
        public void Init(bool key)
        {
                List<NodePort> connections = GetOutputPort("Output").GetConnections();
                if (connections != null)
                {
                        foreach (NodePort port in connections)
                        {
                                if (port.node is IPutOnSwimsuit putOnSwimsuit)
                                {
                                        putOnSwimsuit.PutOnSwimsuit(key);
                                }
                        }
                }
        }
}