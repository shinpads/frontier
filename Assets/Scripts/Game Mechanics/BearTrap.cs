using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour {

	private const int damagePerSecond = 10;
	private const int upfrontDamage = 75;
	private Animator bearTrapAnimatorControl;
	int teamId, userId;
	bool canDamage, isSprung;
	private PhotonView photonview;
	Character prey;

	void Start () {
		bearTrapAnimatorControl = gameObject.GetComponentInChildren<Animator> ();
		photonview = gameObject.GetComponent<PhotonView> ();
		canDamage = false;
		isSprung = true;
		object[] data = GetComponent<PhotonView>().instantiationData;
		userId = (int)data[0];
		teamId = (int)data[1];
	}
	
	void OnTriggerEnter(Collider col) {
		if (!PhotonNetwork.isMasterClient) { return; }
		if (col.gameObject.tag == "Terrain") {
			photonview.RPC ("trapOpen", PhotonTargets.All);
			isSprung = false;
		}
		if (col.gameObject.tag != "Player" || col.gameObject.GetComponent<Character>().getTeamId() == teamId || isSprung) { return; }
		photonview.RPC ("trapClose", PhotonTargets.All);
		isSprung = true;
		prey = col.gameObject.GetComponent<Character> ();
		prey.gameObject.GetComponent<PlayerController> ().setAbilityToMove (false);
		prey.gameObject.GetComponent<PhotonView>().RPC ("setHealth", PhotonTargets.All, -upfrontDamage, userId);
		prey.displayMessage ("Press F to release bear trap");
		StartCoroutine (waitOneSecond ());
	}

	void OnTriggerStay(Collider col) {
		if (!PhotonNetwork.isMasterClient || !canDamage || col.gameObject.tag != "Player" || col.gameObject.GetComponent<Character>().getTeamId() == teamId) { return; }
		if (prey.getCurrentHealth () < damagePerSecond) {
			prey.gameObject.GetComponent<PhotonView> ().RPC ("setHealth", PhotonTargets.All, -damagePerSecond, userId);
			destroy ();
		} else {
			prey.gameObject.GetComponent<PhotonView> ().RPC ("setHealth", PhotonTargets.All, -damagePerSecond, userId);
			StartCoroutine (waitOneSecond ());
		}
	}

	public void trapOpened() {
		prey.displayMessage ("");
		prey.gameObject.GetComponent<PlayerController> ().setAbilityToMove (true);
		photonview.RPC ("trapOpen", PhotonTargets.All);
		PhotonNetwork.Destroy (gameObject);
	}

	public void destroy() {
		PhotonNetwork.Destroy (gameObject);
	}

	private IEnumerator waitOneSecond() {
		canDamage = false;
		yield return new WaitForSeconds(1);
		canDamage = true;
	}

	[PunRPC]
	public void trapOpen() {
		bearTrapAnimatorControl.SetTrigger ("Open");
	}

	[PunRPC]
	public void trapClose() {
		bearTrapAnimatorControl.SetTrigger ("Close");
	}

	public int getTeamId() { return teamId; }

}
