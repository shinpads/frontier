using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
	[HideInInspector] public int userId;
	bool doDamage = true;
	void Start() {
		if (!PhotonNetwork.isMasterClient) { this.enabled = false; }
		StartCoroutine(destroyObject());
	}
	void OnTriggerEnter(Collider col) {
		if (!doDamage) { return; }
		if (col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<PhotonView> ().RPC("setHealth", PhotonTargets.All, -50, userId);
		}
	}
	private IEnumerator removeCollider() {
		yield return new WaitForSeconds(0.05f);
		doDamage = false;
	}
	private IEnumerator destroyObject () {
			yield return new WaitForSeconds(5f);
			PhotonNetwork.Destroy(gameObject);
	}
}
