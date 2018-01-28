using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	private const int LIFE_SPAN = 3;

	void Start () {
		StartCoroutine(setTimeOutDestroy());
	}

	void OnCollisionEnter (Collision collision) {
		if (!Network.isServer){ return; }
		Network.Destroy(gameObject);
		if (collision.gameObject.name == "Player(Clone)"){
				collision.gameObject.GetComponent<NetworkView> ().RPC("healthLoss", RPCMode.All, 5);
		}
	}

	private IEnumerator setTimeOutDestroy () {
		yield return new WaitForSeconds(LIFE_SPAN);
		Network.Destroy(gameObject);
	}

}
