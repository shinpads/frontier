using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour {
	Camera playerCamera;
	float shootTime = 10f;
    public const float SHOT_DELAY = 0.5f;
	RaycastHit hit;
	Ray ray;
	Vector3 endpoint;
	float distance;
    bool canshoot = true;
	[SerializeField] GameObject tipOfGun;
	NetworkView networkview;
	void Start () {
		playerCamera = gameObject.GetComponent<PlayerController>().playerCamera;
		endpoint = new Vector3(0,0,0);
		distance = 0;
		networkview = gameObject.GetComponent<NetworkView>();
	}	

	void Update () {
		if (!networkview.isMine) { return; }
		if (Input.GetButtonDown ("Fire1") && canshoot == true) {
            //Get Point where bullet will hit
            StartCoroutine(delayedShooting());
			ray = new Ray(playerCamera.transform.position,playerCamera.transform.forward*100);
			if (Physics.Raycast(ray ,out hit)) {
				endpoint = ray.GetPoint(hit.distance);
			} else {
			endpoint = ray.GetPoint(100);
			}

			gameObject.GetComponent<NetworkView>().RPC("shoot",RPCMode.All, tipOfGun.transform.position,endpoint);
		}
	}
	[RPC]
	private void shoot(Vector3 start, Vector3 end) {
		Debug.Log("calling shoot RPC");
		if(!Network.isServer) { return; }
		//create the bullet at tip of gun
		GameObject shot = (GameObject) Network.Instantiate ((GameObject)Resources.Load("Prefabs/Bullet"), start,Quaternion.LookRotation(Vector3.Normalize(end-start)), 0);
		shot.GetComponent<Rigidbody>().velocity = Vector3.Normalize(end-start)*300;
	}
    private IEnumerator delayedShooting(){
        canshoot = false;
        yield return new WaitForSeconds(SHOT_DELAY);
        canshoot = true;
    }
}
