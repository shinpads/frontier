using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	public int characterHealth = 100;
	[SerializeField] PlayerGUI gui;

	void Start () {

		gui.setHealth (characterHealth);
	}	

	void Update(){
	}
		
	[RPC]
	void healthLoss(int damage){
		if (!gameObject.GetComponent<NetworkView> ().isMine) {return;}
		characterHealth -= damage;
		gui.setHealth (characterHealth);
		if (characterHealth <= 0) {
			getDead ();
		}
	}
	void getDead(){
		Network.Destroy(gameObject);
		Network.Instantiate(Resources.Load("Prefabs/Player"), new Vector3(0, 30, 0), Quaternion.identity, 0);
	}
}
