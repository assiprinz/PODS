using UnityEngine;
using System.Collections;

public class BeeBehavior : MonoBehaviour {

	public Transform target;
	public Rigidbody rb;

	public float thrust = 2;
	public float torque = 1;

	// PID
	private Vector3 targetPos = Vector3.zero; // target pos
 	public float pGain = 2f; // proportional 
 	public float iGain = 5f; // integral
 	public float dGain = 3f; // differential
 	private Vector3 integrator = Vector3.zero; // error accumulator
 	private Vector3 lastError = Vector2.zero; 

 	// Sensors ;)
 	private Vector3 curPos;
 	private float curSpeed;
 	private float curAngular;
	
 	// States
 	private bool killRotationEnabled = false;
	private bool navigationEnabled = false;
	private bool realignEnabled = false;
	private bool lookAtEnabled = false;

	void Start () {
		rb = gameObject.GetComponent<Rigidbody>();
		curPos = gameObject.transform.position;
		targetPos = target.position;
	}
	
	void FixedUpdate () {

		curSpeed = rb.velocity.magnitude;
		curAngular = rb.angularVelocity.magnitude;
		curPos = gameObject.transform.position;

		inputEval();

		Vector3 frameForce = new Vector3(0,0,0);
		Vector3 frameTorque = new Vector3(0,0,0);

		frameForce += inputForce();
		frameTorque += inputTorque();

		if(realignEnabled) {
			if(!realigned()) {
				killRotation();
				killTranslation();
			} else {
				realignEnabled = false;
			}
		}

		if(lookAtEnabled) {
			frameTorque += lookAtTarget();
		}

		

		

		if(killRotationEnabled) {
			if (!rotZero()) {
				killRotation();
			} else {
				killRotationEnabled = false;
			}
			

		}

		rb.AddForce(frameForce);
		rb.AddTorque(frameTorque);

	}

	private bool realigned() {
		bool rotAligned = curAngular < 0.01f;
		bool transAligned = curSpeed < 0.01f;
		return (rotAligned && transAligned);
	}

	private bool rotZero() {
		return curAngular < 0.01f;
	}

	private bool transZero() {
		return curSpeed < 0.01f;
	}

	private void pidControlPosition() {
		transform.position = transform.position;
		float distanceToTarget = (targetPos - transform.position).magnitude;

   		Vector3 error = new Vector3(targetPos.x - transform.position.x, targetPos.y - transform.position.y, targetPos.z - transform.position.z);
   		integrator += error * Time.deltaTime;
   		Vector3 subt = new Vector3(error.x - lastError.x, error.y - lastError.y, error.z - lastError.z);
   		Vector3 diff = new Vector3(subt.x / Time.deltaTime, subt.y / Time.deltaTime, subt.y / Time.deltaTime);
   		lastError = error;
   		Vector3 force = error * pGain + integrator * iGain + diff * dGain;

   		if(distanceToTarget > 1.5f) {
   			force = Vector3.ClampMagnitude(force, thrust);
   		} else {
   			force = force * -1;
   			force = Vector3.ClampMagnitude(force, thrust);
   		}

   		rb.AddForce(force);
	}

	private Vector3 lookAtTarget() {

		Vector3 retTorque = new Vector3();
		
		Vector3 curDir = transform.forward;
		Vector3 targetDir = targetPos - transform.position;
		targetDir.Normalize();
		float dotProd = Vector3.Dot(curDir, targetDir);
		float angle = Mathf.Acos(dotProd);

		float brakeLength = (curAngular * curAngular) / (2 * torque);

		Vector3 force = Vector3.Cross(transform.forward, (target.transform.position - transform.position)).normalized;

		
		Vector3 green = Vector3.Cross(transform.forward, (target.transform.position - transform.position)).normalized;
		Vector3 red = rb.angularVelocity.normalized;

		float dotProd2 = Vector3.Dot(green, red);

		float arschlochFaktor = 1f - (Mathf.Abs(dotProd2));
		float targetRotFactor = 1f - arschlochFaktor;
		float rotKillFactor = arschlochFaktor;

		float dir;
		if (dotProd2 <= 0) {
			dir = -1f;
		} else {
			dir = 1f;
		}

		if (rotKillFactor == 1f) {
			rotKillFactor = 0f;
			targetRotFactor = 1;
		}

		if (dir == -1f || dir == 1f && brakeLength < angle) {
			retTorque += (red * torque * rotKillFactor * -1 + force * torque * targetRotFactor);
		} else {
			if(angle > 0.01f) {
				retTorque += ((force * torque) * -1 + red * torque * rotKillFactor * -1);
			}
		}

		return retTorque;
		
		//Debug.Log("D2: " + dotProd2 + "   aF: " + arschlochFaktor + "   rKF: " + rotKillFactor);
		//Debug.Log(dir + "a: " + angle + "f: " + force + "trf " + targetRotFactor);
		
	}

	private void killRotation() {
		rb.AddTorque( - Vector3.ClampMagnitude(rb.angularVelocity * 2, torque));
	}

	private void killTranslation() {
		rb.AddForce( - Vector3.ClampMagnitude(rb.velocity * 5, thrust));
	}

	private void inputEval() {
		// Navigation Switch
		if(Input.GetKeyUp(KeyCode.F)) {
			navigationEnabled = !navigationEnabled;
		}

		// Kill Rot
		if(Input.GetKeyUp(KeyCode.R)) {
			realignEnabled = !realignEnabled;
		}

		if(Input.GetKeyUp(KeyCode.L)) {
			lookAtEnabled = !lookAtEnabled;
		}
	}

	private Vector3 inputForce() {

		Vector3 retForce = new Vector3();

		// Manual Controls
		// Translation
		if(Input.GetKey(KeyCode.W)) {
			retForce += (transform.forward * thrust);
		}
		if(Input.GetKey(KeyCode.S)) {
			retForce += (-transform.forward * thrust);
		}

		if(Input.GetKey(KeyCode.Space)) {
			retForce += (transform.up * thrust);
		}
		if(Input.GetKey(KeyCode.A)) {
			retForce += (-transform.right * thrust);
		}
		if(Input.GetKey(KeyCode.LeftControl)) {
			retForce += (-transform.up * thrust);
		}
		if(Input.GetKey(KeyCode.D)) {
			retForce += (transform.right * thrust);
		}

		return retForce;
	}

	private Vector3 inputTorque() {

		Vector3 retForce = new Vector3();
		// Rotation
		if(Input.GetKey(KeyCode.UpArrow)) {
			retForce += (transform.right * torque);
		}
		if(Input.GetKey(KeyCode.DownArrow)) {
			retForce += (-transform.right * torque);
		}
		if(Input.GetKey(KeyCode.LeftArrow)) {
			retForce += (-transform.up * torque);
		}
		if(Input.GetKey(KeyCode.RightArrow)) {
			retForce += (transform.up * torque);
		}
		if(Input.GetKey(KeyCode.Q)) {
			retForce += (transform.forward * torque);
		}
		if(Input.GetKey(KeyCode.E)) {
			retForce += (-transform.forward * torque);
		}

		return retForce;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, transform.forward * 50f);
		Gizmos.color = Color.blue;
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
