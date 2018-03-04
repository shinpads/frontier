using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	private const int HEALTH_INDEX = 0;
	private const int SPEED_INDEX = 1;
	private const int GOLD_CARRY_INDEX = 2;
	// [Tank, Scout, Thief, Other, Assualt]
	private int[,] characterStats = new int[,] { {200, 1, 5}, {75, 2, 4}, {100, 5, 3}, {150, 3, 2}, {125, 4, 1} };
	private int characterHealth;
	private int characterSpeed;
	private int goldCapacity;
	private int goldCarry;
	private int teamId;
	private int maxHealth;
	private PlayerGUI gui;
	private Global earth;
	[Header("Effects")]
	[SerializeField] private GameObject bloodObject;
	private GameObject gameController;

	void Start () {
		maxHealth = characterHealth;
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		gui.setHealth (characterHealth);
		gameController = GameObject.FindWithTag("Control");
		goldCarry = 0;
		gui.setGold (goldCarry, goldCapacity);
	}

	public void setClass (int reference) {
		characterHealth = characterStats [reference, HEALTH_INDEX];
		characterSpeed = characterStats [reference, SPEED_INDEX];
		goldCapacity = characterStats [reference, GOLD_CARRY_INDEX];
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
		gameController.GetComponent<GameController>().spawnPlayer();
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
		if (col.gameObject.name == "cartCollider") {
			gui.setInteract ("");
		}
	}
}
