using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
	[HideInInspector] public int userId;
	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<PhotonView> ().RPC("setHealth", PhotonTargets.All, -50, userId);
		}
	}
}
