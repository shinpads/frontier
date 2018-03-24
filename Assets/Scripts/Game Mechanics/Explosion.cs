using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
	private int userId;

	void Start() {
		if (!PhotonNetwork.isMasterClient) { this.enabled = false; }
		object[] data = GetComponent<PhotonView>().instantiationData;
		userId = (int)data[0];
		SphereCollider sc = gameObject.GetComponent<SphereCollider>();
		Collider[] allOverlappingColliders = Physics.OverlapSphere(sc.transform.position, sc.radius);
		HashSet<Collider> colliders = new HashSet<Collider>(allOverlappingColliders);
		foreach (Collider col in colliders) {
	 		if (col.gameObject.tag == "Player") {
				col.gameObject.GetComponent<PhotonView> ().RPC("setHealth", PhotonTargets.All, -25, userId);
			}
		}
		StartCoroutine(destroyObject());
	}
	private IEnumerator destroyObject () {
			yield return new WaitForSeconds(4f);
			PhotonNetwork.Destroy(gameObject);
	}
}
