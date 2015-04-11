using UnityEngine;
using System.Collections;

public class PayloadObject : DockableObject
{



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
		Gizmos.color = GetComponentInChildren<MeshRenderer> ().sharedMaterial.color;
		Gizmos.DrawLine (transform.position, transform.position + transform.forward * 1.0f);
	}
}
