using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
	private int userId;
	[SerializeField] AudioClip explosionSound;
	void Start() {
		gameObject.GetComponent<AudioSource>().PlayOneShot(explosionSound);
		if (!PhotonNetwork.isMasterClient) { this.enabled = false; return;}
		object[] data = GetComponent<PhotonView>().instantiationData;
		userId = (int)data[0];
		Collider[] allOverlappingColliders = Physics.OverlapSphere(gameObject.transform.position, 7);
		HashSet<Collider> colliders = new HashSet<Collider>(allOverlappingColliders);
		foreach (Collider col in colliders) {
	 		if (col.gameObject.tag == "Player") {
				col.gameObject.GetComponent<PhotonView> ().RPC("setHealth", PhotonTargets.All, -100, userId, Global.SOUND_TYPE.DEFAULT_DAMAGE);
			}
		}
		StartCoroutine(destroyObject());
	}
	private IEnumerator destroyObject () {
			yield return new WaitForSeconds(4f);
			PhotonNetwork.Destroy(gameObject);
	}
}
