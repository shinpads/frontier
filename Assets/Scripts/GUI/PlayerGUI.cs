using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour {
	Text healthText;
	public static int health;
	void Start() {
		healthText = GetComponent <Text> ();
		healthText.text = "Health: " + health;
	}
	void Update() {
		health = Character.characterHealth;
		healthText.text = "Health: " + health;
	}
}