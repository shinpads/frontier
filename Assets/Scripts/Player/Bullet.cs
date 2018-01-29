using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	private const int LIFE_SPAN = 3;
	Vector3 currentPosition, lastPosition;
	float positionDifference;
	RaycastHit hit;
	Rigidbody rigidbod;
	void Start () {
		if(!Network.isServer) { enabled = false; }
		currentPosition = gameObject.transform.position;
		lastPosition = gameObject.transform.position;
		rigidbod = gameObject.GetComponent<Rigidbody>();
		rigidbod.detectCollisions = false;
		StartCoroutine(setTimeOutDestroy());
	}
	void FixedUpdate() {
		gameObject.transform.rotation = Quaternion.LookRotation(rigidbod.velocity.normalized);
		currentPosition = gameObject.transform.position;
		positionDifference = (currentPosition - lastPosition).magnitude;
		// check if there was a collision in the last 0.1 units
		if (positionDifference > 0.1f) {
			if (Physics.Raycast(lastPosition,rigidbod.velocity.normalized, out hit, positionDifference)) {
				Network.Destroy(gameObject);
				if (hit.collider.gameObject.name == "Player(Clone)") {
					hit.collider.gameObject.GetComponent<NetworkView> ().RPC("healthLoss", RPCMode.All, 25);
				}
			}
			lastPosition = currentPosition;
		}
	}
	private IEnumerator setTimeOutDestroy () {
		yield return new WaitForSeconds(LIFE_SPAN);
		Network.Destroy(gameObject);
	}

}
