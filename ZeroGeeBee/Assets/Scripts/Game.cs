using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{

	public PathNode[] pathNodes;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public Vector3 GetClosestPathIntersection (Vector3 position)
	{
		float shortestPathMagnitude = float.PositiveInfinity;
		Vector3 shortestPathIntersection = Vector3.zero;

		foreach (PathNode node in pathNodes) {
			foreach (PathNode node2 in node.outgoingNodes) {
				Vector3 pathStart = node.transform.localPosition;
				Vector3 pathDirection = node2.transform.localPosition - node.transform.localPosition;
				Vector3 positionToPathStart = pathStart - position;
				float dotRealPart = positionToPathStart.x * pathDirection.x;
				dotRealPart += positionToPathStart.y * pathDirection.y;
				dotRealPart += positionToPathStart.z * pathDirection.z;
				dotRealPart *= -1;
				float dotVariablePart = pathDirection.x * pathDirection.x;
				dotVariablePart += pathDirection.y * pathDirection.z;
				dotVariablePart += pathDirection.z * pathDirection.z;
				float directionFactor = dotRealPart / dotVariablePart;
				// Prevent intersections outside of the vectors range
				directionFactor = Mathf.Clamp01 (directionFactor);
				Vector3 intersection = pathStart + pathDirection * directionFactor;
				Vector3 positionToIntersection = intersection - position;
				float magnitude = positionToIntersection.sqrMagnitude;
				if (magnitude < shortestPathMagnitude) {
					shortestPathMagnitude = magnitude;
					shortestPathIntersection = intersection;
				}
			}
		}
		//PathNode node = pathRoot;
		//while
		Debug.Log (shortestPathIntersection);
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
}
