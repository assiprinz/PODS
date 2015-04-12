using UnityEngine;
using System.Collections;

public class TargetGizmo : MonoBehaviour {

	private Color sphereColor = Color.red;

	void Start() {

	}

	void OnTriggerEnter(Collider other) {
		sphereColor = Color.green;
	}

	void OnTriggerExit(Collider other) {
		sphereColor = Color.red;
	}

	void OnDrawGizmos() {
		Gizmos.color = sphereColor;
		Gizmos.DrawWireSphere(transform.position, 1f);
		/*
		Gizmos.color = Color.green;
		Vector3 force = Vector3.Cross(transform.forward, (target.transform.position - transform.position));
		Gizmos.DrawRay(transform.position, force * 2);
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, rb.angularVelocity * 2);
		*/
	}

}
