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
	void setHealth(){
		health = gameObject.GetComponentInParent<Character> ().characterHealth;
		healthText.text = "Health: " + health;
	}
}