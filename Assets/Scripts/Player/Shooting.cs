using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CinematicEffects;

public class Shooting : MonoBehaviour {
	public Camera playerCamera;
	public GameObject gunCamera;
	private RaycastHit hit;
	private Ray ray;
	private Vector3 endpoint;
	private float distance;
  	private bool canShoot = true;
	private Vector3 ads, hip;
	private PlayerGUI gui;
	private int currentGunIndex;
	private int currentEquipmentIndex;
	private GameObject armPivot;
	private GameObject gunContainer;
	private GameObject[] gunObjects;
	private GameObject[] equipmentObjects;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private GameObject muzzleFlash;
	[SerializeField] private GameObject bulletObject;
	private Animator armPivotAnimator;
	private Gun currentGun;
	private Equipment currentEquipment;
	private PhotonView photonView;
	private bool isReloading = false;
	private bool isAds = false;
	private bool stillScoped = false;
	private bool outAndHeld = false;
	private float coneLength;
	private float coneRadius;
	private bool loaded = false;
	private bool freeze = false;
	LayerMask ignoreRayCastLayer;
	Character player;
	PlayerController playerController;
	void Start () {
		coneLength = 15f;
		coneRadius = 0.75f;
		currentGunIndex = 0;
		currentEquipmentIndex = -1;
		hip = new Vector3(0, 0, 0);
		ads = new Vector3(-0.24f, 0.09f, -0.18f);
		playerCamera = gameObject.GetComponent<PlayerController>().playerCamera;
		endpoint = new Vector3(0,0,0);
		distance = 0;
		photonView = gameObject.GetComponent<PhotonView>();
		player = gameObject.GetComponent<Character> ();
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		// all layers except 2nd which is Ignore Raycast
		ignoreRayCastLayer = ~(1 << 2);
		playerController = gameObject.GetComponent<PlayerController>();
	}

	void Update () {
		photonView = gameObject.GetComponent<PhotonView>();
		if (!photonView.isMine || !loaded) { return; }

		if (Input.GetKeyDown (KeyCode.R) && currentGun.getAmmo() != currentGun.getMagCapacity() && !isReloading) {
		     StartCoroutine(reloadwait());
		}

		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			if (currentGunIndex < gunObjects.Length - 1 && currentGunIndex >= 0) {
				currentGunIndex++;
				photonView.RPC ("sendSwapGuns", PhotonTargets.All,currentGunIndex);
			} else if (currentGunIndex == gunObjects.Length - 1 && currentGunIndex != 0) {
				photonView.RPC ("sendSwapEquipment", PhotonTargets.All, 0);
				currentEquipmentIndex = 0;
			} else if (currentGunIndex == -1 && currentEquipmentIndex < equipmentObjects.Length - 1 && currentEquipmentIndex >= 0) {
				currentEquipmentIndex++;
				photonView.RPC ("sendSwapEquipment", PhotonTargets.All, currentGunIndex);
			}
		}

		else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			if (currentEquipmentIndex > 0) {
				currentEquipmentIndex--;
				photonView.RPC ("sendSwapEquipment", PhotonTargets.All, currentEquipmentIndex);
			} else if (currentEquipmentIndex == 0) {
				currentGunIndex = gunObjects.Length - 1;
				photonView.RPC ("sendSwapGuns", PhotonTargets.All, currentGunIndex);
			} else if (currentEquipmentIndex == -1 && currentGunIndex > 0) {
				currentGunIndex--;
				photonView.RPC ("sendSwapGuns", PhotonTargets.All, currentGunIndex);
			}
		}

		if (Input.GetKeyDown (KeyCode.Alpha1) && gunObjects.Length > 0 && currentGunIndex != 0 ) {
			photonView.RPC ("sendSwapGuns", PhotonTargets.All, 0);
			currentGunIndex = 0;
		}

		else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			if  (equipmentObjects.Length > 1 && currentEquipmentIndex != 0) {
				photonView.RPC ("sendSwapEquipment", PhotonTargets.All, 0);
				currentEquipmentIndex = 0;
			} else if (gunObjects.Length > 1 && gunObjects.Length > 0 && currentGunIndex != 1) {
				photonView.RPC ("sendSwapGuns", PhotonTargets.All, 1);
				currentGunIndex = 1;
			}
		}

		else if (Input.GetKeyDown (KeyCode.Alpha3) && equipmentObjects.Length > 0 && currentEquipmentIndex != equipmentObjects.Length) {
			photonView.RPC ("sendSwapEquipment", PhotonTargets.All, equipmentObjects.Length - 1);
			currentEquipmentIndex = equipmentObjects.Length - 1;
		}
		if (!freeze) {
			if (currentGunIndex != -1 && canShoot && !isReloading && !stillScoped) {
				if (currentGun.getAmmo () == 0) {
					if (Input.GetButtonDown ("Fire1")) {
						photonView.RPC ("sendDryFireSound", PhotonTargets.All);
					} else if (currentGun.getIsAutomatic ()) {
						if (Input.GetButton ("Fire1") && !outAndHeld) {
							photonView.RPC ("sendDryFireSound", PhotonTargets.All);
							outAndHeld = true;
						}
					}
				} else if ((currentGun.getIsAutomatic () && Input.GetButton ("Fire1")) || (!currentGun.getIsAutomatic () && Input.GetButtonDown ("Fire1"))) {
					shootBullet ();
				}
			} else if (currentEquipmentIndex != -1 && Input.GetButtonDown("Fire1")) {
				StartCoroutine(throwEquipment());
			}
		}

		if (Input.GetButtonDown("Fire2") && currentGunIndex != -1) {
			isAds = true;
			hipToAds();
		} else if (Input.GetButtonUp("Fire2") && currentGunIndex != -1) {
			isAds = false;
			adsToHip(false);
			if (currentGun.getIsScoped ()) {
				stillScoped = false;
			}
		}
	}

	public void setContainer () {
		gunContainer = gameObject.GetComponent<MeshController>().getContainer();
		photonView = gameObject.GetComponent<PhotonView>();
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		ItemContainer itemContainer = gunContainer.GetComponent<ItemContainer>();
		armPivot = itemContainer.armPivot;
		gunObjects = itemContainer.guns;
		equipmentObjects = itemContainer.equipment;
		currentGun = gunObjects[0].GetComponent<Gun>();
		gui.setAmmoCounter (currentGun.getMagCapacity(), currentGun.getMagCapacity());
		armPivotAnimator = armPivot.GetComponent<Animator>();
		// currentEquipment = equipmentObjects[0].GetComponent<Equipment>();
		if (photonView.isMine) {
			setGunLayers();
			photonView.RPC ("sendSwapGuns", PhotonTargets.All, (currentGunIndex));
		}
		loaded = true;
	}

	private void setGunLayers () {
		// ONLY FOR USERS OWN PLAYER
		for (int i = 0; i < gunObjects.Length; i++) {
			gunObjects[i].layer = 12;
			foreach (Transform child in gunObjects[i].transform) {
				child.gameObject.layer = 12;
				foreach (Transform child2 in child.transform) {
					child2.gameObject.layer = 12;
				}
			}
		}
		for (int i = 0; i < equipmentObjects.Length; i++) {
			equipmentObjects[i].layer = 12;
			foreach (Transform child in equipmentObjects[i].transform) {
				child.gameObject.layer = 12;
			}
		}
		armPivot.layer = 12;
	}

	[PunRPC]
	private void shoot(Vector3 start, Vector3 end, int userId, bool ads) {
		PhotonView photonView = gameObject.GetComponent<PhotonView>();
		audioSource.PlayOneShot (currentGun.getGunShotSound());
		// create muzzle effect
		Instantiate (muzzleFlash, currentGun.getJustTheTip().transform.position, Quaternion.LookRotation(end - start), playerCamera.transform);
		if(!photonView.isMine) { return; }
		if (currentGun.getIsShotgun()) {
			float rangeFactor = 20f;
			if (ads) {
				rangeFactor = 35f;
			}
			float range = Mathf.PI/rangeFactor;
			float theta = 0f;
			for (int i = 0; i < 5; i++) {
				Vector3 direction = Vector3.Normalize(end-start);
				theta = Random.Range(-range/2f, range/2f);
				direction.x = (direction.x * Mathf.Cos(theta)) - (direction.y * Mathf.Sin(theta));
				direction.y = (direction.x * Mathf.Sin(theta)) + (direction.y * Mathf.Cos(theta));
				theta = Random.Range(-range/2f, range/2f);
				direction.x = (direction.x * Mathf.Cos(theta)) + (direction.z * Mathf.Sin(theta));
				direction.z = -(direction.x * Mathf.Sin(theta)) + (direction.z * Mathf.Cos(theta));
				theta = Random.Range(-range/2f, range/2f);
				direction.y = (direction.y * Mathf.Cos(theta)) - (direction.z * Mathf.Sin(theta));
				direction.z = (direction.y * Mathf.Sin(theta)) + (direction.z * Mathf.Cos(theta));
				photonView.RPC("createBullet", PhotonTargets.All, start ,Quaternion.LookRotation(direction), userId, direction*currentGun.getBulletSpeed(), photonView.viewID, currentGun.getBulletDamage(), currentGun.getDropOff(), currentGun.getDropOffStop(), currentGun.getBulletLife());
				theta += range/5f;
			}
		} else {
			Vector3 direction = Vector3.Normalize(end-start);
			photonView.RPC("createBullet", PhotonTargets.All, start, Quaternion.LookRotation(direction), userId, direction*currentGun.getBulletSpeed(), photonView.viewID, currentGun.getBulletDamage(), currentGun.getDropOff(), currentGun.getDropOffStop(), currentGun.getBulletLife());
		}

	}
	[PunRPC]
	private void createBullet (Vector3 position, Quaternion direction, int userId, Vector3 velocity, int photonViewId, int damage, float dropOff, float dropOffEnd, float life) {
		GameObject bulletInstance = (GameObject) GameObject.Instantiate(bulletObject, position, direction);
		//if (!PhotonNetwork.isMasterClient) { return; }
		//GameObject bulletInstance = (GameObject) PhotonNetwork.Instantiate("Bullet", position, direction, 0);
		Bullet bullet = bulletInstance.GetComponent<Bullet>();
		bullet.setUserId (userId);
		bullet.velocity = velocity;
		bullet.userPlayer = PhotonView.Find (photonViewId);
		bullet.maxDamage = damage;
		bullet.dropOff = dropOff;
		bullet.dropOffStop = dropOffEnd;
		bullet.lifeTime = life;
		bullet.damage = bullet.maxDamage;
	}
	[PunRPC]
	private void throwEquipmentRPC(Vector3 position, int userId, Vector3 forwards, int teamId) {
		GameObject equipment = (GameObject)PhotonNetwork.Instantiate(currentEquipment.getProjectile(), position, Quaternion.Euler(0, 0, 0), 0, new object[] {userId, teamId});
		equipment.GetComponent<Rigidbody>().AddForce(forwards * currentEquipment.getThrowVelocity());
	}

	private IEnumerator reloadwait()  {
		isReloading = true;
		yield return new WaitForSeconds(currentGun.getReloadTime());
		currentGun.reload ();
		gui.setAmmoCounter (currentGun.getAmmo(), currentGun.getMagCapacity());
		isReloading = false;
		canShoot = true;
		if (currentGun.getIsAutomatic() && outAndHeld) {
			outAndHeld = false;
		}
	}

    private IEnumerator delayedShooting(){
    	canShoot = false;
		yield return new WaitForSeconds(currentGun.getShotDelay());
    	canShoot = true;
    }

	[PunRPC]
	private void sendSwapGuns (int newGunIndex) {
		swapGuns(gunObjects[newGunIndex]);
	}

	[PunRPC]
	private void sendSwapEquipment (int newEquipmentIndex) {
		swapEquipment(equipmentObjects[newEquipmentIndex]);
	}
	private void shootBullet() {
		StartCoroutine(delayedShooting());
		armPivotAnimator.Play(currentGun.getShootingAnimationName());
		currentGun.playShootingAnimation();
		if (isAds || currentGun.getIsShotgun()) {
			ray = new Ray (playerCamera.transform.position, playerCamera.transform.forward * 100);
		} else {
			float randomRadius = Random.Range (0f, currentGun.getBulletSpray());
			float randomAngle = Random.Range (0f, 2 * Mathf.PI);
			Vector3 randomDirection = new Vector3 (randomRadius * Mathf.Cos (randomAngle), randomRadius * Mathf.Sin (randomAngle), coneLength);
			ray = new Ray (playerCamera.transform.position, playerCamera.transform.TransformDirection(randomDirection.normalized));
		}

		if (Physics.Raycast (ray, out hit, Mathf.Infinity, ignoreRayCastLayer)) {
			endpoint = ray.GetPoint (hit.distance);
		} else {
			endpoint = ray.GetPoint (1000);
		}
		gameObject.GetComponent<PhotonView>().RPC("shoot", PhotonTargets.All, currentGun.getJustTheTip().transform.position,endpoint, player.getUserId(), isAds);
		currentGun.ammoShot ();
		gui.setAmmoCounter (currentGun.getAmmo(), currentGun.getMagCapacity());
		if (currentGun.getIsScoped() && isAds) {
			stillScoped = true;
		}
		gameObject.GetComponent<PlayerController>().setRecoil(currentGun.getRecoilPerShot());
	}

	private IEnumerator throwEquipment() {
		armPivotAnimator.Play(currentEquipment.getThrowAnimationName());
		yield return new WaitForSeconds(0.5f);
		photonView.RPC("throwEquipmentRPC", PhotonTargets.MasterClient, playerCamera.transform.position + playerCamera.transform.forward, player.getUserId(), playerCamera.transform.forward, player.getTeamId());
		currentGunIndex = 0;
		photonView.RPC ("sendSwapGuns", PhotonTargets.All, 0);
	}

	private void swapGuns(GameObject newGun) {
		currentEquipmentIndex = -1;
		stillScoped = false;
		if (currentGun != null) { currentGun.gameObject.SetActive (false); }
		if (currentEquipment != null) { currentEquipment.gameObject.SetActive(false); }
		newGun.SetActive (true);
		currentGun = newGun.GetComponent<Gun>();
		gui.setAmmoCounter (currentGun.getAmmo (), currentGun.getMagCapacity ());
		adsToHip (true);
	}

	private void swapEquipment(GameObject newEquipment) {
		currentGunIndex = -1;
		stillScoped = false;
		if (currentEquipment != null) { currentEquipment.gameObject.SetActive(false); }
		if (currentGun != null) { currentGun.gameObject.SetActive (false); }
		newEquipment.SetActive (true);
		currentEquipment = newEquipment.GetComponent<Equipment>();
		gui.setAmmoCounter (0, 0);
		adsToHip (true);
	}

	private void adsToHip(bool direct) {
		if (!direct) {
			StartCoroutine (lerpGunPosition (currentGun.ads, currentGun.hip, 0.07f));
		} else {
			StartCoroutine (lerpGunPosition (currentGun.ads, currentGun.hip, 0f));
		}
		playerController.changeAdsState (false);
		if (currentGun.getIsScoped()) {
			gui.setCrosshairEnabled(false);
		} else {
			gui.setCrosshairEnabled(true);
		}
		gui.setScopeEnabled(false);
		gunCamera.SetActive(true);
		playerController.setSensitivity(0);
		if (currentGun.getIsScoped()) {
			playerCamera.fieldOfView = 60;
		} else {
			StartCoroutine(lerpGunZoom(playerCamera.fieldOfView, 60, 0.1f));
		}
	}

	private void hipToAds() {
    StartCoroutine(lerpGunPosition(currentGun.hip, currentGun.ads, 0.07f));
		playerController.changeAdsState (true);
		gui.setCrosshairEnabled(false);
		if (currentGun.getIsScoped()) {
			gui.setScopeEnabled(true);
			gunCamera.SetActive(false);
			playerController.setSensitivity(2);
			playerCamera.fieldOfView = currentGun.adsFov;
		} else {
			playerController.setSensitivity(1);
			StartCoroutine(lerpGunZoom(playerCamera.fieldOfView, currentGun.adsFov, 0.1f));
		}
	}

	[PunRPC]
	private void sendDryFireSound() {
		audioSource.PlayOneShot (currentGun.getDryFireSound());
	}

	public void setFreeze(bool isFreeze) {
		freeze = isFreeze;
	}

	private IEnumerator lerpGunPosition (Vector3 startPosition, Vector3 endPosition, float time) {
	    float startTime = Time.time;
	    while (Time.time < startTime + time) {
			gunContainer.transform.localPosition = Vector3.Lerp(startPosition, endPosition, (Time.time - startTime) / time);
			yield return new WaitForEndOfFrame();
	    }
	    gunContainer.transform.localPosition = endPosition;
	}

	private IEnumerator lerpGunZoom (float startValue, float endValue, float time) {
		float startTime = Time.time;
	    while (Time.time < startTime + time) {
			playerCamera.fieldOfView = Mathf.Lerp(startValue, endValue, (Time.time - startTime) / time);
			yield return new WaitForEndOfFrame();
	    	}
	    playerCamera.fieldOfView = endValue;
	}

}
