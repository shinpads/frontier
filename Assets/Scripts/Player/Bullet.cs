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
	if (collision.gameObject.name == "Player(Clone)"){
			Debug.Log("You hit a heck boi");
	}
	Network.Destroy(gameObject);
	}

	private IEnumerator setTimeOutDestroy () {
		yield return new WaitForSeconds(LIFE_SPAN);
		Network.Destroy(gameObject);
	}

}
