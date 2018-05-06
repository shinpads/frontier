using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CinematicEffects;

public class Character : MonoBehaviour {
	private const int HEALTH_INDEX = 0;
	private const int SPEED_INDEX = 1;
	private const int GOLD_CARRY_INDEX = 2;
	private int[,] characterStats = new int[,] { {200, 9, 5}, {75, 9, 4}, {100, 9, 2000}, {150, 9, 2}, {125, 9, 1} };
	private int characterRef;
	private int characterHealth;
	private int characterSpeed;
	private int goldCapacity;
	private int[] goldBreakdown = new int[4];
	private int goldCarry;
	private int teamId;
	private int maxHealth;
	private int userId;
	private bool dead = false;
	private LinkedList<int[]> damagers = new LinkedList<int[]>();
	private PlayerGUI gui;
	private Global earth;
	[SerializeField] private Material mat0, mat1, mat2, mat3;
	[Header("Effects")]
	[SerializeField] private GameObject bloodObject;
	private GameController gameController;
	private MeshRenderer renderer;
	PhotonView photonView;
	LensAberrations bloodCameraEffect;

	void Start () {
		photonView = gameObject.GetComponent<PhotonView>();
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		object[] data = photonView.instantiationData;
		setUserId((int)data[1]);
		if (userId == PhotonNetwork.player.ID) {
			gameObject.GetComponent<PhotonView> ().RPC ("setClass", PhotonTargets.All, (int)data [0]);
			gameObject.GetComponent<PhotonView>().RPC("setTeamId",PhotonTargets.All,(int)data[2]);
		}
		gui.setHealth (characterHealth);
		gameController = GameObject.FindWithTag("Control").GetComponent<GameController>();
		goldCarry = 0;
		goldBreakdown [teamId] = -1;
		gui.setGold (0, goldCapacity);
		bloodCameraEffect = gameObject.GetComponent<PlayerController>().playerCamera.GetComponent<LensAberrations>();
	}

	public void setUserId(int id) {
		userId = id;
	}

	[PunRPC]
	public void setClass (int reference) {
		characterRef = reference;
		characterHealth = characterStats [reference, HEALTH_INDEX];
		characterSpeed = characterStats [reference, SPEED_INDEX];
		goldCapacity = characterStats [reference, GOLD_CARRY_INDEX];
		gameObject.GetComponent<PlayerController>().setSpeed(characterSpeed);
		gameObject.GetComponent<MeshController>().setClass(reference);
		maxHealth = characterHealth;
	}

	[PunRPC]
	void setHealth(int dHealth, int enemyId) {
		if (gameObject.GetComponent<PhotonView> ().isMine) {
			if (dHealth < 0) {
				setDamagers (enemyId, dHealth);
				gameController.sendHitMarked (enemyId);
				PhotonNetwork.Instantiate ("BloodParticles", gameObject.transform.position, Quaternion.Euler (gameObject.transform.forward), 0);
				bloodCameraEffect.vignette.intensity = ((maxHealth - characterHealth) / (float)maxHealth) * 1.5f;
				StartCoroutine(fadeBlood(bloodCameraEffect.vignette.intensity, 0f, 1.4f));
			} else if (dHealth > 0) {
				removeDamagers (dHealth);
			}
		}
		if (PhotonNetwork.isMasterClient || gameObject.GetComponent<PhotonView> ().isMine) {
			characterHealth += dHealth;
			if (characterHealth > maxHealth) {
				characterHealth = maxHealth;
			}
			else if (characterHealth < 0) {
				characterHealth = 0;
			}
		}
		if (gameObject.GetComponent<PhotonView> ().isMine) {
			gui.setHealth(characterHealth);
			if (characterHealth == 0 && !dead) {
				dead = true;
				getDead (enemyId);
			}
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
		if (goldCarry > 0) {
			gameController.sendInstantiateGold ("goldPiece", gameObject.transform.position, Quaternion.identity, goldBreakdown);
		}
		PhotonNetwork.Destroy (gameObject);
		HashSet<int> assistSet = new HashSet<int> ();
		gameController.sendPlayerDeathRPC (userId);
		gameController.sendPlayerKillRPC (enemyId, userId);
		foreach (int[] enemy in damagers) {
			if (assistSet.Add (enemy [0]) && enemy [0] != enemyId) {
				gameController.sendPlayerAssistRPC (enemy [0]);
			}
		}
		damagers.Clear ();
		gameController.spawnPlayer ();
	}

	public int getSpeed() {
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

	public int getMaxHealth() {
		return maxHealth;
	}

	public int getCurrentHealth() {
		return characterHealth;
	}

	[PunRPC]
	public void setTeamId(int id){
		teamId = id;
		photonView = gameObject.GetComponent<PhotonView> ();
		photonView.RPC ("setCharacterMaterial", PhotonTargets.All, id);
	}

	public void setGoldCarry(int teamId, int gold) {
		if (gold != -goldCarry) {
			goldBreakdown [teamId] += gold;
		} else {
			goldBreakdown = new int[4];
		}
		goldCarry += gold;
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
					setGoldCarry(cartId, amount);
					gui.setInteract ("");
				}
			}
			else if (cartId == teamId && goldCarry > 0) {
				gui.setInteract ("Press F to Place Gold");
				if (Input.GetKey (KeyCode.F)) {
					gameController.sendPlayerGoldStolenRPC (userId, goldCarry);
					gameController.sendCartGoldRPC(cartId, goldCarry);
					setGoldCarry (cartId, -goldCarry);
					gui.setInteract ("");
				}
			}
		}
		else if (col.gameObject.tag == "freeGold") {
			DroppedGold gold = col.gameObject.GetComponent<DroppedGold> ();
			if (gold.getTeamId () != teamId) {
				gui.setInteract ("Press F to Pick Up Gold");
				if (Input.GetKey (KeyCode.F)) {
					setGoldCarry(gold.getTeamId(), gold.pickUpGold(goldCarry, goldCapacity));
					gui.setInteract ("");
				}
			}
		} else if (col.gameObject.tag == "bearTrap" && col.gameObject.GetComponent<BearTrap>().getTeamId() != teamId && Input.GetKey (KeyCode.F) ) {
			col.gameObject.GetComponent<BearTrap>().trapOpened();
		}
	}

	void OnTriggerExit (Collider col) {
		if (col.gameObject.tag == "Mine Cart" || col.gameObject.tag == "freeGold") {
			gameObject.GetComponentInChildren<PlayerGUI>().setInteract ("");
		}
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "freeGold") {
			DroppedGold gold = col.gameObject.GetComponent<DroppedGold> ();
			if (gold.getTeamId () == teamId) {
				gold.returnGold ();
			}
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

	private IEnumerator fadeBlood(float startValue, float endValue, float time) {
		float startTime = Time.time;
		while (Time.time < startTime + time) {
			bloodCameraEffect.vignette.intensity = Mathf.Lerp(startValue, endValue, (Time.time - startTime) / time);
			yield return new WaitForEndOfFrame();
		}
		bloodCameraEffect.vignette.intensity = endValue;
	}

	public void displayMessage(string message) {
		gui.setInteract (message);
	}
}
