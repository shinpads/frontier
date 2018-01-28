using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : MonoBehaviour {
	private const int LIFE_SPAN = 3;
	Character player;

	void Start () {
		StartCoroutine(setTimeOutDestroy());
	}

	void OnCollisionEnter (Collision collision) {
		if (!Network.isServer){return;}
		if (collision.gameObject.name == "Player(Clone)" && collision.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer){
			collision.gameObject.GetComponent<Character>().healthLoss();
		}
		Network.Destroy(gameObject);
	}

	private IEnumerator setTimeOutDestroy () {
		yield return new WaitForSeconds(LIFE_SPAN);
		Network.Destroy(gameObject);
	}

}
