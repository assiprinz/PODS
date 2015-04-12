using UnityEngine;
using System.Collections;

public class DockingPort : MonoBehaviour
{

	public DockingPort dockedWith;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public Vector3 getDockingNearLocation ()
	{
		return (transform.position + (transform.forward * 1));
	}

	public DockableObject GetDockedObject ()
	{
		return dockedWith.GetComponentInParent<DockableObject> ();
	}

	public bool IsDocked ()
	{
		return dockedWith != null;
	}

	public void DockWith (DockingPort otherPort)
	{
		dockedWith = otherPort;
		otherPort.dockedWith = this;
	}

	public void Undock ()
	{
		dockedWith.dockedWith = null;
		dockedWith = null;
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere (transform.position, 0.2f);
		Gizmos.DrawLine (transform.position, transform.position + transform.forward * 0.3f);
		Gizmos.DrawLine (transform.position + transform.forward * 0.1f, transform.position + transform.forward * 0.1f + transform.right * 0.3f);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere (getDockingNearLocation (), 0.2f);
	}

}
