using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	[SerializeField] private int characterHealth;
	[SerializeField] private float characterSpeed;
	[SerializeField] private int goldCapacity;
	private int goldCarry;
	private int teamId;
	private int maxHealth;
	private PlayerGUI gui;
	[Header("Effects")]
	[SerializeField] private GameObject bloodObject;
	void Start () {
		maxHealth = characterHealth;
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		gui.setHealth (characterHealth);
		goldCarry = 0;
		gui.setGold (goldCarry, goldCapacity);
	}

	[RPC]
	void setHealth(int damage) {
		if (!gameObject.GetComponent<NetworkView> ().isMine) {return;}
		if (damage < 0) {
			Network.Instantiate(bloodObject, gameObject.transform.position, Quaternion.Euler(gameObject.transform.forward), 0);
		}
		characterHealth += damage;
		if (characterHealth > maxHealth) {
			characterHealth = maxHealth;
		} 
		else if (characterHealth < 0) {
			characterHealth = 0;
		}
		gui.setHealth(characterHealth);
		if (characterHealth == 0) {
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
		return goldCapacity;
	}

	public int getTeamId(){
		return teamId;
	}

	public void setTeamId(int id){
		teamId = id;
	}
	public void setgoldCarry(int gold) { 
		goldCarry = gold;
		gui.setGold (goldCarry, goldCapacity);
	}

	public int getGoldCarry(){
		return goldCarry;
	}

	void OnTriggerStay (Collider col) {
		if (col.gameObject.name == "cartCollider" && goldCarry != goldCapacity) {
			gui.setInteract ("Press F to Steal Gold");
			if (Input.GetKey (KeyCode.F)) {
				setgoldCarry (goldCapacity);
				gui.setInteract ("");
			}
		} 
	}

	void OnTriggerExit (Collider col) {
		Debug.Log ("bitch");
		if (col.gameObject.name == "cartCollider") {
			gui.setInteract ("");
		}
	}
}
