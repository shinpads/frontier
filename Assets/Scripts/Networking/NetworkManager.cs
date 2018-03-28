using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {
	private bool playerSpawned = false;
	private const float TIC_RATE = 64f;
	private const string ip = "127.0.0.1";
	private const int port = 25002;
	private string username;
	private bool usernameSet = false;
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject gameControl;
	private bool gameControlSpawned = false;
	void Start () {
		PhotonNetwork.sendRate = 64;
		PhotonNetwork.sendRateOnSerialize = 50;
		Application.runInBackground = true;
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = -1;
		username = "";
	}
  void Update () {
    if (!PhotonNetwork.connected) {
			if (Input.GetKeyDown(KeyCode.H)) {
				// CreateServer (port);
			}
			if (Input.GetKeyDown(KeyCode.J)) {
				if (username.Length > 0) {
					usernameSet = true;
					Global.username = username;
					JoinServer();
				}
			}
		}
  }
	void OnJoinedRoom () {
		Debug.Log("Joined Room");
		if (!gameControlSpawned && PhotonNetwork.isMasterClient) {
			PhotonNetwork.Instantiate("GameController", new Vector3(0, 0, 0), Quaternion.identity, 0);
			gameControlSpawned = true;
		}
	}
	private void JoinServer () {
		PhotonNetwork.AuthValues = new AuthenticationValues(Global.username);
		PhotonNetwork.ConnectUsingSettings ("v1.0.0");
		// Network.Connect(ip,port);
	}
	private void CreateServer(int port) {
		Network.InitializeServer (10, port, false);
		Network.sendRate = TIC_RATE;
	}
	void OnGUI () {
		if (!PhotonNetwork.connected) {
			if (!usernameSet) {
				username = GUI.TextField(new Rect(10,10, 200, 30), username, 15);
			}
			GUI.Label(new Rect(10, 40, 100, 20), "[J] to Join");
		}
		// GUI.Label(new Rect(10, 70, 150, 20), PhotonNetwork.connectionStateDetailed.ToString());
	}

	void OnJoinedLobby() {
		Debug.Log("Joined Lobby");
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.isVisible = true;
		roomOptions.isOpen = true;
		roomOptions.maxPlayers = 21;
		roomOptions.PublishUserId = true;
		TypedLobby typedLobby = new TypedLobby("lobby", LobbyType.SqlLobby);
		PhotonNetwork.JoinOrCreateRoom ("CoolRoom123", roomOptions, typedLobby);
	}

	void OnApplicationQuit(){
		Network.Disconnect(200);
	}
}
