using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterNSeconds : MonoBehaviour {
	[SerializeField] float lifeSpan;
	void Start() {
		StartCoroutine(delete(lifeSpan));
	}
	private IEnumerator delete(float lifeSpan) {
		yield return new WaitForSeconds(lifeSpan);
		GameObject.Destroy(gameObject);
	}
}
