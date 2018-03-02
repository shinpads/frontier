using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	private int characterHealth = 100;
	[SerializeField] private PlayerGUI gui;
	[Header("Effects")]
	[SerializeField] private GameObject bloodObject;
	void Start () {
		gui.setHealth (characterHealth);
	}

	[RPC]
	void setHealth(int damage) {
		if (!gameObject.GetComponent<NetworkView> ().isMine) {return;}
		characterHealth += damage;
		gui.setHealth(characterHealth);
		if (damage < 0) {
			Network.Instantiate(bloodObject, gameObject.transform.position, Quaternion.Euler(gameObject.transform.forward), 0);
		}
		if (characterHealth <= 0) {
			getDead ();
		}
	}
	void getDead(){
		Network.Destroy(gameObject);
		Network.Instantiate(Resources.Load("Prefabs/Player"), new Vector3(0, 30, 0), Quaternion.identity, 0);
	}

}
