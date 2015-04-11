using UnityEngine;
using System.Collections;

public class BeeBehavior : MonoBehaviour {

	public Transform target;

	public float thrust = 1;
	public float torque = 1;

	// PID Stuff
	private Vector3 targetPos = Vector3.zero; // the desired position
 	public float pGain = 2f; // the proportional gain
 	public float iGain = 5f; // the integral gain
 	public float dGain = 3f; // differential gain
 	private Vector3 integrator = Vector3.zero; // error accumulator
 	private Vector3 lastError = Vector2.zero; 

 	private Vector3 curPos;

	private Rigidbody rb;

	void Start () {
		rb = gameObject.GetComponent<Rigidbody>();

		targetPos = target.position;

		pGain = pGain / 10f;
		iGain = iGain / 10f;
		dGain = dGain / 10f;

	}
	
	void FixedUpdate () {
		evaluateInput();
	}

	private void pidControlPosition() {
		curPos = transform.position;
   		Vector3 error = new Vector3(targetPos.x - curPos.x, targetPos.y - curPos.y, targetPos.z - curPos.z);
   		integrator += error * Time.deltaTime;
   		//Vector3 subt = new Vector3(error.x - lastError.x, error.y - lastError.y, error.z - lastError.z);
   		Vector3 subt = error - lastError;
   		Vector3 diff = new Vector3(subt.x / Time.deltaTime, subt.y / Time.deltaTime, subt.y / Time.deltaTime); // differentiate error
   		lastError = error;
   		// calculate the force summing the 3 errors with respective gains:
   		Vector3 force = error * pGain + integrator * iGain + diff * dGain;

   		Vector3 distance = targetPos - curPos;
   		Debug.Log("distance" + distance);

   		// clamp the force to the max value available
   		force = Vector3.ClampMagnitude(force, thrust);
   		// apply the force to accelerate the rigidbody:
   		rb.AddForce(force);
	}

	private void killRotation() {

		rb.AddTorque( - Vector3.ClampMagnitude(rb.angularVelocity, torque));
	}

	private void killTranslation() {
		rb.AddForce( - Vector3.ClampMagnitude(rb.velocity, thrust));
	}

	private void evaluateInput() {

		if(Input.GetKey(KeyCode.F)) {
			pidControlPosition();
		}

		if(Input.GetKey(KeyCode.R)) {
			killRotation();
		}

		if(Input.GetKey(KeyCode.C)) {
			killTranslation();
		}

		if(Input.GetKey(KeyCode.W)) {
			rb.AddForce(transform.forward * thrust);
		}
		if(Input.GetKey(KeyCode.S)) {
			rb.AddForce(-transform.forward * thrust);
		}

		if(Input.GetKey(KeyCode.Space)) {
			rb.AddForce(transform.up * thrust);
		}
		if(Input.GetKey(KeyCode.A)) {
			rb.AddForce(-transform.right * thrust);
		}
		if(Input.GetKey(KeyCode.LeftControl)) {
			rb.AddForce(-transform.up * thrust);
		}
		if(Input.GetKey(KeyCode.D)) {
			rb.AddForce(transform.right * thrust);
		}

		if(Input.GetKey(KeyCode.UpArrow)) {
			rb.AddTorque(transform.right * torque);
		}
		if(Input.GetKey(KeyCode.DownArrow)) {
			rb.AddTorque(-transform.right * torque);
		}
		if(Input.GetKey(KeyCode.LeftArrow)) {
			rb.AddTorque(-transform.up * torque);
		}
		if(Input.GetKey(KeyCode.RightArrow)) {
			rb.AddTorque(transform.up * torque);
		}
		if(Input.GetKey(KeyCode.Q)) {
			rb.AddTorque(transform.forward * torque);
		}
		if(Input.GetKey(KeyCode.E)) {
			rb.AddTorque(-transform.forward * torque);
		}
	}
}
