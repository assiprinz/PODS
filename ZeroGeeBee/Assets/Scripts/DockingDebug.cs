using UnityEngine;
using System.Collections;

public class DockingDebug : MonoBehaviour {


	public DockingPort sourcePort;
	public DockingPort targetPort;
	// Use this for initialization
	float speed = 1.0f;

	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
			//rotateTowardsDock();
	}

	void rotateTowardsDock(){
		Vector3 targetDir = targetPort.transform.position - transform.position;
		float step = speed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
		Debug.DrawRay(transform.position, newDir, Color.red);
		transform.rotation = Quaternion.LookRotation(newDir);
	}
		
}