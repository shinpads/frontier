using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	[SerializeField] AudioClip collisionSound;
	AudioSource audioSource;
	Rigidbody rigidbod;
	[HideInInspector] public int userId;
	bool collided = false;
	void Start() {
		audioSource = gameObject.GetComponent<AudioSource>();
		rigidbod = gameObject.GetComponent<Rigidbody>();
		if (!PhotonNetwork.isMasterClient) {this.enabled = false; return;}
	}
	void onImpact(Collision col) {
		gameObject.GetComponent<Renderer>().enabled = false;
		rigidbod.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
		audioSource.PlayOneShot(collisionSound);
		GameObject explosion = (GameObject) PhotonNetwork.Instantiate("dynamiteExplosion", gameObject.transform.position, Quaternion.identity, 0);
		explosion.GetComponent<Explosion>().userId = userId;
		StartCoroutine(destroyObject());
	}

	void OnCollisionEnter (Collision col) {
		if (!collided) {
			onImpact(col);
			collided = true;
		}
	}

	private IEnumerator destroyObject() {
		yield return new WaitForSeconds(3f);
		PhotonNetwork.Destroy(gameObject);
	}
}
