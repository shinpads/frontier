using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour {

	private const int damagePerSecond = 10;
	private const int upfrontDamage = 75;
	int teamId, userId;
	bool canDamage, isSprung;
	Character prey;

	void Start () {
		canDamage = false;
		isSprung = true;
		object[] data = GetComponent<PhotonView>().instantiationData;
		userId = (int)data[0];
		teamId = (int)data[1];
	}
	
	void OnTriggerEnter(Collider col) {
		if (!PhotonNetwork.isMasterClient) { return; }
		if (col.gameObject.tag == "Terrain") {
			isSprung = false;
		}
		if (col.gameObject.tag != "Player" || col.gameObject.GetComponent<Character>().getTeamId() == teamId || isSprung) { return; }
		isSprung = true;
		prey = col.gameObject.GetComponent<Character> ();
		prey.gameObject.GetComponent<PlayerController> ().setAbilityToMove (false);
		prey.gameObject.GetComponent<PhotonView>().RPC ("setHealth", PhotonTargets.All, -upfrontDamage, userId);
		prey.displayMessage ("Press F to release bear trap");
		StartCoroutine (waitOneSecond ());
	}

	void OnTriggerStay(Collider col) {
		if (!PhotonNetwork.isMasterClient || !canDamage || col.gameObject.tag != "Player" || col.gameObject.GetComponent<Character>().getTeamId() == teamId) { return; }
		prey.gameObject.GetComponent<PhotonView>().RPC ("setHealth", PhotonTargets.All, -damagePerSecond, userId);
		StartCoroutine(waitOneSecond());
	}

	void OnTriggerExit(Collider col) {
		if (col.gameObject.Equals (prey) && prey != null) {
			destroy ();
		}
	}

	public void trapOpened() {
		prey.displayMessage ("");
		prey.gameObject.GetComponent<PlayerController> ().setAbilityToMove (true);
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

	public int getTeamId() { return teamId; }

}
