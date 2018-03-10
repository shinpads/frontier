using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	private const int HEALTH_INDEX = 0;
	private const int SPEED_INDEX = 1;
	private const int GOLD_CARRY_INDEX = 2;
	private int[,] characterStats = new int[,] { {200, 1, 5}, {75, 2, 4}, {100, 5, 300}, {150, 3, 2}, {125, 4, 1} };
	private int characterHealth;
	private int characterSpeed;
	private int goldCapacity;
	private int goldCarry;
	private int teamId;
	private int maxHealth;
	private int userId;
	private LinkedList<int[]> damagers = new LinkedList<int[]>();
	private PlayerGUI gui;
	private Global earth;
	[SerializeField] private Material mat0, mat1, mat2, mat3;
	[Header("Effects")]
	[SerializeField] private GameObject bloodObject;
	private GameController gameController;
	private MeshRenderer renderer;
	PhotonView photonView;

	void Start () {
		photonView = gameObject.GetComponent<PhotonView>();
		object[] data = photonView.instantiationData;
		setClass((int)data[0]);
		setUserId((int)data[1]);
		setTeamId((int)data[2]);
		maxHealth = characterHealth;
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		gui.setHealth (characterHealth);
		gameController = GameObject.FindWithTag("Control").GetComponent<GameController>();
		goldCarry = 0;
		gui.setGold (goldCarry, goldCapacity);
	}

	public void setUserId(int id) {
		userId = id;
	}

	public void setClass (int reference) {
		characterHealth = characterStats [reference, HEALTH_INDEX];
		characterSpeed = characterStats [reference, SPEED_INDEX];
		goldCapacity = characterStats [reference, GOLD_CARRY_INDEX];
		gameObject.GetComponent<PlayerController>().setSpeed(characterSpeed);
	}

	[PunRPC]
	void setHealth(int dHealth, int enemyId) {
		if (!gameObject.GetComponent<PhotonView> ().isMine) {return;}
		if (dHealth < 0) {
			setDamagers (enemyId, dHealth);
			PhotonNetwork.Instantiate ("BloodParticles", gameObject.transform.position, Quaternion.Euler (gameObject.transform.forward), 0);
		} else if (dHealth > 0) {
			removeDamagers (dHealth);
		}
		characterHealth += dHealth;
		if (characterHealth > maxHealth) {
			characterHealth = maxHealth;
		}
		else if (characterHealth < 0) {
			characterHealth = 0;
		}
		gui.setHealth(characterHealth);
		if (characterHealth == 0) {
			getDead (enemyId);
		}
	}

	void setDamagers(int enemyId, int damage) {
		int[] assistData = { enemyId, damage };
		damagers.AddLast(assistData);
	}

	void removeDamagers(int healed) {
		if (damagers.First == null) {
			return;
		}
		else if (healed == damagers.First.Value [1] * -1) {
			damagers.RemoveFirst ();
		} else if (healed > damagers.First.Value [1] * -1) {
			healed += damagers.First.Value [1];
			damagers.RemoveFirst ();
			removeDamagers (healed);
		}
		else {
			damagers.First.Value [1] += healed;
		}
	}

	void getDead(int enemyId) {
		PhotonNetwork.Destroy (gameObject);
		HashSet<int> assistSet = new HashSet<int> ();
		gameController.sendPlayerDeathRPC (userId);
		gameController.sendPlayerKillRPC (enemyId);
		foreach (int[] enemy in damagers) {
			if (assistSet.Add (enemy [0]) && enemy [0] != enemyId) {
				gameController.sendPlayerAssistRPC (enemy [0]);
			}
		}
		damagers.Clear ();
		gameController.spawnPlayer ();
	}

	public float getSpeed() {
		return characterSpeed;
	}

	public int getGoldCapacity() {
		return goldCapacity;
	}

	public int getTeamId() {
		return teamId;
	}

	public int getUserId() {
		return userId;
	}

	public void setTeamId(int id){
		teamId = id;
		photonView = gameObject.GetComponent<PhotonView> ();
		photonView.RPC ("setCharacterMaterial", PhotonTargets.All, id);
	}
	public void setGoldCarry(int gold) {
		goldCarry = gold;
		gui.setGold (goldCarry, goldCapacity);
	}

	public int getGoldCarry() {
		return goldCarry;
	}

	void OnTriggerStay (Collider col) {
		if (col.gameObject.tag == "Mine Cart") {
			Minecart cart = col.gameObject.GetComponentInParent<Minecart> ();
			int cartId = cart.getTeamId();
			if (cartId != teamId && goldCarry < goldCapacity && cart.getGold() > 0) {
				gui.setInteract ("Press F to Steal Gold");
				if (Input.GetKey (KeyCode.F)) {
					int amount = Mathf.Min(goldCapacity - goldCarry, cart.getGold());
					gameController.sendCartGoldRPC(cartId, -amount);
					photonView.RPC("setGoldCarryRPC", PhotonTargets.All, amount);
					gui.setInteract ("");
				}
			}
			else if (cartId == teamId && goldCarry > 0) {
				gui.setInteract ("Press F to Place Gold");
				if (Input.GetKey (KeyCode.F)) {
					gameController.sendPlayerGoldStolenRPC (userId, goldCarry);
					gameController.sendCartGoldRPC(cartId, goldCarry);
					photonView.RPC("setGoldCarryRPC", PhotonTargets.All, -goldCarry);
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
	[PunRPC]
	void setCharacterMaterial(int id) {
		renderer = gameObject.GetComponent<MeshRenderer> ();
		switch (id) {
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
	[PunRPC]
	void setGoldCarryRPC (int amount) {
		setGoldCarry(goldCarry + amount);
	}
}
