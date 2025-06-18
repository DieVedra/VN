using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "SeriaPartNodeGraph", menuName = "NodeGraphs/SeriaPartNodeGraph", order = 51)]
public class SeriaPartNodeGraph : NodeGraph
{
	private BaseNode _currentNode;
	private int _nodeCount = 0;
	private int _currentNodeIndex;
	private int _currentSeriaIndex;
	private List<BaseNode> _baseNodes;
	private List<Stat> _stats;
	private NodeGraphInitializer _nodeGraphInitializer;
	public int CurrentNodeIndex => nodes.IndexOf(_currentNode);
	
	public void Init(NodeGraphInitializer nodeGraphInitializer, List<Stat> stats, int currentSeriaIndex = 0, int currentNodeIndex = 0)
	{
		_nodeGraphInitializer = nodeGraphInitializer;
		_currentNodeIndex = currentNodeIndex;
		_currentSeriaIndex = currentSeriaIndex;
		_nodeCount = nodes.Count;
		_stats = stats;
		TryInitNodes();
		if (Application.isPlaying == false)
		{
			// OnChangeGraph = null;
			OnChangeGraph += InitNewNode;
		}
		else
		{
			nodeGraphInitializer.SendCurrentNodeEvent.Subscribe(SetCurrentNodeAndEnter);
			_currentNode.Enter().Forget();
		}
	}

	public void Dispose()
	{
		if (_baseNodes != null && _baseNodes.Count > 0)
		{
			foreach (var baseNodes in _baseNodes)
			{
				baseNodes.Dispose();
			}
		}
	}

	public async UniTaskVoid MoveNext()
	{
		await _currentNode.Exit();
		_currentNode = _currentNode.GetNextNode();
		await _currentNode.Enter();
	}

	public void TryPutOnSwimsuit(bool putOnSwimsuit)
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			if (nodes[i] is PutOnSwimsuitsNode putOnSwimsuitsNode)
			{
				putOnSwimsuitsNode.Init(putOnSwimsuit);
			}
		}
	}

	private void SetCurrentNodeAndEnter(BaseNode baseNode)
	{
		_currentNode = baseNode;
		_currentNode.Enter().Forget();
	}
	private void TryInitNodes()
	{
		if (nodes.Count > 0)
		{
			_baseNodes = new List<BaseNode>(nodes.Count);
			for (int i = 0; i < nodes.Count; i++)
			{
				if (Application.isPlaying == false)
				{
					nodes[i].MyInit();
				}

				if (nodes[i] is BaseNode baseNodes)
				{
					_baseNodes.Add(baseNodes);
				}

				if (_currentNodeIndex == 0)
				{
					if (nodes[i] is StartNode startNode)
					{
						_currentNodeIndex = nodes.IndexOf(startNode);
					}
				}
			}
			_nodeGraphInitializer.Init(_baseNodes, _stats, _currentSeriaIndex);
			_currentNode = _baseNodes[_currentNodeIndex];
		}
	}

	private void InitNewNode(Node node)
	{
		if (_nodeCount != nodes.Count)
		{
			TryInitNodes();
			_nodeGraphInitializer.InitOneNode(node as BaseNode, _currentSeriaIndex);
			_nodeCount = nodes.Count;
		}
	}
}