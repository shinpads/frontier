using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	float sensitivity = 2.0f; 
	public Camera playerCamera;
	Rigidbody playerRigidbody;
	Vector3 rotationX, rotationY;
	float rotationXValue;
	void Start () {
		playerRigidbody = GetComponent<Rigidbody> ();
		playerRigidbody.freezeRotation = true;
	}

	void Update () {
		rotationY = new Vector3 (0f, Input.GetAxisRaw ("Mouse X"), 0f) * sensitivity;
		rotationX = new Vector3 (Input.GetAxisRaw ("Mouse Y"), 0f, 0f) * -sensitivity;
		playerCamera.transform.Rotate (rotationX);

	}
	void FixedUpdate () {
		playerRigidbody.MoveRotation (playerRigidbody.rotation * Quaternion.Euler (rotationY));
	}
}
