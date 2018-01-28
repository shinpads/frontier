using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	public int characterHealth = 100;
	PlayerGUI playerGUI;
	// Use this for initialization
	void Start () {
		playerGUI = gameObject.GetComponentInChildren<PlayerGUI>();
	}
	
	// Update is called once per frame
	void Update(){
		
	}

	public void healthLoss(){
		characterHealth--;
		playerGUI.setHealth();
	}

}
