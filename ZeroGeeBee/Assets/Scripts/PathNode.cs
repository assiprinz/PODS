using UnityEngine;
using System.Collections;

public class PathNode : MonoBehaviour
{

	public PathNode[] outgoingNodes;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere (transform.position, 0.3f);
		foreach (PathNode node in outgoingNodes) {
			if (node == null)
				continue;
			Gizmos.DrawLine (transform.localPosition, node.transform.localPosition);
		}
	}
}
