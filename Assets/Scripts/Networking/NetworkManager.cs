using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {
    bool Player_Spawned = false;
	const float TIC_RATE = 64f;
	const string ip = "127.0.0.1";
	const int port = 25002;
	void Start () {
		Application.runInBackground = true;
	}	
	private void JoinServer (){
		Network.Connect(ip,port);
	}
	private void CreateServer(int port) {
		Network.InitializeServer (10, port, false);
		Network.sendRate = TIC_RATE;
		SpawnPlayer();	
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
		if (Network.isClient && Player_Spawned == false) {
			if (GUI.Button(new Rect(10,10,100,40), "Spawn")) {
				SpawnPlayer();
			}
		}
	}

	//SPAWN PLAYER
	private void SpawnPlayer(){
        if (Player_Spawned == false)
        {
            Network.Instantiate(Resources.Load("Prefabs/Player"), new Vector3(0, 30, 0), Quaternion.identity, 0);
            Player_Spawned = true;
        }
	}
	void OnPlayerConnected(){
		Debug.Log("PLAYER CONNECTED");
	}

	void OnApplicationQuit(){
		Network.Disconnect(200);
	}


}
