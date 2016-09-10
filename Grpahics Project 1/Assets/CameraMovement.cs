using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public CharacterController charController;

	public float rollSpeed = 0.5f;
	public float mouseSpeed = 0.5f;
	public float keyboardSpeed = 0.5f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 moveDirection = Vector3.zero;

		Vector3 forwardVec = transform.forward;
		Vector3 rightVec = transform.right;

		// mouse control
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

		float yaw = mouseX * mouseSpeed * Time.deltaTime;
		float pitch = -mouseY * mouseSpeed * Time.deltaTime;
		float roll = 0f;

		// keyboard control
		if (Input.GetKey(KeyCode.W)) {
			moveDirection += forwardVec * keyboardSpeed;
		}
		
		if (Input.GetKey(KeyCode.S)) {
			moveDirection -= forwardVec * keyboardSpeed;
		}
		
		if (Input.GetKey(KeyCode.A)) {
			moveDirection -= rightVec * keyboardSpeed;
		}
		
		if (Input.GetKey(KeyCode.D)) {
			moveDirection += rightVec * keyboardSpeed;
		}

		if (Input.GetKey(KeyCode.Q)) {
			roll += keyboardSpeed * Time.deltaTime;
		}

		if (Input.GetKey(KeyCode.E)) {
			roll -= keyboardSpeed * Time.deltaTime;
		}


		if (Input.GetKey(KeyCode.H)) {
			Debug.Log ("transform.forward " + transform.forward);
			Debug.Log ("forwardvec " + forwardVec);
		}


		Vector3 rotation = transform.rotation.eulerAngles;

		// bound x rotation to avoid weird camera control going beyond up/down
		float newX = rotation.x + pitch;
		if ( ! ((newX <= 90f) || (newX >= 270f)) ) {
			newX = rotation.x;
		} 
			

		charController.Move (moveDirection * Time.deltaTime);
		transform.rotation = Quaternion.Euler (newX, rotation.y + yaw, rotation.z + roll);
	
	}
}
