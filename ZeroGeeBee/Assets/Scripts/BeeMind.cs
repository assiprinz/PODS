using UnityEngine;
using System.Collections;

public class BeeMind : DockableObject
{


	enum Command
	{
		Idle,
		TurnTo,
		MoveTo,
		DockAt
	}
	/*
	 * Deliver (dock) cargo x to dock y
	 *   Get to cargo
	 *     Get onto highway
	 *     Follow highway to destination room
	 *   Dock
	 *     Fly to alignment position
	 *     Turn to face to port
	 *     Advance till docking achieved
	 *   Undock cargo from dock
	 *   Get to target dock
	 *     Get onto highway
	 *     Follow highway to target room
	 *   Align cargo docking port with target dock port
	 *   Dock
	 *   Undock payload from drone
	 * 
	 * 
	 */

	public Game game;

	public PayloadObject targetPayload;
	public DockableObject targetDock;

	private bool isOnHighway;
	private Vector3 highwayIntersection;
	private PathNode highwayExit;
	private PathResult highwayPath;

	private bool isInRoomWithTarget;
	private DockingPort targetPort;

	private bool isInFrontOfTargetPort;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (isInFrontOfTargetPort) {

		} else if (isInRoomWithTarget) {
			// Choose port and move to it
			if (MoveTowardsPoint (targetPort.getDockingNearLocation ())) {
				isInFrontOfTargetPort = true;
				Debug.Log ("Arrived at target port");
			}
		} else if (isOnHighway) {
			// Move along highway
			PathNode nextHighwayNode = highwayPath.GetCurrentNode ();
			if (MoveTowardsPoint (nextHighwayNode.transform.position)) {
				// Arrived at node
				highwayPath.AdvanceToNextNode ();
				nextHighwayNode = highwayPath.GetCurrentNode ();
				if (nextHighwayNode == null) {
					// Arrived in target room
					isInRoomWithTarget = true;
					targetPort = targetPayload.GetFreeDockingPort ();
					Debug.Log ("Arrived in target room");
				}
			}
		} else {
			// Move to highway
			Vector3 highwayIntersection = game.GetClosestPathIntersection (transform.position);

			if (MoveTowardsPoint (highwayIntersection)) {

				// Arrived on highway, calculate highway path
				isOnHighway = true;
				PathNode exitNode = game.GetClosestPathNodeToTargetPosition (targetPayload.transform.position);
				highwayPath = game.GetPathToNode (transform.position, exitNode);
				Debug.Log ("Arrived at highway, nodes to go: " + highwayPath.nodes.Length + ", first node: " + highwayPath.GetCurrentNode ());
			}
		}

	}

	private bool RotateToAlignWithForwardAndUp (Vector3 targetForward, Vector3 targetUp)
	{
		return false;
	}

	private bool MoveTowardsPoint (Vector3 targetPoint)
	{
		Vector3 targetVector = targetPoint - transform.position;
		float distance = targetVector.magnitude;
		if (distance < 0.01f) {
			// Arrived target
			return true;
		}

		targetVector.Normalize ();

		float targetAngle = Mathf.Acos (Vector3.Dot (targetVector, transform.forward));
		if (targetAngle > 0.001f) {
			// Turn to target
			float turnAngle = Mathf.Clamp (targetAngle * Mathf.Rad2Deg, 0f, 3f);
			Vector3 axis = Vector3.Cross (transform.forward, targetVector);
			Quaternion targetRotation = Quaternion.AngleAxis (turnAngle, axis);
			transform.rotation = targetRotation * transform.rotation;

			return false;
		}

		// Turned to target, move
		float moveDistance = Mathf.Clamp (distance, 0f, 0.05f);
		transform.localPosition += targetVector * moveDistance;

		return false;
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = GetComponentInChildren<MeshRenderer> ().sharedMaterial.color;
		Gizmos.DrawRay (transform.position, transform.forward * 2f);

		Vector3 pathIntersection = game.GetClosestPathIntersection (transform.position);
		Gizmos.DrawLine (transform.position, pathIntersection);
		Gizmos.DrawSphere (pathIntersection, 0.3f);

		
		PathNode closestNode = game.GetClosestPathNodeToTargetPosition (transform.position);
		Gizmos.DrawLine (transform.position, closestNode.transform.position);
		Gizmos.DrawSphere (closestNode.transform.position, 0.3f);

		PathNode exitNode = game.GetClosestPathNodeToTargetPosition (targetPayload.transform.position);
		PathResult path = game.GetPathToNode (transform.position, exitNode);
		Gizmos.color = Color.magenta;
		foreach (PathNode node in path.nodes) {
			Gizmos.DrawSphere (node.transform.position, 0.3f);
		}
	}
}
