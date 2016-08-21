using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public float rollSpeed = 0.5f;
	public float mouseSpeed = 0.5f;
	public float keyboardSpeed = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 forwardVec = Vector3.Normalize(this.transform.right + this.transform.forward);
		Vector3 rightVec = -Vector3.Normalize (Vector3.Cross (Vector3.up, forwardVec));

		// mouse control
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

		this.transform.Rotate (-mouseY * mouseSpeed, mouseX * mouseSpeed, 0);


		// keyboard control

		if (Input.GetKey(KeyCode.W)) {
			this.transform.position -= forwardVec * keyboardSpeed;
		}
		
		if (Input.GetKey(KeyCode.S)) {
			this.transform.position += forwardVec * keyboardSpeed;
		}
		
		if (Input.GetKey(KeyCode.A)) {
			this.transform.position -= rightVec * keyboardSpeed;
		}
		
		if (Input.GetKey(KeyCode.D)) {
			this.transform.position += rightVec * keyboardSpeed;
		}

		if (Input.GetKey(KeyCode.Q)) {
			this.transform.Rotate (0, 0, rollSpeed);
		}

		if (Input.GetKey(KeyCode.E)) {
			this.transform.Rotate (0, 0, -rollSpeed);
		}


		if (Input.GetKey(KeyCode.H)) {
			Debug.Log (this.transform.rotation);
		}
	
	}
}
