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
	public float speed = 1.0f;
	public bool connected = false;
	public DockingPort targetPort;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{

	}
	void Update() {
		float step = speed * Time.deltaTime;
		if(transform.position.Equals(targetPort.getDockingNearLocation())){
			float upDot = Vector3.Dot(dockingPorts[0].transform.up,targetPort.transform.up);
			float forwardDot = Vector3.Dot(dockingPorts[0].transform.forward * -1,targetPort.transform.forward);
			//Debug.Log (upDot);
			//Debug.Log (forwardDot);
			if(upDot == 1f && forwardDot == 1f){
				if(!connected){
					Joint joint = transform.gameObject.AddComponent<FixedJoint>();
					joint.connectedBody = targetPort.GetComponentInParent<Rigidbody>();
					connected = true;
				}
				//Debug.Log("done");
			}else{
				//Debug.Log("dockport");
				//Debug.Log (dockingPorts[0].transform.up+" "+dockingPorts[0].transform.up.magnitude);
				//Debug.Log ("targetPort");
				//Debug.Log (targetPort.transform.up+" "+targetPort.transform.up.magnitude);
				Vector3 targetDir = targetPort.transform.position - transform.position;
				Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
				Debug.DrawRay(transform.position, newDir, Color.red);
				transform.rotation = Quaternion.LookRotation(newDir,targetPort.transform.up);
				//transform.rotation = Quaternion.Slerp(transform.rotation, targetPort.transform.rotation, step);
			}

		} else {
			transform.position = Vector3.MoveTowards(transform.position, targetPort.getDockingNearLocation(), step);
		}
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
