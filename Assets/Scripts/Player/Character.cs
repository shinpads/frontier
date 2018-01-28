using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	public int characterHealth = 100;
	PlayerGUI gui;
	// Use this for initialization
	void Start () {
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		gui.setHealth (characterHealth);
	}
	
	// Update is called once per frame
	void Update(){
	}
	public void healthLoss(int damage){
		characterHealth -= damage;
		gui.setHealth (characterHealth);
		if (characterHealth <= 0) {
			getDead ();
		}
	}
	void getDead(){
		Debug.Log ("You got dead");
	}
}
