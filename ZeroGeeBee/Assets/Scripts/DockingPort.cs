﻿using UnityEngine;
using System.Collections;

public class DockingPort : MonoBehaviour
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
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere (transform.position, 0.2f);
		Gizmos.DrawLine (transform.position, transform.position + transform.forward * 0.3f);
		Gizmos.DrawLine (transform.position + transform.forward * 0.1f, transform.position + transform.forward * 0.1f + transform.right * 0.3f);
	}
}
