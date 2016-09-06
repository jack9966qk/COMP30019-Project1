using UnityEngine;
using System.Collections;

public class SunMovement : MonoBehaviour {

	public Vector3 initialPos;
	public Vector3 centrePos;
	public int speed;

	// Use this for initialization
	void Start () {
		this.transform.position = initialPos;
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround (centrePos, Vector3.forward, Time.deltaTime * speed);
	}
}
