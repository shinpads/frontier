using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour {
	[SerializeField] Text healthText;
	void Start() {
	}
	void Update() {
	}
	public void setHealth(int health){
		if (health < 0) {
			health = 0;
		}
		healthText.text = "Health: " + health;
	}
}