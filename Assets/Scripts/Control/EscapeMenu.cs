using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour {
	[SerializeField] GameObject escapeMenuCanvas;
	private bool enabled = false;
	private GameObject playerObject;
	[Header("UI Elements")]
	public Button buttonContinue;
	public Button buttonDisconnect;
	public Button buttonOptions;
	void Start() {
			buttonContinue.onClick.AddListener(returnFromMenu);
			buttonDisconnect.onClick.AddListener(disconnect);
	}
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			playerObject = gameObject.GetComponent<GameController>().getPlayerObject();
			enabled = !enabled;
			updateEnabled();
		}
	}
	void updateEnabled() {
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
	void returnFromMenu() {
		enabled = false;
		updateEnabled();
	}
	void disconnect() {
		enabled = false;
		PhotonNetwork.Disconnect();
	}
}
