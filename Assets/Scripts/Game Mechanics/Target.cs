using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
	private const int circle1 = 5;
	private const int circle2 = 10;
	private const int circle3 = 15;
	private const int circle4 = 20;
	private const int circle5 = 25;
	private const int circle6 = 50;
	private const int circle7 = 100;
	private bool isDown;
	private Animator targetAnimatorControl;

	void Start() {
		targetAnimatorControl = gameObject.GetComponent<Animator> ();
		targetUp ();
		isDown = false;
	}

	public int hitTarget(GameObject circleHit) {
		if (isDown) {return -1;}
		int score = 0;
		switch(circleHit.name) {
		case("Circle1"):
			score = circle1;
			break;
		case("Circle2"):
			score = circle1;
			break;
		case("Circle3"):
			score = circle1;
			break;
		case("Circle4"):
			score = circle1;
			break;
		case("Circle5"):
			score = circle1;
			break;
		case("Circle6"):
			score = circle1;
			break;
		case("Circle7"):
			score = circle1;
			break;
		default:
			break;
		}
		return score;
	}

	private void targetDown() {
		isDown = true;
		targetAnimatorControl.SetTrigger ("targetDown");
	}

	private void targetUp() {
		targetAnimatorControl.SetTrigger ("targetUp");
		isDown = false;
	}
}