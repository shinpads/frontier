using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinSetup : MonoBehaviour {
	//Array of things we dont want to be enabled for some arbitary player
	[SerializeField] private Behaviour[] thingsToDisable;
	private PhotonView photonView;
	void Start () {
		disableThings();
	}

	void OnPlayerConnect () {
		disableThings();
	}
	private void disableThings() {
		photonView = GetComponent<PhotonView>();
		// Grab
		if (!photonView.isMine) {
			for (int i = 0; i < thingsToDisable.Length; i++) {
				if(thingsToDisable[i] != null)
					thingsToDisable[i].enabled = false;
			}
		}
	}
}
