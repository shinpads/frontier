using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour {
	Text healthText;
	int health;
	void Start() {
		healthText = GetComponentInChildren<Text> ();
	}
	void Update() {
	}
	public void setHealth(int health){
		healthText.text = "Health: " + health;
	}
}