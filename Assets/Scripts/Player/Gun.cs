using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
	[SerializeField] private AudioClip gunShotSound;
	[SerializeField] private float bulletSpeed;
	[SerializeField] private float bulletSpread;
	[SerializeField] private float rateOfFire;
	[SerializeField] private bool isAutomatic;
	[SerializeField] private bool isScoped;
	[SerializeField] private bool isShotgun;
	[SerializeField] private int bulletDamage;
	[SerializeField] private int magCapacity;
	[SerializeField] private GameObject tipOfGun;
	[SerializeField] private float reloadWait;
	[SerializeField] private float dropOff;
	[SerializeField] private float dropOffStop;
	[SerializeField] private string shootingAnimationName = "revolver";
	[SerializeField] private AudioClip dryFire;
	[SerializeField] private float bulletLife;
	[SerializeField] private float recoilPerShot;
	[SerializeField] private bool hasShootingAnimation;
	public Vector3 hip, ads;
	public float adsFov;
	private int ammo;
	//Recoil?

	void Awake () {
		ammo = magCapacity;
	}

	// Lazy ass getters
	public AudioClip getGunShotSound() { return gunShotSound; }
	public AudioClip getDryFireSound() { return dryFire; }
	public float getBulletSpeed() { return bulletSpeed; }
	public float getBulletSpray() { return bulletSpread; }
	public float getShotDelay() { return (float)1/rateOfFire; }
	public bool getIsAutomatic() { return isAutomatic; }
	public int getBulletDamage() { return bulletDamage; }
	public int getMagCapacity() { return magCapacity; }
	public GameObject getJustTheTip() { return tipOfGun; }
	public int getAmmo() { return ammo; }
	public bool getIsScoped() { return isScoped; }
	public float getReloadTime() { return reloadWait; }
	public string getShootingAnimationName() { return shootingAnimationName; }
	public float getDropOff() { return dropOff; }
	public float getDropOffStop() { return dropOffStop; }
	public bool getIsShotgun() { return isShotgun; }
	public float getBulletLife() { return bulletLife; }
	public float getRecoilPerShot() { return recoilPerShot; }
	public void ammoShot() {
		if (ammo > 0) {
			ammo--;
		}
	}

	public void reload() {
		ammo = magCapacity;
	}
	public void playShootingAnimation() {
		if (hasShootingAnimation) {
			gameObject.GetComponent<Animator>().SetTrigger("shoot");
		}
	}
}
