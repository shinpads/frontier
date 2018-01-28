using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour {
	Camera playerCamera;
	float shootTime = 10f;
	RaycastHit hit;
	Ray ray;
	Vector3 endpoint;
	float distance;
	[SerializeField] GameObject tipOfGun;
	[SerializeField] GameObject bullet;
	[SerializeField] GameObject testPrefab;

	void Start () {
		playerCamera = gameObject.GetComponent<PlayerController>().playerCamera;
		endpoint = new Vector3(0,0,0);
		distance = 0;
	}	

	void Update () {
		if (Input.GetButtonDown ("Fire1")) {

			//Get Point where bullet will hit
			ray = new Ray(playerCamera.transform.position,playerCamera.transform.forward*100);
			if (Physics.Raycast(ray ,out hit)) {
				endpoint = ray.GetPoint(hit.distance);
			} else {
			endpoint = ray.GetPoint(100);
			}
			//create the bullet at tip of gun
			GameObject shot = (GameObject) Network.Instantiate (bullet, tipOfGun.transform.position,Quaternion.LookRotation(Vector3.Normalize(endpoint-tipOfGun.transform.position)), 0);
			shot.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(endpoint-tipOfGun.transform.position)*10000);	
		}
	}
}
