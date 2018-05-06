using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour {

	private const int damagePerSecond = 10;
	private const int upfrontDamage = 75;
	int teamId, userId;
	int speedOfPrey;
	bool canDamage, isSprung;
	Character prey;

	void Start () {
		canDamage = isSprung = false;
		object[] data = GetComponent<PhotonView>().instantiationData;
		userId = (int)data[0];
		teamId = (int)data[1];
	}
	
	void OnTriggerEnter(Collider col) {
		if (!PhotonNetwork.isMasterClient || col.gameObject.tag != "Player" || col.gameObject.GetComponent<Character>().getTeamId() == teamId || isSprung) { return; }
		isSprung = true;
		prey = col.gameObject.GetComponent<Character> ();
		speedOfPrey = prey.getSpeed();
		prey.setSpeed (0);
		prey.gameObject.GetComponent<PhotonView>().RPC ("setHealth", PhotonTargets.All, -upfrontDamage, userId);
		prey.displayMessage ("Press F to release bear trap");
		StartCoroutine (waitOneSecond ());
	}

	void OnTriggerStay(Collider col) {
		if (!PhotonNetwork.isMasterClient || !canDamage || col.gameObject.tag != "Player" || col.gameObject.GetComponent<Character>().getTeamId() == teamId) { return; }
		prey.gameObject.GetComponent<PhotonView>().RPC ("setHealth", PhotonTargets.All, -damagePerSecond, userId);
		StartCoroutine(waitOneSecond());
	}

	public void trapOpened() {
		prey.displayMessage ("");
		prey.setSpeed (speedOfPrey);
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
}
