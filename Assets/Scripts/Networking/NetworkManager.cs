using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour {
	private string username;
	private bool usernameSet = false;
	private bool gameControlSpawned = false;
	void Start () {
		PhotonNetwork.sendRate = 64;
		PhotonNetwork.sendRateOnSerialize = 64;
		PhotonNetwork.automaticallySyncScene = true;
		Application.runInBackground = true;
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = -1;
		username = "";
	}
	/*
  void Update () {
    if (!PhotonNetwork.connected) {
			if (Input.GetKeyDown(KeyCode.J)) {
				if (username.Length > 0) {
					usernameSet = true;
					Global.username = username;
					JoinServer();
				}
			}
		}
  }*/
	void OnJoinedRoom () {
		Debug.Log("Joined Room");
		//Application.LoadLevel("Map1");
		//StartCoroutine(loadLevelAsync());
		PhotonNetwork.LoadLevel("map1");
	}
	public void JoinServer () {
		PhotonNetwork.AuthValues = new AuthenticationValues(Global.username);
		PhotonNetwork.ConnectUsingSettings ("v1.0.0");
	}
	/*
	void OnGUI () {
		if (!PhotonNetwork.connected) {
			if (!usernameSet) {
				username = GUI.TextField(new Rect(10,10, 200, 30), username, 15);
			}
			GUI.Label(new Rect(10, 40, 100, 20), "[J] to Join");
		}
		// GUI.Label(new Rect(10, 70, 150, 20), PhotonNetwork.connectionStateDetailed.ToString());
	}
	*/

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

	private IEnumerator loadLevelAsync() {
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("map1");
		while (!asyncLoad.isDone) {
			yield return null;
		}
	}
}
