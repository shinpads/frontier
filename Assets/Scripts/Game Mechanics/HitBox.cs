using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {
	[SerializeField] private float damageMultiplier = 1.0f;
	[SerializeField] private GameObject playerObject;

	public float getDamageMultipler() { return damageMultiplier; }
	public GameObject getPlayerObject() { return playerObject; }
}
