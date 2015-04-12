using UnityEngine;
using System.Collections;

public class PathResult
{

	public PathNode[] nodes;
	private int currentNode;

	public PathResult (PathNode[] nodes)
	{
		this.nodes = nodes;
		currentNode = nodes.Length - 1;
	}

	public PathNode AdvanceToNextNode ()
	{
		currentNode--;
		if (currentNode < 0) {
			return null;
		} else {
			return nodes [currentNode];
		}
	}

	public PathNode GetCurrentNode ()
	{
		return currentNode >= 0 ? nodes [currentNode] : null;
	}
}
