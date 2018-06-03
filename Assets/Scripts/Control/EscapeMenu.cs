using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenu : MonoBehaviour {
	[SerializeField] GameObject escapeMenuCanvas;
	private bool enabled = false;
	private GameObject playerObject;
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			playerObject = gameObject.GetComponent<GameController>().getPlayerObject();
			enabled = !enabled;
			if (enabled) {
				Screen.lockCursor = false;
				Cursor.visible = true;
				escapeMenuCanvas.SetActive(true);
				playerObject.GetComponent<Shooting>().setFreeze(true);
				playerObject.GetComponent<PlayerController>().setFreeze(true);
			} else {
				Screen.lockCursor = true;
				Cursor.visible = false;
				escapeMenuCanvas.SetActive(false);
				playerObject.GetComponent<Shooting>().setFreeze(false);
				playerObject.GetComponent<PlayerController>().setFreeze(false);
			}
		}
	}
}
