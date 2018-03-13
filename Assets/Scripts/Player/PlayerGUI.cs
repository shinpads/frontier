using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour {
	[SerializeField] private Text interactText;
	[SerializeField] private Text healthText;
	[SerializeField] private Text goldText;
	[SerializeField] private RawImage crosshair;
	void Start() {
	}

	public void setHealth(int health){
		healthText.text = "Health: " + health;
	}

	public void setGold(int gold, int max) {
		goldText.text = "Gold: " + gold + "/" + max;
	}

	public void setInteract(string message) {
		interactText.text = message;
	}

	public void toggleCrosshair () {
		crosshair.enabled = !crosshair.enabled;
	}

}
