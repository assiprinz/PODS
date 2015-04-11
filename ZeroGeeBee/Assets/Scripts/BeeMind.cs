using UnityEngine;
using System.Collections;

public class BeeMind : DockableObject
{
	[System.Serializable]
	public class Thruster
	{
		public string test;
	}

	public BeeConnector platform1;
	public BeeConnector platform2;
	public Transform payload;

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
	}
}
