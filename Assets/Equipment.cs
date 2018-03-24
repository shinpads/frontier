using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour {
	[SerializeField] string projectile;
	[SerializeField] float throwVelocity;
	[SerializeField] string throwAnimation;
	public string getProjectile() { return projectile; }
	public float getThrowVelocity() { return throwVelocity; }
	public string getThrowAnimationName() { return throwAnimation; }
}
