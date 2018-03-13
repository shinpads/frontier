using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
	private const int CIRCLE_DOWNTIME = 10;
	private const int CIRCLE_1 = 5;
	private const int CIRCLE_2 = 10;
	private const int CIRCLE_3 = 15;
	private const int CIRCLE_4 = 20;
	private const int CIRCLE_5 = 25;
	private const int CIRCLE_6 = 50;
	private const int CIRCLE_7 = 100;
	private bool isDown;
	private Animator targetAnimatorControl;
	private AudioSource audioSource;
	private PhotonView photonView;
	[SerializeField] private AudioClip targetPing;

	void Start() {
		photonView = gameObject.GetComponent<PhotonView> ();
		targetAnimatorControl = gameObject.GetComponent<Animator> ();
		audioSource = gameObject.GetComponent<AudioSource> ();
		isDown = false;
	}

	public int hitTarget(GameObject circleHit) {
		if (isDown) {return -1;}
		Debug.Log (circleHit.name);
		photonView.RPC ("playTargetPing", PhotonTargets.All);
		targetAnimatorControl.SetTrigger("targetDown");
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
			score = CIRCLE_7;
			break;
		default:
			break;
		}
		return score;
	}

	private IEnumerator targetDownTime() {
		yield return new WaitForSeconds (CIRCLE_DOWNTIME);
		targetAnimatorControl.SetTrigger ("targetUp");
		isDown = false;
	}

	[PunRPC]
	public void playTargetPing() {
		audioSource.PlayOneShot (targetPing);
	}
}