using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	[SerializeField] private int characterHealth;
	[SerializeField] private float characterSpeed;
	[SerializeField] private int characterGoldCapacity;
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
		Network.Instantiate(gameObject, new Vector3(0, 30, 0), Quaternion.identity, 0);
	}

	public float getSpeed(){
		return characterSpeed;
	}

	public int getGoldCap(){
		return characterGoldCapacity;
	}

}
