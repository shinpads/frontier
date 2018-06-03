using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour {
	public GameObject mainCamera;
	public GameObject player;
	Vector3 startingPosition;
	public float maxAmount = 0.5f;
	public float speed;
	public float amount;
	void Start() {
		startingPosition = gameObject.transform.localPosition;
	}
	void Update () {
		if (!player.GetComponent<PlayerController>().getFreeze()) {
			float movementX = Mathf.Clamp(Input.GetAxis("Mouse X") * amount, -maxAmount, maxAmount);
			float movementY = Mathf.Clamp(Input.GetAxis("Mouse Y") * amount, -maxAmount, maxAmount);
			Vector3 finalPosition = new Vector3(movementX, movementY, 0);
			gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, finalPosition + startingPosition, Time.deltaTime * speed);
		}
	}
}
