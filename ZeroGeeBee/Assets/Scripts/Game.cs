using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{

	public PathNode[] pathNodes;
	public PathNodeLink[] pathNodeLinks;

	// Use this for initialization
	void Awake ()
	{

		foreach (PathNode node in pathNodes) {
			List<PathNodeLink> linksToNode = new List<PathNodeLink> ();
			foreach (PathNodeLink link in pathNodeLinks) {
				if (link.HasNode (node)) {
					linksToNode.Add (link);
				}
			}
			node.nodeLinks = linksToNode.ToArray ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public Vector3 GetClosestPathIntersection (Vector3 position)
	{
		float shortestPathMagnitude = float.PositiveInfinity;
		Vector3 shortestPathIntersection = Vector3.zero;

		foreach (PathNodeLink nodeLink in pathNodeLinks) {
			Vector3 pathStart = nodeLink.node1.transform.localPosition;
			Vector3 pathDirection = nodeLink.node2.transform.localPosition - nodeLink.node1.transform.localPosition;
			Vector3 positionToPathStart = pathStart - position;
			float dotRealPart = positionToPathStart.x * pathDirection.x;
			dotRealPart += positionToPathStart.y * pathDirection.y;
			dotRealPart += positionToPathStart.z * pathDirection.z;
			dotRealPart *= -1;
			float dotVariablePart = pathDirection.x * pathDirection.x;
			dotVariablePart += pathDirection.y * pathDirection.z;
			dotVariablePart += pathDirection.z * pathDirection.z;
			float directionFactor = dotRealPart / dotVariablePart;
			// Prevent intersections outside of the node-link's range
			directionFactor = Mathf.Clamp01 (directionFactor);
			Vector3 intersection = pathStart + pathDirection * directionFactor;
			Vector3 positionToIntersection = intersection - position;
			float magnitude = positionToIntersection.sqrMagnitude;
			if (magnitude < shortestPathMagnitude) {
				shortestPathMagnitude = magnitude;
				shortestPathIntersection = intersection;
			}
		}

		return shortestPathIntersection;
	}

	public PathNode GetClosestPathNodeToTargetPosition (Vector3 position)
	{
		float closestNodeDistance = float.PositiveInfinity;
		PathNode closestNode = null;
		foreach (PathNode node in pathNodes) {
			float distanceSquared = (node.transform.position - position).sqrMagnitude;
			if (distanceSquared < closestNodeDistance) {
				closestNodeDistance = distanceSquared;
				closestNode = node;
			}
		}

		return closestNode;
	}

	public PathResult GetPathToNode (Vector3 position, PathNode targetNode)
	{
		List<PathNode> path = new List<PathNode> ();

		PathNode currentNode = targetNode;
		float currentNodeDistance = (targetNode.transform.position - position).sqrMagnitude;

		path.Add (currentNode);
		// While shit == fucked
		while (currentNode.nodeLinks != null) {
			PathNode nextNode = currentNode;
			float nextNodeDistance = currentNodeDistance;
			foreach (PathNodeLink link in currentNode.nodeLinks) {
				PathNode node = link.GetOtherNode (currentNode);
				float distanceToTarget = (node.transform.position - position).sqrMagnitude;
				if (distanceToTarget < nextNodeDistance) {
					nextNode = node;
					nextNodeDistance = distanceToTarget;
				}
			}
			if (currentNode == nextNode) {
				// Found no node closer to the target than the current node
				break;
			} else {
				path.Add (nextNode);
				currentNode = nextNode;
				currentNodeDistance = nextNodeDistance;
			}

		}

		PathResult result = new PathResult (path.ToArray ());

		return result;
	}

	public bool IsObjectsInSameRoom (Transform obj1, Transform obj2)
	{
		Vector3 targetVector = obj2.position - obj1.position;
		// CLose than 2 meters
		return targetVector.sqrMagnitude <= 4f;
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.green;
		foreach (PathNode node in pathNodes) {
			Gizmos.DrawWireSphere (node.transform.position, 0.3f);
		}
		foreach (PathNodeLink nodeLink in pathNodeLinks) {
			if (nodeLink == null)
				continue;
			Gizmos.DrawLine (nodeLink.node1.transform.position, nodeLink.node2.transform.position);
		}
	}
}
