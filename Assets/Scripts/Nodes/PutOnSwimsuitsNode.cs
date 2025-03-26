using System.Collections.Generic;
using XNode;

public class PutOnSwimsuitsNode : BaseNode
{ 
        public void Init(bool key)
        {
                List<NodePort> _connections = GetOutputPort("Output").GetConnections();
                if (_connections != null)
                {
                        foreach (NodePort port in _connections)
                        {
                                if (port.node is IPutOnSwimsuit putOnSwimsuit)
                                {
                                        putOnSwimsuit.PutOnSwimsuit(key);
                                }
                        }
                }
        }
}