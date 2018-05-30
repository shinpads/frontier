using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {
	[SerializeField] private float damageMultiplier = 1.0f;
	[SerializeField] private GameObject playerObject;
	[SerializeField] private bool isHead;

	public float getDamageMultipler() { return damageMultiplier; }
	public GameObject getPlayerObject() { return playerObject; }
	public bool getIsHead() { return isHead; }
}
