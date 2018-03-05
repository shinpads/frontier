using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	private const int LIFE_SPAN = 3;
	private Vector3 currentPosition, lastPosition;
	private float positionDifference;
	private RaycastHit hit;
	private Ray ray;
	private Rigidbody rigidbod;
	[SerializeField] GameObject dirtImpactParticles;
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
			ray = new Ray(lastPosition,rigidbod.velocity.normalized);
			if (Physics.Raycast(ray, out hit, positionDifference)) {
				if (hit.collider.gameObject.tag == "Player") {
					hit.collider.gameObject.GetComponent<NetworkView> ().RPC("setHealth", RPCMode.All, -25);
				} else {
					Network.Instantiate (dirtImpactParticles, ray.GetPoint(hit.distance), Quaternion.EulerAngles(new Vector3(-90, 0, 0)), 0);
				}
				Network.Destroy(gameObject);
			}
			lastPosition = currentPosition;
		}
	}
	private IEnumerator setTimeOutDestroy () {
		yield return new WaitForSeconds(LIFE_SPAN);
		Network.Destroy(gameObject);
	}

}
