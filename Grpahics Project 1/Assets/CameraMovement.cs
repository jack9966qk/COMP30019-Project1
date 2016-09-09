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
		Vector3 rotateDirection = Vector3.zero;

		Vector3 forwardVec = transform.forward;
		Vector3 rightVec = transform.right;

		// mouse control
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

//		rotateDirection += new Vector3(-mouseY * mouseSpeed, mouseX * mouseSpeed, 0);
		float xRotate = mouseX * mouseSpeed * Time.deltaTime;
		float yRotate = -mouseY * mouseSpeed * Time.deltaTime;
		float zRotate = 0f;

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
			zRotate += keyboardSpeed * Time.deltaTime;
		}

		if (Input.GetKey(KeyCode.E)) {
			zRotate -= keyboardSpeed * Time.deltaTime;
		}


		if (Input.GetKey(KeyCode.H)) {
			Debug.Log ("transform.forward " + transform.forward);
			Debug.Log ("forwardvec " + forwardVec);
		}

		charController.Move (moveDirection * Time.deltaTime);
		Vector3 rotation = transform.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler (rotation.x + yRotate, rotation.y + xRotate, rotation.z + zRotate);
//		transform.eulerAngles = new Vector3 (rotation.x + yRotate, rotation.y + xRotate, rotation.z + zRotate);
//		transform.rotation *= Quaternion.AngleAxis (xRotate, Vector3.up);
//		transform.rotation *= Quaternion.AngleAxis (zRotate, Vector3.forward);
//		transform.rotation *= Quaternion.AngleAxis (yRotate, Vector3.right);
		//transform.rotation *= Quaternion.Euler (yRotate, xRotate, zRotate);
	
	}
}
