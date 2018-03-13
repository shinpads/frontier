using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
	[SerializeField] private AudioClip gunShotSound;
	[SerializeField] private float bulletSpeed;
	[SerializeField] private float rateOfFire;
	[SerializeField] private bool isAutomatic;
	[SerializeField] private int bulletDamage;
	[SerializeField] private int magCapacity;
	[SerializeField] private GameObject tipOfGun;
	private Vector3 hipFire, ads;
	private int ammo;
	//Recoil?

	void Awake () {
		ammo = magCapacity;
	}

	// Lazy ass getters
	public AudioClip getGunShotSound() { return gunShotSound; }
	public float getBulletSpeed() { return bulletSpeed; }
	public float getShotDelay() { return (float)1/rateOfFire; }
	public bool getIsAutomatic() { return isAutomatic; }
	public int getBulletDamage() { return bulletDamage; }
	public int getMagCapacity() { return magCapacity; }
	public GameObject getJustTheTip() { return tipOfGun; }
	public int getAmmo() { return ammo; }

	public void ammoShot() {
		if (ammo > 0) {
			ammo--;
		}
	}

	public void reload() {
		ammo = magCapacity;
	}
}
