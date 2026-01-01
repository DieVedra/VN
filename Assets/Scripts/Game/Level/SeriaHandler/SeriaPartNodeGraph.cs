using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "SeriaPartNodeGraph", menuName = "NodeGraphs/SeriaPartNodeGraph", order = 51)]
public class SeriaPartNodeGraph : NodeGraph
{
	private BaseNode _currentNode;
	private BaseNode _toSaveNode;
	private int _currentNodeIndex;
	private int _currentSeriaIndex;
	private List<BaseNode> _baseNodes;
	private NodeGraphInitializer _nodeGraphInitializer;
	public int CurrentNodeIndex => nodes.IndexOf(_currentNode);
	public int NodeIndexToSave => nodes.IndexOf(_toSaveNode);
	public bool PutOnSwimsuitKey { get; private set; }
	
	public void Init(NodeGraphInitializer nodeGraphInitializer, int currentSeriaIndex = 0, int currentNodeIndex = 0)
	{
		_nodeGraphInitializer = nodeGraphInitializer;
		_currentNodeIndex = currentNodeIndex;
		_currentSeriaIndex = currentSeriaIndex;
		PutOnSwimsuit();
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

	public void Shutdown()
	{
		if (_baseNodes != null && _baseNodes.Count > 0)
		{
			foreach (var baseNodes in _baseNodes)
			{
				baseNodes.Shutdown();
			}
		}
	}

	public async UniTaskVoid MoveNext()
	{
		_toSaveNode = _currentNode.GetNextNode();
		await _currentNode.Exit();
		_currentNode = _toSaveNode;
		await _currentNode.Enter();
	}

	public void SetKeyPutOnSwimsuit(bool putOnSwimsuit)
	{
		PutOnSwimsuitKey = putOnSwimsuit;
	}
	private void PutOnSwimsuit()
	{
		foreach (var t in nodes)
		{
			if (t is PutOnSwimsuitsNode putOnSwimsuitsNode)
			{
				putOnSwimsuitsNode.Init(PutOnSwimsuitKey);
				break;
			}
		}
	}
	private void TryInitNodes()
	{
		if (nodes.Count > 0)
		{
			_baseNodes = new List<BaseNode>(nodes.Count);
			foreach (var t in nodes)
			{
				if (Application.isPlaying == false)
				{
					t.MyInit();
				}

				if (t is BaseNode baseNodes)
				{
					_baseNodes.Add(baseNodes);
				}

				if (_currentNodeIndex == 0)
				{
					if (t is StartNode startNode)
					{
						_currentNodeIndex = nodes.IndexOf(startNode);
					}
				}
			}
			_nodeGraphInitializer.Init(_baseNodes, _currentSeriaIndex);
			_toSaveNode = _currentNode = _baseNodes[_currentNodeIndex];
		}
	}

	private void InitNewNode(Node node)
	{
		_nodeGraphInitializer.InitOneNode(node as BaseNode, _currentSeriaIndex);
	}
}