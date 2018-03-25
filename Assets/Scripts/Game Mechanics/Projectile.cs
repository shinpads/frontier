using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	Rigidbody rigidbod;
	[HideInInspector] public int userId;
	bool collided = false;
	void Start() {
		object[] data = GetComponent<PhotonView>().instantiationData;
		userId = (int)data[0];
		rigidbod = gameObject.GetComponent<Rigidbody>();
	}
	void onImpact(Collision col) {
		gameObject.GetComponent<Renderer>().enabled = false;
		rigidbod.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
		PhotonNetwork.Instantiate("dynamiteExplosion", gameObject.transform.position, Quaternion.identity, 0, new object[] {userId});
		StartCoroutine(destroyObject());
	}

	void OnCollisionEnter (Collision col) {
		if (!collided &&  PhotonNetwork.isMasterClient) {
			collided = true;
			onImpact(col);
		}
	}
	private IEnumerator destroyObject() {
		yield return new WaitForSeconds(3f);
		PhotonNetwork.Destroy(gameObject);
	}
}
