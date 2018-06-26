using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMapControl : MonoBehaviour {
	void Update() {
		if (PhotonNetwork.connected) {
			if (PhotonNetwork.isMasterClient) {
				PhotonNetwork.Instantiate("GameController", new Vector3(0, 0, 0), Quaternion.identity, 0);
			}
			this.enabled = false;
		}
	}

}
