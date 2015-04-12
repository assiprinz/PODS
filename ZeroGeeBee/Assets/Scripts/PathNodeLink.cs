using UnityEngine;
using System.Collections;

[System.Serializable]
public class PathNodeLink
{

	public PathNode node1;
	public PathNode node2;

	public PathNode GetOtherNode (PathNode node)
	{
		if (node1 == node) {
			return node2;
		} else {
			return node1;
		}
	}

	public bool HasNode (PathNode node)
	{
		return node1 == node || node2 == node;
	}
}
