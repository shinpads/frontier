using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {
  private bool playerSpawned = false;
	private const float TIC_RATE = 64f;
	private const string ip = "130.15.220.142";
	private const int port = 25002;
  [SerializeField] private GameObject playerTank;
  [SerializeField] private GameObject playerAssualt;
  [SerializeField] private GameObject playerScout;
  [SerializeField] private GameObject playerThief;
  [SerializeField] private GameObject playerOther;
	void Start () {
		Application.runInBackground = true;
	}
	private void JoinServer (){
		Network.Connect(ip,port);
	}
	private void CreateServer(int port) {
		Network.InitializeServer (10, port, false);
		Network.sendRate = TIC_RATE;
	}

	private void OnGUI ()
	{
		if (!Network.isClient && !Network.isServer) {
			if (GUI.Button (new Rect (10, 10, 100, 40), "Host")) {
				CreateServer (port);
			}
			if (GUI.Button (new Rect (10, 60, 100, 40), "Join")) {
				JoinServer ();
			}
		}
		if (Network.isClient || Network.isServer && playerSpawned == false) {
      if (GUI.Button(new Rect(10,10,100,40), "Tank")) {
        SpawnPlayer(playerTank);
      }
      else if (GUI.Button(new Rect(10,60,100,40), "Assualt")) {
        SpawnPlayer(playerAssualt);
      }
      else if (GUI.Button(new Rect(10,110,100,40), "Scout")) {
        SpawnPlayer(playerScout);
      }
      else if (GUI.Button(new Rect(10,160,100,40), "Thief")) {
        SpawnPlayer(playerThief);
      }
      else if (GUI.Button(new Rect(10,210,100,40), "Other")) {
        SpawnPlayer(playerOther);
      }
		}
	}

	//SPAWN PLAYER
	private void SpawnPlayer(GameObject playerType){
        if (playerSpawned == false)
        {
            Network.Instantiate(playerType, new Vector3(0, 30, 0), Quaternion.identity, 0);
            playerSpawned = true;
        }
	}
	void OnPlayerConnected(){
		Debug.Log("PLAYER CONNECTED");
	}

	void OnApplicationQuit(){
		Network.Disconnect(200);
	}


}
