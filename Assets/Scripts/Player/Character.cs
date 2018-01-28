using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	public int characterHealth = 100;
	[SerializeField] PlayerGUI gui;
	bool isDead;
	// Use this for initialization
	void Start () {
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		isDead = false;
		gui.setHealth (characterHealth);
	}	
	// Update is called once per frame
	void Update(){
	}
		
	[RPC]
	void healthLoss(int damage){
		if (!gameObject.GetComponent<NetworkView> ().isMine) {return;}
		if (isDead) {return;}
		characterHealth -= damage;
		gui.setHealth (characterHealth);
		if (characterHealth <= 0) {
			getDead ();
		}
	}
	void getDead(){
		isDead = true;
		Debug.Log ("You got dead");
	}
}
