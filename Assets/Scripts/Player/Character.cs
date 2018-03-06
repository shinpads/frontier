using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	private const int HEALTH_INDEX = 0;
	private const int SPEED_INDEX = 1;
	private const int GOLD_CARRY_INDEX = 2;
	private int[,] characterStats = new int[,] { {200, 1, 5}, {75, 2, 4}, {100, 5, 600}, {150, 3, 2}, {125, 4, 1} };
	private int characterHealth;
	private int characterSpeed;
	private int goldCapacity;
	private int goldCarry;
	private int teamId;
	private int maxHealth;
	private PlayerGUI gui;
	private Global earth;
	[SerializeField] private Material mat0, mat1, mat2, mat3;
	[Header("Effects")]
	[SerializeField] private GameObject bloodObject;
	private GameObject gameController;
	private MeshRenderer renderer;

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
		setMaterial ();
	}
	public void setgoldCarry(int gold) {
		goldCarry = gold;
		gui.setGold (goldCarry, goldCapacity);
	}

	public int getGoldCarry(){
		return goldCarry;
	}

	void OnTriggerStay (Collider col) {
		if (col.gameObject.tag == "Mine Cart") {
			Minecart cart = col.gameObject.GetComponentInParent<Minecart> ();
			int cartId = cart.getTeamId();
			if (cartId != teamId && goldCarry != goldCapacity && cart.getGold() != 0) {
				gui.setInteract ("Press F to Steal Gold");
				if (Input.GetKey (KeyCode.F)) {
					setgoldCarry (cart.loseGold (goldCapacity));
					gui.setInteract ("");
				}
			} 
			else if (cartId == teamId && goldCarry != 0) {
				gui.setInteract ("Press F to Place Gold");
				if (Input.GetKey (KeyCode.F)) {
					cart.setCartGold (goldCarry);
					setgoldCarry (0);
					gui.setInteract ("");
				}
			}
		}
	}

	void OnTriggerExit (Collider col) {
		if (col.gameObject.tag == "Mine Cart") {
			gui.setInteract ("");
		}
	}

	void setMaterial() {
		renderer = gameObject.GetComponent<MeshRenderer> ();
		switch (teamId) {
		case(0):
			renderer.material = mat0;
			break;
		case(1):
			renderer.material = mat1;
			break;
		case(2):
			renderer.material = mat2;
			break;
		case(3):
			renderer.material = mat3;
			break;
		default:
			break;
		}
	}
}
