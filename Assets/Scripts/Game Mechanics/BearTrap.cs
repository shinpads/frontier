using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour {

	private const int damagePerSecond = 10;
	private const int upfrontDamage = 50;
	private Animator bearTrapAnimatorControl;
	int teamId, userId;
	bool canDamage, isSprung;
	private PhotonView photonview;
	Character prey;

	void Start () {
		bearTrapAnimatorControl = gameObject.GetComponent<Animator> ();
		photonview = gameObject.GetComponent<PhotonView> ();
		canDamage = false;
		isSprung = true;
		object[] data = GetComponent<PhotonView>().instantiationData;
		userId = (int)data[0];
		teamId = (int)data[1];
	}
	void Update() {
		if (isSprung && prey != null && prey.gameObject.GetComponent<PhotonView>().isMine) {
			if (Input.GetKeyDown(KeyCode.F)) {
				prey.gameObject.GetComponent<PlayerController>().setAbilityToMove(true);
 				photonview.RPC("trapReleased", PhotonTargets.All);
			}
		}
		if (prey != null && prey.getCurrentHealth() <= 0) {
			destroy();
		}
		if (PhotonNetwork.isMasterClient && isSprung && prey!= null && canDamage) {
			prey.gameObject.GetComponent<PhotonView> ().RPC ("setHealth", PhotonTargets.All, -damagePerSecond, userId, Global.SOUND_TYPE.DEFAULT_DAMAGE);
			if(prey.getCurrentHealth() <= 0) {
				destroy();
				return;
			}
			StartCoroutine (waitOneSecond ());
		}
	}
	void OnTriggerEnter(Collider col) {
		if (!PhotonNetwork.isMasterClient) { return; }

		// open it up when it hits the ground
		//photonview.RPC ("trapOpen", PhotonTargets.All);
		//gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
		//isSprung = false;

		if (col.gameObject.tag != "Player" || col.gameObject.GetComponent<Character>().getTeamId() != teamId || isSprung) { return; }
		photonview.RPC ("trapClose", PhotonTargets.All);
		isSprung = true;
		prey = col.gameObject.GetComponent<Character> ();
		prey.addToDestroyOnDead(gameObject);
		photonview.RPC("triggeredByTrap", PhotonTargets.All);
		//prey.gameObject.GetComponent<PlayerController> ().setAbilityToMove (false);
		prey.gameObject.GetComponent<PhotonView>().RPC ("setHealth", PhotonTargets.All, -upfrontDamage, userId, Global.SOUND_TYPE.DEFAULT_DAMAGE);
		if(prey.getCurrentHealth() <= 0) {
			destroy();
			return;
		}
		//prey.displayMessage ("Press F to release bear trap");
		StartCoroutine (waitOneSecond ());
	}
	void OnCollisionEnter(Collision col) {
		if (isSprung) {
			photonview.RPC ("trapOpen", PhotonTargets.All);
			isSprung = false;
			//gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
		}
	}
/*	void OnTriggerStay(Collider col) {
		if (!PhotonNetwork.isMasterClient || !canDamage || col.gameObject.tag != "Player" || col.gameObject.GetComponent<Character>().getTeamId() != teamId) { return; }
		prey.gameObject.GetComponent<PhotonView> ().RPC ("setHealth", PhotonTargets.All, -damagePerSecond, userId, Global.SOUND_TYPE.DEFAULT_DAMAGE);
		if(prey.getCurrentHealth() <= 0) {
			destroy();
			return;
		}
		StartCoroutine (waitOneSecond ());
	} */

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
	[PunRPC]
	public void trapReleased() {
		if (PhotonNetwork.isMasterClient) {
			destroy();
		}
	}
	[PunRPC]
	public void triggeredByTrap() {
		if (prey.gameObject.GetComponent<PhotonView>().isMine) {
				prey.gameObject.GetComponent<PlayerController> ().setAbilityToMove (false);
				prey.displayMessage ("Press F to release bear trap");
		}
	}
	public int getTeamId() { return teamId; }

}
