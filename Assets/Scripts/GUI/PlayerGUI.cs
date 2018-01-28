using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour {
	Text healthText;
	Character character = new Character();
	void Start() {
		healthText = GetComponentInChildren<Text> ();
		setHealth ();
	}
	void Update() {
	}
	public void setHealth(){
		Debug.Log(character.characterHealth);
		healthText.text = "Health: " + character.characterHealth;
	}
}