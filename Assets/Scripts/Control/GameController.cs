using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	const int MAX_PLAYERS = 25;
	[SerializeField] GameObject playerPrefab;
	private NetworkView networkView;
	private ArrayList scores = new ArrayList();
	private Teams[] teams = { new Teams (0), new Teams (1), new Teams (2), new Teams (3) };
	[SerializeField]private GameObject[] minecarts = new GameObject[4];
	private Dictionary<int, Teams> userTeam = new Dictionary<int, Teams>();
	private string thisUsername;
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
		thisUsername = "Player";
		networkView = gameObject.GetComponent<NetworkView>();
		// GUI things
		pixelColor = Color.black;
		pixelColor.a = 0.5f;
		pixel = new Texture2D (1, 1);
		pixel.SetPixel (0, 0, pixelColor);
		pixel.Apply ();
		loadMineCartObjects();
	}

	void Update () {
		if (Network.isServer || Network.isClient) {
			if (!connected) { onConnected(); }
		}
		if (connected) {
			if (!usernameSet) {
				if (Input.GetKeyDown(KeyCode.Return)) {
					if (thisUsername.Length > 0) {
						usernameSet = true;
						// if server add to next team (no need to call RPC to itself)
						if (Network.isServer) {
							nextTeam = (nextTeam+1)%4;
							networkView.RPC("addToTeam", RPCMode.AllBuffered, thisUserId, nextTeam, thisUsername);
						} else {
							// get server to setup team
							networkView.RPC("setupTeam", RPCMode.Server, thisUserId, thisUsername);
						}
					} else {
						thisUsername = "Too Short";
					}
				}
			}
		}
	}
	private void onConnected() {
		thisUserId = int.Parse(Network.player.ToString());
		classTypeSet = false;
		connected = true;
		loadMineCartObjects();
	}
	private void OnGUI() {
		if (connected) {
			if (!gameStarted) {
				if (!usernameSet) {
					thisUsername = GUI.TextField(new Rect(10,10, 200, 30), thisUsername, 15);
					return;
				}
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
				if (!classTypeSet) {
					GUI.DrawTexture(new Rect(0, Screen.height - 100, Screen.width, Screen.height), pixel);
					if (GUI.Button(new Rect(10, Screen.height - 90, 150, 80), "Tank")) {
						networkView.RPC("setClassType", RPCMode.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_TANK);
					}
					if (GUI.Button(new Rect(170, Screen.height - 90, 150, 80), "Scout")) {
						networkView.RPC("setClassType", RPCMode.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_SCOUT);
					}
					if (GUI.Button(new Rect(330, Screen.height - 90, 150, 80), "Thief")) {
						networkView.RPC("setClassType", RPCMode.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_THIEF);
					}
					if (GUI.Button(new Rect(490, Screen.height - 90, 150, 80), "Other")) {
						networkView.RPC("setClassType", RPCMode.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_OTHER);
					}
					if (GUI.Button(new Rect(650, Screen.height - 90, 150, 80), "Assualt")) {
						networkView.RPC("setClassType", RPCMode.AllBuffered, thisUserId, thisTeam, Global.CHARACTER_ASSUALT);
					}
				}
				if (Network.isServer) {
					if (GUI.Button(new Rect(Screen.width - 210, 10, 200, 40), "Start Game")) {
						networkView.RPC("startGame", RPCMode.All);
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
		GameObject playerObject = (GameObject) Network.Instantiate(playerPrefab, new Vector3(0, 30, 0), Quaternion.identity, 1);
		playerObject.GetComponent<Character>().setClass(thisPlayer.getClassType());
		playerObject.GetComponent<Character> ().setUserId (thisPlayer.getUserId ());
		playerObject.GetComponent<Character> ().setTeamId (thisTeam);
	}
	private void loadMineCartObjects () {
		for (int i = 0; i < 4; i++) {
			minecarts[i] = GameObject.Find("Mine Cart" + i);
		}
	}
	public void sendCartGoldRPC (int teamId, int amount) {
		networkView.RPC("setCartGold", RPCMode.All, teamId, amount);
	}
	public void sendPlayerDeathRPC (int userId) {
		networkView.RPC("addPlayerDeath", RPCMode.All, userId);
	}

	public void sendPlayerKillRPC (int userId) {
		networkView.RPC("addPlayerKill", RPCMode.All, userId);
	}

	public void sendPlayerAssistRPC (int userId) {
		networkView.RPC("addPlayerAssist", RPCMode.All, userId);
	}

	[RPC]
	public void startGame () {
		spawnPlayer();

		GameObject.FindWithTag("MenuCamera").SetActive(false);

		gameStarted = true;
	}
	[RPC]
	public void addToTeam (int userId, int team, string username) {
		if (userId >= MAX_PLAYERS) {
			Debug.Log("Invalid userId on addToTeam");
			return;
		}
		if (team < 0 || team > 3) {
			Debug.Log("Invalid team on addToTeam");
			return;
		}
		Player newPlayer = teams[team].addPlayer(userId, username);
		userTeam[userId] = teams[team];
		if (userId.ToString() == Network.player.ToString()) {
			thisTeam = team;
			thisPlayer = newPlayer;
		}
	}

	// SERVER ONLY
	[RPC]
	public void setupTeam (int userId, string username) {
		if (userId >= MAX_PLAYERS) {
			Debug.Log("Invalid userId on setupTeam");
			return;
		}
		if (Network.isServer) {
			nextTeam = (nextTeam+1)%4;
			networkView.RPC("addToTeam", RPCMode.AllBuffered, userId, nextTeam, username);
		}
	}

	[RPC]
	public void setClassType (int userId, int teamId, int classType) {
		Player player = teams[teamId].findPlayerByUserId (userId);
		if (player == null) { return; }
		player.setClassType (classType);
		if (userId == thisUserId) {
			classTypeSet = true;
		}
	}

	[RPC]

	public void setCartGold (int teamId, int gold) {
		minecarts[teamId].GetComponent<Minecart>().setCartGold(gold);
	}

	[RPC]
	public void addPlayerDeath(int userId) {
		userTeam [userId].addPlayerDeath(userId);
	}

	[RPC]
	public void addPlayerKill(int userId) {
		userTeam [userId].addPlayerKill (userId);
	}

	[RPC]
	public void addPlayerAssist(int userId) {
		userTeam [userId].addPlayerAssist (userId);
	}

	public ArrayList getScores() {
		foreach (Teams t in teams) {
			scores.Add(t.getScoreList());
		}
		return scores;
	}

	public Dictionary<int, Teams> getUserTeamDict() {
		return userTeam;

	}

	/*public void sortTeamScore(List<KeyValuePair<int, Teams.Stats>> lst) {
		int i, j;
		for (i = 1; i < lst.Count - 1; i++) {
			j = i;
			while ((j >= 0) && (lst[j - 1].Value.kills >= lst[i].Value.kills)) {
				if (lst[j - 1].Value.kills == lst[i].Value.kills) {
					if (lst[j - 1].Value.deaths > lst[i].Value.deaths) {
						j--;
						continue;
					}
					else if (lst[j - 1].Value.deaths == lst[i].Value.deaths && lst[j - 1].Value.assists < lst[i].Value.assists) {
						j--;
						continue;
					}
				}
				lst[j] = lst[j-1];
				j--;
			}
			lst[j] = lst[i];		
		}
	}*/
}
