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
 	private float curSpeed;
 	private float curAngular;
	
 	// States
 	private bool killRotationEnabled = false;
 	private bool killTranslationEnabled = false;
	private bool moveTowardsEnabled = false;
	private bool realignEnabled = false;
	private bool lookAtEnabled = false;

	private PID pid;

	void Start () {
		rb = gameObject.GetComponent<Rigidbody>();
		targetPos = target.position;
		pid = new PID(8f,0f,20f);
	}
	
	void FixedUpdate () {

		curSpeed = rb.velocity.magnitude;
		curAngular = rb.angularVelocity.magnitude;

		inputKeys();

		Vector3 frameForce = new Vector3();
		Vector3 frameTorque = new Vector3();

		frameForce += inputForce();
		frameTorque += inputTorque();

		if (killRotationEnabled) {
			frameTorque += killRotationMovement();
			if (rotationKilled()) {
				killRotationEnabled = false;
				Debug.Log("Rotation killed");
			}
		}

		if (killTranslationEnabled) {
			frameForce += killTranslationMovement();
			if (translationKilled()) {
				killTranslationEnabled = false;
				Debug.Log("Translation killed");
			}
		}

		if (moveTowardsEnabled) {
			float distance = (targetPos - transform.position).magnitude;
			float tolerance = 0.05f;
			//frameForce += towardsTargetForce();
			frameForce -= Vector3.ClampMagnitude(((targetPos - transform.position) * pid.Update(0f, distance, Time.deltaTime)), thrust);
			if (distance <= 0.5f && lookAtEnabled) {
				lookAtEnabled = false;
				killTranslationEnabled = true;
				Debug.Log("close to target. deactivating lookat");
			}
			if (distance <= 0.07f && rb.velocity.magnitude <= 0.02f) {
			}
		}

		if(lookAtEnabled) {
			frameTorque += targetDirectionTorque();
		}

		rb.AddForce(frameForce, ForceMode.Acceleration);
		rb.AddTorque(frameTorque, ForceMode.Acceleration);

	}

	private bool translationKilled() {
		return rb.velocity.magnitude < 0.01f;
	}

	private bool rotationKilled() {
		return rb.angularVelocity.magnitude < 0.01f;
	}

	private Vector3 killRotationMovement() {
		return ( - Vector3.ClampMagnitude(rb.angularVelocity * 2, torque));
	}

	private Vector3 killTranslationMovement() {
		 return ( - Vector3.ClampMagnitude(rb.velocity * 5, thrust));
	}

	private Vector3 towardsTargetForce() {

		Vector3 returnForce = new Vector3();

		transform.position = transform.position;
		float distanceToTarget = (targetPos - transform.position).magnitude;

   		Vector3 error = new Vector3(targetPos.x - transform.position.x, targetPos.y - transform.position.y, targetPos.z - transform.position.z);
   		integrator += error * Time.deltaTime;
   		Vector3 subt = new Vector3(error.x - lastError.x, error.y - lastError.y, error.z - lastError.z);
   		Vector3 diff = new Vector3(subt.x / Time.deltaTime, subt.y / Time.deltaTime, subt.y / Time.deltaTime);
   		lastError = error;
   		Vector3 force = error * pGain + integrator * iGain + diff * dGain;

   		float brakeLength = (rb.velocity.magnitude * rb.velocity.magnitude) / (2 * thrust);

   		if(brakeLength < distanceToTarget) {
   			returnForce += Vector3.ClampMagnitude(force, thrust);
   		} else {
   			force = force * -1;
   			returnForce += Vector3.ClampMagnitude(force, thrust);
   		}

   		return returnForce;
	}

	private Vector3 targetDirectionTorque() {

		Vector3 returnTorque = new Vector3();
		
		float angle = Mathf.Acos(Vector3.Dot(transform.forward, (targetPos - transform.position).normalized));

		// needed angular to slow down to zero at target direction
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
			returnTorque += (red * torque * rotKillFactor * -1 + force * torque * targetRotFactor);
		} else {
			if(angle > 0.01f) {
				returnTorque += ((force * torque) * -1 + red * torque * rotKillFactor * -1);
			}
		}
		return returnTorque;
	}



	private void inputKeys() {
		// Navigation Switch
		if(Input.GetKeyUp(KeyCode.F)) {
			if (!moveTowardsEnabled) {
				moveTowardsEnabled = true;
				Debug.Log("Navigation ON");
			} else {
				moveTowardsEnabled = false;
				Debug.Log("navigation off");
			}
			
			
		}

		// Kill Rot
		if(Input.GetKeyUp(KeyCode.R)) {
			killRotationEnabled = true;
			Debug.Log("RotKill ON");
		}

		if(Input.GetKeyUp(KeyCode.T)) {
			killTranslationEnabled = true;
			Debug.Log("TransKill ON");
		}

		if(Input.GetKeyUp(KeyCode.L)) {
			if (lookAtEnabled) {
				lookAtEnabled = false;
				killRotationEnabled = true;
				Debug.Log("lookat off");
			} else {
				lookAtEnabled = true;
				Debug.Log("LookAt ON");
			}
			
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

public class PID {
	public float pFactor, iFactor, dFactor;
		
	float integral;
	float lastError;
	
	
	public PID(float pFactor, float iFactor, float dFactor) {
		this.pFactor = pFactor;
		this.iFactor = iFactor;
		this.dFactor = dFactor;
	}
	
	
	public float Update(float setpoint, float actual, float timeFrame) {
		float present = setpoint - actual;
		integral += present * timeFrame;
		float deriv = (present - lastError) / timeFrame;
		lastError = present;
		return present * pFactor + integral * iFactor + deriv * dFactor;
	}
}
