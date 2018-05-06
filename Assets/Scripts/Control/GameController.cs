using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	const int MAX_PLAYERS = 25;
	[SerializeField] GameObject playerPrefab;
	private PhotonView photonView;
	private Teams[] teams = { new Teams (0), new Teams (1), new Teams (2), new Teams (3) };
	private Vector3[] teamSpawns =  { new Vector3(-501.6f, 2.5f, -436.34f), new Vector3(-156.1f, 2.5f, -55.6f), new Vector3(-497.8f, 2.5f, -102.3f), new Vector3(-149f, 2.5f, -430f) };
	[SerializeField]private GameObject[] minecarts = new GameObject[4];
	[Header("Sounds")]
	[SerializeField]private AudioClip eliminatedSound;
	private Dictionary<int, Teams> userTeam = new Dictionary<int, Teams>();
	private int thisTeam;
	private int thisUserId;
	private Player thisPlayer;
	bool usernameSet = false;
	bool classTypeSet = false;
	bool connected = false;
	bool gameStarted = false;
	int nextTeam = -1;
	GameObject playerObject;
	Texture2D pixel;
	Color pixelColor;
	string killNotification = "";
	GUIStyle guiStyle;
	AudioSource audioSource;
	void Start () {
		photonView = gameObject.GetComponent<PhotonView>();
		// GUI things
		pixelColor = Color.black;
		pixelColor.a = 0.5f;
		pixel = new Texture2D (1, 1);
		pixel.SetPixel (0, 0, pixelColor);
		pixel.Apply ();
		audioSource = gameObject.GetComponent<AudioSource>();
		guiStyle = new GUIStyle();
		guiStyle.alignment = TextAnchor.MiddleCenter;
		guiStyle.fontSize = 20;
		guiStyle.normal.textColor = Color.white;
		loadMineCartObjects();
	}

	void Update () {
		if (PhotonNetwork.connected) {
			if (!connected) { onConnected(); }
		}
		if (connected) {
			// if server add to next team (no need to call RPC to itself)
			/*
			if (Network.) {
				nextTeam = (nextTeam+1)%4;
				photonView.RPC("addToTeam", PhotonTargetss.AllBuffered, thisUserId, nextTeam, thisUsername);
			} else {
				// get server to setup team
			} */
		}
	}
	private void onConnected() {
		thisUserId = PhotonNetwork.player.ID;
		classTypeSet = false;
		connected = true;
		loadMineCartObjects();
		photonView.RPC("setupTeam", PhotonTargets.AllBuffered, thisUserId, Global.username);
	}
	private void OnGUI() {
		if (connected) {
			if (!gameStarted) {
				// Teams
				GUI.DrawTexture(new Rect(0, 0, 850, 220), pixel);
				for (int curTeam = 0; curTeam < 4; curTeam ++) {
					GUI.Label(new Rect(10 + (210 * curTeam), 10, 200, 20), "TEAM " + (curTeam + 1).ToString());
					int i = 0;
					foreach (KeyValuePair<int, Player> entry in teams[curTeam].getPlayerDict()) {
						Player curPlayer = entry.Value;
						string lab = curPlayer.getUsername() + " [Choosing...]";
						if (curPlayer.getClassType() != -1) {
							lab = curPlayer.getUsername() + " [" + Global.CHARACTER_NAMES[curPlayer.getClassType()] + "]";
						}
						GUI.Label(new Rect(10 + (210 * curTeam), 30 + (20 * i), 200, 20), lab);
						i++;
					}
				}
				// Class Selection

						GUI.DrawTexture(new Rect(0, Screen.height - 100, Screen.width, Screen.height), pixel);
					if (GUI.Button(new Rect(10, Screen.height - 90, 150, 80), Global.CHARACTER_NAMES[0])) {
						photonView.RPC("setClassType", PhotonTargets.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_TANK);
					}
					if (GUI.Button(new Rect(170, Screen.height - 90, 150, 80), Global.CHARACTER_NAMES[1])) {
						photonView.RPC("setClassType", PhotonTargets.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_SCOUT);
					}
					if (GUI.Button(new Rect(330, Screen.height - 90, 150, 80), Global.CHARACTER_NAMES[2])) {
						photonView.RPC("setClassType", PhotonTargets.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_THIEF);
					}
					if (GUI.Button(new Rect(490, Screen.height - 90, 150, 80), Global.CHARACTER_NAMES[3])) {
						photonView.RPC("setClassType", PhotonTargets.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_OTHER);
					}
					if (GUI.Button(new Rect(650, Screen.height - 90, 150, 80), Global.CHARACTER_NAMES[4])) {
						photonView.RPC("setClassType", PhotonTargets.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_ASSUALT);
					}
				if (PhotonNetwork.isMasterClient) {
					if (GUI.Button(new Rect(Screen.width - 210, 10, 200, 40), "Start Game") && allClassesPicked()) {
						photonView.RPC("startGame", PhotonTargets.All);
						foreach (GameObject cart in minecarts) {
							cart.GetComponent<Minecart> ().startCarts ();
						}
					}
				}
			} else {
				GUI.DrawTexture(new Rect(0, Screen.height-40, 450, Screen.height), pixel);
				for (int curTeam = 0; curTeam < 4; curTeam ++) {
					GUI.Label(new Rect(10 + (110 * curTeam), Screen.height - 30, 200, 20), "TEAM " + (curTeam + 1).ToString() + " $" + minecarts[curTeam].GetComponent<Minecart>().getGold());
				}
				if (killNotification != "") {
					GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/4, 200, 50), killNotification, guiStyle);
				}
			}
		}
	}

	public void spawnPlayer() {
		playerObject =  PhotonNetwork.Instantiate("Player", teamSpawns[thisTeam], Quaternion.identity, 0, new object[] {thisPlayer.getClassType(), thisPlayer.getUserId(), thisTeam});
		//playerObject.GetComponent<Character>().setClass(thisPlayer.getClassType());
		//playerObject.GetComponent<Character> ().setUserId (thisPlayer.getUserId ());
		//playerObject.GetComponent<Character> ().setTeamId (thisTeam);
	}
	private void loadMineCartObjects () {
		for (int i = 0; i < 4; i++) {
			minecarts[i] = GameObject.Find("Mine Cart" + i);
		}
	}

	public void sendHitMarked(int shooterId) {
		photonView.RPC("hitMarked", PhotonTargets.All, shooterId);
	}

	public void sendCartGoldRPC (int teamId, int amount) {
		photonView.RPC("setCartGold", PhotonTargets.All, teamId, amount);
	}
	public void sendPlayerDeathRPC (int userId) {
		photonView.RPC("addPlayerDeath", PhotonTargets.All, userId);
	}

	public void sendPlayerKillRPC (int userId, int deadManId) {
		photonView.RPC("addPlayerKill", PhotonTargets.All, userId, deadManId);
	}

	public void sendPlayerAssistRPC (int userId) {
		photonView.RPC("addPlayerAssist", PhotonTargets.All, userId);
	}

	public void sendPlayerGoldStolenRPC (int userId, int gold) {
		photonView.RPC ("addPlayerGoldStolen", PhotonTargets.All, userId, gold);
	}

	public void sendSetCartGold(int teamId, int gold) {
		photonView.RPC ("setCartGold", PhotonTargets.All, teamId, gold);
	}

	public void sendDestroyGold(GameObject goldObject) {
		photonView.RPC ("destroyGold", PhotonTargets.MasterClient, goldObject.GetPhotonView().viewID);
	}

	public void sendInstantiateGold(string name, Vector3 location, Quaternion rotation, int[] goldBreakdown) {
		photonView.RPC ("instantiateGold", PhotonTargets.MasterClient, name, location, rotation, goldBreakdown);
	}

	[PunRPC]
	void instantiateGold(string name, Vector3 location, Quaternion rotation, int[] goldBreakdown) {
		Vector3 dropSpot;
		for (int i = 0; i < 4; i++) {
			if (goldBreakdown [i] > 0) {
				switch (i) {
				case 0:
					dropSpot = gameObject.transform.forward * 2;
					break;
				case 1:
					dropSpot = gameObject.transform.right * -2;
					break;
				case 2:
					dropSpot = gameObject.transform.right * 2;
					break;
				default:
					dropSpot = gameObject.transform.position;
					break;
				}
				PhotonNetwork.Instantiate (name, location + dropSpot, rotation, 0, new object[] { i, goldBreakdown [i] });
			}
		}
	}

	[PunRPC]
	void destroyGold(int goldInstanceId) {
		GameObject[] allFreeGold = GameObject.FindGameObjectsWithTag ("freeGold");
		foreach (GameObject freeGold in allFreeGold) {
			if (freeGold.GetPhotonView().viewID == goldInstanceId) {
				PhotonNetwork.Destroy (freeGold);
				return;
			}
		}
	}

	[PunRPC]
	public void startGame () {
		GameObject.FindWithTag("MenuCamera").SetActive(false);
		spawnPlayer();
		gameStarted = true;
	}
	[PunRPC]
	public void addToTeam (int userId, int team, string username) {
		if (team < 0 || team > 3) {
			Debug.Log("Invalid team on addToTeam");
			return;
		}
		Player newPlayer = teams[team].addPlayer(userId, username);
		userTeam[userId] = teams[team];
		if (userId == PhotonNetwork.player.ID) {
			thisTeam = team;
			thisPlayer = newPlayer;
		}
	}

	// SERVER ONLY
	[PunRPC]
	public void setupTeam (int userId, string username) {
		if (!PhotonNetwork.isMasterClient) { return; }
		if (userId >= MAX_PLAYERS) {
			Debug.Log("Invalid userId on setupTeam");
			return;
		}

		nextTeam = (nextTeam+1)%4;
		photonView.RPC("addToTeam", PhotonTargets.AllBuffered, userId, nextTeam, username);

	}

	[PunRPC]
	public void setClassType (int userId, int teamId, int classType) {
		Player player = teams[teamId].findPlayerByUserId (userId);
		if (player == null) { return; }
		player.setClassType (classType);
		if (userId == thisUserId) {
			classTypeSet = true;
		}
	}

	[PunRPC]
	public void setCartGold (int teamId, int gold) {
		minecarts[teamId].GetComponent<Minecart>().setCartGold(gold);
	}

	[PunRPC]
	public void addPlayerDeath(int userId) {
		userTeam [userId].findPlayerByUserId (userId).addDeath ();
	}

	[PunRPC]
	public void addPlayerKill(int userId, int deadManId) {
		userTeam [userId].findPlayerByUserId (userId).addKill ();
		if (userId == thisUserId) {
			playerObject.GetComponentInChildren<PlayerGUI> ().killMarked ();
			audioSource.PlayOneShot(eliminatedSound);
			killNotification = "Killed " + userTeam [deadManId].findPlayerByUserId (deadManId).getUsername();
			StartCoroutine(clearKillNotification());
		}
	}

	[PunRPC]
	public void addPlayerGoldStolen(int userId, int gold) {
		userTeam [userId].findPlayerByUserId (userId).addGoldStolen (gold);
	}

	[PunRPC]
	public void addPlayerAssist(int userId) {
		userTeam [userId].findPlayerByUserId (userId).addAssist ();
	}

	[PunRPC]
	public void hitMarked(int shooterId) {
		if (shooterId == thisUserId) {
			playerObject.GetComponentInChildren<PlayerGUI> ().hitMarked ();
		}
	}

	public Teams getUserTeam(int userId) {
		return userTeam [userId];
	}

	public bool allClassesPicked() {
		foreach (Teams team in teams) {
			if (!team.teamClassesPicked ()) {
				return false;
			}
		}
		return true;
	}
	private IEnumerator clearKillNotification() {
		yield return new WaitForSeconds(2f);
		killNotification = "";
	}

	public int getThisTeam() { return thisTeam; }
}
