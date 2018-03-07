using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {
	private bool playerSpawned = false;
	private const float TIC_RATE = 64f;
	private const string ip = "127.0.0.1";
	private const int port = 25002;
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject gameControl;
	private bool gameControlSpawned = false;
	void Start () {
		Application.runInBackground = true;
	}
  void Update () {
    if (!Network.isClient && !Network.isServer) {
			if (Input.GetKeyDown(KeyCode.H)) {
				CreateServer (port);
			}
			if (Input.GetKeyDown(KeyCode.J)) {
				JoinServer ();
			}
		} else if (Network.isServer && !gameControlSpawned) {
			Network.Instantiate(gameControl, new Vector3(0, 0, 0), Quaternion.identity, 0);
			gameControlSpawned = true;
		}
  }
	private void JoinServer (){
		Network.Connect(ip,port);
	}
	private void CreateServer(int port) {
		Network.InitializeServer (10, port, false);
		Network.sendRate = TIC_RATE;
	}
	void OnGUI () {
		if (!Network.isServer && !Network.isClient) {
			GUI.Label(new Rect(10, 10, 100, 20), "[H] to Host");
			GUI.Label(new Rect(10, 40, 100, 20), "[J] to Join");
		}
	}
	void OnPlayerConnected(){
		Debug.Log("PLAYER CONNECTED");
	}

	void OnApplicationQuit(){
		Network.Disconnect(200);
	}


}
