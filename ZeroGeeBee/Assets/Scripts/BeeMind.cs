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
	
	public float speed = 1.0f;
	public bool connected = false;
	public DockingPort targetPort;
	
	public DockableObject targetDock;

	private bool isOnHighway;
	private Vector3 highwayIntersection;
	private PathNode highwayExit;
	private PathResult highwayPath;

	private bool isInRoomWithTarget;

	private bool isInFrontOfTargetPort;

	private bool isAlignedWithPort;



	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (isAlignedWithPort) {
			// Move to dock
			if (MoveTowardsPoint (targetPort.transform.position + targetPort.transform.forward * 0.1f)) {
				Debug.Log ("Docked");
			}
		} else if (isInFrontOfTargetPort) {
			// Align with target port
			Vector3 targetVector = targetPort.transform.position - transform.position;
			if (RotateToAlignWithForwardAndUp (targetVector, targetPort.transform.up)) {
				// Aligned with port
				isAlignedWithPort = true;
				Debug.Log ("Aligned with port");
			}
		} else if (isInRoomWithTarget) {
			// Move to port
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
					targetPort = targetDock.GetFreeDockingPort ();
					Debug.Log ("Arrived in target room");
				}
			}
		} else {
			// Move to highway
			Vector3 highwayIntersection = game.GetClosestPathIntersection (transform.position);

			if (MoveTowardsPoint (highwayIntersection)) {

				// Arrived on highway, calculate highway path
				isOnHighway = true;
				PathNode exitNode = game.GetClosestPathNodeToTargetPosition (targetDock.transform.position);
				highwayPath = game.GetPathToNode (transform.position, exitNode);
				Debug.Log ("Arrived at highway, nodes to go: " + highwayPath.nodes.Length + ", first node: " + highwayPath.GetCurrentNode ());
			}
		}

	}

	private bool RotateToAlignWithForwardAndUp (Vector3 targetForward, Vector3 targetUp)
	{
		Vector3 newDir = Vector3.RotateTowards (transform.forward, targetForward, 0.02f, 0.0f);
		transform.rotation = Quaternion.LookRotation (newDir, targetUp);
		return newDir == targetForward;
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
	/*void Update ()
	{
		float step = speed * Time.deltaTime;
		if (transform.position.Equals (targetPort.getDockingNearLocation ())) {
			float upDot = Vector3.Dot (dockingPorts [0].transform.up, targetPort.transform.up);
			float forwardDot = Vector3.Dot (dockingPorts [0].transform.forward * -1, targetPort.transform.forward);
			//Debug.Log (upDot);
			//Debug.Log (forwardDot);
			if (upDot == 1f && forwardDot == 1f) {
				if (!connected) {
					Joint joint = transform.gameObject.AddComponent<FixedJoint> ();
					joint.connectedBody = targetPort.GetComponentInParent<Rigidbody> ();
					connected = true;
				}
				//Debug.Log("done");
			} else {
				//Debug.Log("dockport");
				//Debug.Log (dockingPorts[0].transform.up+" "+dockingPorts[0].transform.up.magnitude);
				//Debug.Log ("targetPort");
				//Debug.Log (targetPort.transform.up+" "+targetPort.transform.up.magnitude);
				Vector3 targetDir = targetPort.transform.position - transform.position;
				Vector3 newDir = Vector3.RotateTowards (transform.forward, targetDir, step, 0.0f);
				Debug.DrawRay (transform.position, newDir, Color.red);
				transform.rotation = Quaternion.LookRotation (newDir, targetPort.transform.up);
				//transform.rotation = Quaternion.Slerp(transform.rotation, targetPort.transform.rotation, step);
			}

		} else {
			transform.position = Vector3.MoveTowards (transform.position, targetPort.getDockingNearLocation (), step);
		}
	}*/
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

		PathNode exitNode = game.GetClosestPathNodeToTargetPosition (targetDock.transform.position);
		PathResult path = game.GetPathToNode (transform.position, exitNode);
		Gizmos.color = Color.magenta;
		foreach (PathNode node in path.nodes) {
			Gizmos.DrawSphere (node.transform.position, 0.3f);
		}
	}
}
