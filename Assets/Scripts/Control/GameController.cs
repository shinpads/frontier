using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	const int MAX_PLAYERS = 25;
	[SerializeField] GameObject playerPrefab;
	private PhotonView photonView;
	private ArrayList scores = new ArrayList();
	private Teams[] teams = { new Teams (0), new Teams (1), new Teams (2), new Teams (3) };
	[SerializeField]private GameObject[] minecarts = new GameObject[4];
	private Dictionary<int, Teams> userTeam = new Dictionary<int, Teams>();
	private int thisTeam;
	private int thisUserId;
	private Player thisPlayer;
	bool usernameSet = false;
	bool classTypeSet = false;
	bool connected = false;
	bool gameStarted = false;
	int nextTeam = -1;
	Texture2D pixel;
	Color pixelColor;
	void Start () {
		photonView = gameObject.GetComponent<PhotonView>();
		// GUI things
		pixelColor = Color.black;
		pixelColor.a = 0.5f;
		pixel = new Texture2D (1, 1);
		pixel.SetPixel (0, 0, pixelColor);
		pixel.Apply ();
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
					for (int i = 0; i < teams[curTeam].getPlayerCount(); i++) {
						Player curPlayer = teams[curTeam].players[i];
						string lab = curPlayer.getUsername() + " [Choosing...]";
						if (curPlayer.getClassType() != -1) {
							lab = curPlayer.getUsername() + " [" + Global.CHARACTER_NAMES[curPlayer.getClassType()] + "]";
						}
						GUI.Label(new Rect(10 + (210 * curTeam), 30 + (20 * i), 200, 20), lab);
					}
				}
				// Class Selection

						GUI.DrawTexture(new Rect(0, Screen.height - 100, Screen.width, Screen.height), pixel);
					if (GUI.Button(new Rect(10, Screen.height - 90, 150, 80), "Tank")) {
						photonView.RPC("setClassType", PhotonTargets.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_TANK);
					}
					if (GUI.Button(new Rect(170, Screen.height - 90, 150, 80), "Scout")) {
						photonView.RPC("setClassType", PhotonTargets.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_SCOUT);
					}
					if (GUI.Button(new Rect(330, Screen.height - 90, 150, 80), "Thief")) {
						photonView.RPC("setClassType", PhotonTargets.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_THIEF);
					}
					if (GUI.Button(new Rect(490, Screen.height - 90, 150, 80), "Other")) {
						photonView.RPC("setClassType", PhotonTargets.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_OTHER);
					}
					if (GUI.Button(new Rect(650, Screen.height - 90, 150, 80), "Assualt")) {
						photonView.RPC("setClassType", PhotonTargets.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_ASSUALT);
					}
				if (PhotonNetwork.isMasterClient) {
					if (GUI.Button(new Rect(Screen.width - 210, 10, 200, 40), "Start Game") && allClassesPicked()) {
						photonView.RPC("startGame", PhotonTargets.All);
					}
				}
			} else {
				GUI.DrawTexture(new Rect(0, Screen.height-40, 450, Screen.height), pixel);
				for (int curTeam = 0; curTeam < 4; curTeam ++)
					GUI.Label(new Rect(10 + (110 * curTeam), Screen.height - 30, 200, 20), "TEAM " + (curTeam + 1).ToString() + " $" + minecarts[curTeam].GetComponent<Minecart>().getGold());
			}
		}
	}

	public void spawnPlayer() {
		PhotonNetwork.Instantiate("Player", new Vector3(0, 30, 0), Quaternion.identity, 0, new object[] {thisPlayer.getClassType(), thisPlayer.getUserId(), thisTeam});
		//playerObject.GetComponent<Character>().setClass(thisPlayer.getClassType());
		//playerObject.GetComponent<Character> ().setUserId (thisPlayer.getUserId ());
		//playerObject.GetComponent<Character> ().setTeamId (thisTeam);
	}
	private void loadMineCartObjects () {
		for (int i = 0; i < 4; i++) {
			minecarts[i] = GameObject.Find("Mine Cart" + i);
		}
	}
	public void sendCartGoldRPC (int teamId, int amount) {
		photonView.RPC("setCartGold", PhotonTargets.All, teamId, amount);
	}
	public void sendPlayerDeathRPC (int userId) {
		photonView.RPC("addPlayerDeath", PhotonTargets.All, userId);
	}

	public void sendPlayerKillRPC (int userId) {
		photonView.RPC("addPlayerKill", PhotonTargets.All, userId);
	}

	public void sendPlayerAssistRPC (int userId) {
		photonView.RPC("addPlayerAssist", PhotonTargets.All, userId);
	}

	public void sendPlayerGoldStolenRPC (int userId, int gold) {
		photonView.RPC ("addPlayerGoldStolen", PhotonTargets.All, userId, gold);
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
		nextTeam = 0;
		//nextTeam = (nextTeam+1)%4;
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
		userTeam [userId].addPlayerDeath(userId);
	}

	[PunRPC]
	public void addPlayerKill(int userId) {
		userTeam [userId].addPlayerKill (userId);
	}

	[PunRPC]
	public void addPlayerGoldStolen(int userId, int gold) {
		userTeam [userId].addPlayerGoldStolen (userId, gold);
	}

	[PunRPC]
	public void addPlayerAssist(int userId) {
		userTeam [userId].addPlayerAssist (userId);
	}

	public ArrayList getScores() {
		scores.Clear();
		foreach (Teams t in teams) {
			scores.Add(t.getScoreList());
		}
		return scores;
	}

	public Dictionary<int, Teams> getUserTeamDict() {
		return userTeam;

	}

	public bool allClassesPicked() {
		foreach (Teams team in teams) {
			if (!team.teamClassesPicked ()) {
				return false;
			}
		}
		return true;
	}
}
