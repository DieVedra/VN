using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "SeriaPartNodeGraph", menuName = "NodeGraphs/SeriaPartNodeGraph", order = 51)]
public class SeriaPartNodeGraph : NodeGraph
{
	private BaseNode _currentNode;
	private int _currentNodeIndex;
	private int _currentSeriaIndex;
	private List<BaseNode> _baseNodes;
	private NodeGraphInitializer _nodeGraphInitializer;
	private ReactiveProperty<bool> _putOnSwimsuitKey;
	public int CurrentNodeIndex => nodes.IndexOf(_currentNode);
	
	public void Init(ReactiveProperty<bool> putOnSwimsuitKey, NodeGraphInitializer nodeGraphInitializer, int currentSeriaIndex = 0, int currentNodeIndex = 0)
	{
		_putOnSwimsuitKey = putOnSwimsuitKey;
		_nodeGraphInitializer = nodeGraphInitializer;
		_currentNodeIndex = currentNodeIndex;
		_currentSeriaIndex = currentSeriaIndex;
		PutOnSwimsuit(_putOnSwimsuitKey.Value);
		TryInitNodes();

		if (Application.isPlaying == false)
		{
			OnChangeGraph += InitNewNode;
		}
		else
		{
			_currentNode.Enter().Forget();
		}
	}

	public void Dispose()
	{
		if (_baseNodes != null && _baseNodes.Count > 0)
		{
			foreach (var baseNodes in _baseNodes)
			{
				baseNodes?.Dispose();
			}
		}
	}

	public async UniTaskVoid MoveNext()
	{
		await _currentNode.Exit();
		_currentNode = _currentNode.GetNextNode();
		await _currentNode.Enter();
	}

	public void SetKeyPutOnSwimsuit(bool putOnSwimsuit)
	{
		_putOnSwimsuitKey.Value = putOnSwimsuit;
	}
	private void PutOnSwimsuit(bool putOnSwimsuit)
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			if (nodes[i] is PutOnSwimsuitsNode putOnSwimsuitsNode)
			{
				putOnSwimsuitsNode.Init(putOnSwimsuit);
			}
		}
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
			_nodeGraphInitializer.Init(_baseNodes, _currentSeriaIndex);
			_currentNode = _baseNodes[_currentNodeIndex];
		}
	}

	private void InitNewNode(Node node)
	{
		_nodeGraphInitializer.InitOneNode(node as BaseNode, _currentSeriaIndex);
	}
}