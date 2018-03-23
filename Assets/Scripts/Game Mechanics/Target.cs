using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
	private const int TARGET_DOWNTIME = 10;
	private const int CIRCLE_1 = 5;
	private const int CIRCLE_2 = 10;
	private const int CIRCLE_3 = 15;
	private const int CIRCLE_4 = 20;
	private const int CIRCLE_5 = 25;
	private const int CIRCLE_6 = 50;
	private const int CIRCLE_7 = 100;
	private bool isDown;
	private bool timeUp;
	private Animator targetAnimatorControl;
	private AudioSource audioSource;
	private PhotonView photonView;
	[SerializeField] private AudioClip targetPing;
	[SerializeField] private AudioClip targetCentre;

	void Start() {
		photonView = gameObject.GetComponent<PhotonView> ();
		targetAnimatorControl = gameObject.GetComponent<Animator> ();
		audioSource = gameObject.GetComponent<AudioSource> ();
		isDown = false;
		timeUp = false;
	}

	void Update() {
		if(!PhotonNetwork.isMasterClient) { return; }
		if (isDown && timeUp) {
			photonView.RPC ("targetUp", PhotonTargets.All);
			isDown = false;
			timeUp = false;
		}
	}

	public int hitTarget(GameObject circleHit) {
		if (isDown) {return -1;}
		photonView.RPC ("playTargetPing", PhotonTargets.All);
		photonView.RPC ("targetDown", PhotonTargets.All);
		StartCoroutine (targetDownTime());
		isDown = true;
		int score = 0;
		switch(circleHit.name) {
		case("Circle1"):
			score = CIRCLE_1;
			break;
		case("Circle2"):
			score = CIRCLE_2;
			break;
		case("Circle3"):
			score = CIRCLE_3;
			break;
		case("Circle4"):
			score = CIRCLE_4;
			break;
		case("Circle5"):
			score = CIRCLE_5;
			break;
		case("Circle6"):
			score = CIRCLE_6;
			break;
		case("Circle7"):
			photonView.RPC ("playTargetCentre", PhotonTargets.All);
			score = CIRCLE_7;
			break;
		default:
			break;
		}
		return score;
	}

	private IEnumerator targetDownTime() {
		yield return new WaitForSeconds (TARGET_DOWNTIME);
		timeUp = true;
	}

	[PunRPC]
	public void targetDown() {
		audioSource.PlayOneShot (targetPing);
		targetAnimatorControl.SetTrigger ("targetDown");
	}

	[PunRPC]
	public void targetUp() {
		targetAnimatorControl.SetTrigger ("targetUp");
	}

	[PunRPC]
	public void playTargetCentre() {
		audioSource.PlayOneShot (targetCentre);
	}
}