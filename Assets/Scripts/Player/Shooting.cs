using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour {
	Camera playerCamera;
	float shootTime = 10f;
	[SerializeField] GameObject bullet;

	void Start () {
		playerCamera = gameObject.GetComponent<PlayerController>().playerCamera;
	}	

	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			Debug.Log("Shoot");
			GameObject shot = (GameObject) Network.Instantiate (bullet, playerCamera.transform.position + playerCamera.transform.forward, Quaternion.identity, 0);
			shot.GetComponent<Rigidbody>().AddForce(playerCamera.transform.forward*1000);
		}
	}
}
