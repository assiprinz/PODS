using UnityEngine;
using System.Collections;

public class BeeMind : DockableObject
{
	[System.Serializable]
	public class Thruster
	{
		public string test;
	}

	public Game game;

	public Thruster a;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{

	}

	void OnDrawGizmos ()
	{
		Gizmos.color = GetComponentInChildren<MeshRenderer> ().sharedMaterial.color;
		Gizmos.DrawLine (transform.position, transform.position + transform.forward * 1.0f);

		Vector3 pathIntersection = game.GetClosestPathIntersection (transform.position);
		Gizmos.DrawLine (transform.position, pathIntersection);
		Gizmos.DrawSphere (pathIntersection, 0.3f);

		
		PathNode closestNode = game.GetClosestPathNodeToTargetPosition (transform.position);
		Gizmos.DrawLine (transform.position, closestNode.transform.position);
		Gizmos.DrawSphere (closestNode.transform.position, 0.3f);
	}
}
