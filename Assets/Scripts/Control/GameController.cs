using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	const int MAX_PLAYERS = 25;
	[SerializeField] GameObject playerPrefab;
	private NetworkView networkView;
	private ArrayList[] teams = new ArrayList[4] {new ArrayList(), new ArrayList(), new ArrayList(), new ArrayList()};
	private string[] usernames = new string[MAX_PLAYERS];
	private string thisUsername;
	bool usernameSet = false;
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
						// send username to all and all who will join
						networkView.RPC("setUsername", RPCMode.AllBuffered, int.Parse(Network.player.ToString()), thisUsername);
						// if server add to next team (no need to call RPC to itself)
						if (Network.isServer) {
							nextTeam = (nextTeam+1)%4;
							networkView.RPC("addToTeam", RPCMode.AllBuffered, int.Parse(Network.player.ToString()), nextTeam);
						} else {
							// get server to setup team
							networkView.RPC("setupTeam", RPCMode.Server, int.Parse(Network.player.ToString()));
						}
					} else {
						thisUsername = "Too Short";
					}
				}
			}
		}
	}
	private void onConnected() {
		connected = true;
	}
	private void OnGUI() {
		if (connected) {
			if (!gameStarted) {
				if (!usernameSet) {
					thisUsername = GUI.TextField(new Rect(10,10, 200, 30), thisUsername, 15);
					return;
				}
				GUI.Label(new Rect(10, 10, 100, 20), "TEAM 1");
				for (int i = 0; i < teams[0].Count; i++) {
					GUI.Label(new Rect(10, 30 + (10 * i), 100, 20), usernames[int.Parse(teams[0][i].ToString())]);
				}
				GUI.Label(new Rect(120, 10, 100, 20), "TEAM 2");
				for (int i = 0; i < teams[1].Count; i++) {
					GUI.Label(new Rect(120, 30 + (10 * i), 100, 20), usernames[int.Parse(teams[1][i].ToString())]);
				}
				GUI.Label(new Rect(230, 10, 100, 20), "TEAM 3");
				for (int i = 0; i < teams[2].Count; i++) {
					GUI.Label(new Rect(230, 30 + (10 * i), 100, 20), usernames[int.Parse(teams[2][i].ToString())]);
				}
				GUI.Label(new Rect(340, 10, 100, 20), "TEAM 4");
				for (int i = 0; i < teams[3].Count; i++) {
					GUI.Label(new Rect(340, 30 + (10 * i), 100, 20), usernames[int.Parse(teams[3][i].ToString())]);
				}
				if (Network.isServer) {
					if (GUI.Button(new Rect(500, 10, 100, 20), "Start Game")) {
						networkView.RPC("startGame", RPCMode.All);
					}
				}
			}
		}
	}

	public void spawnPlayer() {
		Network.Instantiate(playerPrefab, new Vector3(0, 30, 0), Quaternion.identity, 1);
	}

	[RPC]
	public void startGame () {
		spawnPlayer();

		GameObject.FindWithTag("MenuCamera").SetActive(false);

		gameStarted = true;
	}
	[RPC]
	public void setUsername (int userId, string username) {
		if (userId >= MAX_PLAYERS) {
			Debug.Log("Invalid userId on setUsername");
			return;
		}
		usernames[userId] = username;
	}
	[RPC]
	public void addToTeam (int userId, int team) {
		if (userId >= MAX_PLAYERS) {
			Debug.Log("Invalid userId on addToTeam");
			return;
		}
		if (team < 0 || team > 3) {
			Debug.Log("Invalid team on addToTeam");
			return;
		}
		teams[team].Add(userId);
	}

	// SERVER ONLY
	[RPC]
	public void setupTeam (int userId) {
		if (userId >= MAX_PLAYERS) {
			Debug.Log("Invalid userId on setupTeam");
			return;
		}
		if (Network.isServer) {
			nextTeam = (nextTeam+1)%4;
			networkView.RPC("addToTeam", RPCMode.AllBuffered, userId, nextTeam);
		}
	}
}
