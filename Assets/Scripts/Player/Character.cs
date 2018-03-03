using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	[SerializeField] private int characterHealth;
	[SerializeField] private float characterSpeed;
	[SerializeField] private int goldCapacity;
	private bool hasGold;
	private int teamId;
	private int maxHealth;
	private PlayerGUI gui;
	[Header("Effects")]
	[SerializeField] private GameObject bloodObject;
	void Start () {
		maxHealth = characterHealth;
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		gui.setHealth (characterHealth);
		gui.setGold (0, goldCapacity);
		hasGold = false;
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
	public void setHasGold(bool goldCarry) { 
		hasGold = goldCarry;
		if (hasGold) {
			gui.setGold (goldCapacity, goldCapacity);
		} 
		else {
			gui.setGold (0, goldCapacity);
		}
	}

}
