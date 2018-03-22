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
	[SerializeField] private GameObject armPivot;
	[SerializeField] private GameObject gunContainer;
	[SerializeField] private GameObject[] gunObjects;
	[SerializeField] private AudioSource audioSource;
	private Animator armPivotAnimator;
	private Gun currentGun;
	private PhotonView photonView;
	private bool isReloading = false;
	private bool isAds = false;
	private bool stillScoped = false;
	private float coneLength;
	private float coneRadius;
	LayerMask ignoreRayCastLayer;
	Character player;
	PlayerController playerController;
	void Start () {
		coneLength = 15f;
		coneRadius = 0.75f;
		currentGun = gunObjects[0].GetComponent<Gun>();
		currentGunIndex = 0;
		hip = new Vector3(0, 0, 0);
		ads = new Vector3(-0.24f, 0.09f, -0.18f);
		playerCamera = gameObject.GetComponent<PlayerController>().playerCamera;
		endpoint = new Vector3(0,0,0);
		distance = 0;
		photonView = gameObject.GetComponent<PhotonView>();
		armPivotAnimator = armPivot.GetComponent<Animator>();
		player = gameObject.GetComponent<Character> ();
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		// all layers except 2nd which is Ignore Raycast
		ignoreRayCastLayer = ~(1 << 2);
		gui.setAmmoCounter (currentGun.getMagCapacity(), currentGun.getMagCapacity());
		playerController = gameObject.GetComponent<PlayerController>();
		if (photonView.isMine) {
			setGunLayers();
		}
	}

	void Update () {
		if (!photonView.isMine) { return; }

		if (Input.GetKeyDown (KeyCode.R) && currentGun.getAmmo() != currentGun.getMagCapacity() && !isReloading) {
		     StartCoroutine(reloadwait());
		}

		if (Input.GetAxis("Mouse ScrollWheel") < 0 && !currentGun.gameObject.Equals(gunObjects[gunObjects.Length-1])) {
			photonView.RPC ("sendSwapGuns", PhotonTargets.All, (currentGunIndex + 1));
			currentGunIndex++;
		}

		else if (Input.GetAxis("Mouse ScrollWheel") > 0 && !currentGun.gameObject.Equals(gunObjects[0])) {
			photonView.RPC ("sendSwapGuns", PhotonTargets.All, (currentGunIndex - 1));
			currentGunIndex--;
		}

		if (Input.GetKeyDown (KeyCode.Alpha1) && !currentGun.gameObject.Equals(gunObjects[0])) {
			photonView.RPC ("sendSwapGuns", PhotonTargets.All, 0);
			Debug.Log ("bitch");
			currentGunIndex = 0;
		}

		else if (Input.GetKeyDown (KeyCode.Alpha2) && !currentGun.gameObject.Equals(gunObjects[1])) {
			photonView.RPC ("sendSwapGuns", PhotonTargets.All, 1);
			currentGunIndex = 1;
		}

		else if (Input.GetKeyDown (KeyCode.Alpha3) && !currentGun.gameObject.Equals(gunObjects[2])) {
			photonView.RPC ("sendSwapGuns", PhotonTargets.All, 2);
			currentGunIndex = 2;
		}

		else if (Input.GetKeyDown (KeyCode.Alpha4) && !currentGun.gameObject.Equals(gunObjects[3])) {
			photonView.RPC ("sendSwapGuns", PhotonTargets.All, 3);
			currentGunIndex = 3;
		}

		if (canShoot && !isReloading && currentGun.getAmmo () != 0 && !stillScoped) {
			if (currentGun.getIsAutomatic () && Input.GetButton ("Fire1")) {
				shootBullet ();
			} else if (!currentGun.getIsAutomatic () && Input.GetButtonDown ("Fire1")) {
				shootBullet ();
			}
		}

		if (Input.GetButtonDown("Fire2")) {
			isAds = true;
			hipToAds();
		} else if (Input.GetButtonUp("Fire2")) {
			isAds = false;
			adsToHip(false);
			if (currentGun.getIsScoped ()) {
				stillScoped = false;
			}
		}
	}
	private void setGunLayers () {
		// ONLY FOR USERS OWN PLAYER
		for (int i = 0; i < gunObjects.Length; i++) {
			gunObjects[i].layer = 12;
			foreach (Transform child in gunObjects[i].transform) {
				child.gameObject.layer = 12;
			}
		}
		armPivot.layer = 12;
	}
	[PunRPC]
	private void shoot(Vector3 start, Vector3 end, int userId) {
		audioSource.PlayOneShot (currentGun.getGunShotSound());
		if(!PhotonNetwork.isMasterClient) { return; }
		//create the bullet at tip of gun
		PhotonNetwork.Instantiate ("Bullet", start ,Quaternion.LookRotation(Vector3.Normalize(end-start)), 0, new object[] {userId, Vector3.Normalize(end-start)*currentGun.getBulletSpeed(), photonView.viewID, currentGun.getBulletDamage(), currentGun.getDropOff(), currentGun.getDropOffStop()});
		//shot.GetComponent<Rigidbody>().velocity = Vector3.Normalize(end-start)*300;
		//shot.GetComponent<Bullet> ().setUserId (userId);
	}

	private IEnumerator reloadwait()  {
		isReloading = true;
		yield return new WaitForSeconds(currentGun.getReloadTime());
		currentGun.reload ();
		gui.setAmmoCounter (currentGun.getAmmo(), currentGun.getMagCapacity());
		isReloading = false;
		canShoot = true;
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

	private void shootBullet() {
		//Get Point where bullet will hit
		StartCoroutine(delayedShooting());
		armPivotAnimator.Play(currentGun.getShootingAnimationName());
		if (isAds) {
			ray = new Ray (playerCamera.transform.position, playerCamera.transform.forward * 100);
		} else {
			float randomRadius = Random.Range (0f, coneRadius);
			float randomAngle = Random.Range (0f, 2 * Mathf.PI);
			Vector3 randomDirection = new Vector3 (randomRadius * Mathf.Cos (randomAngle), randomRadius * Mathf.Sin (randomAngle), coneLength);
			ray = new Ray (playerCamera.transform.position, transform.TransformDirection(randomDirection.normalized));
		}

		if (Physics.Raycast (ray, out hit, Mathf.Infinity, ignoreRayCastLayer)) {
			endpoint = ray.GetPoint (hit.distance);
		} else {
			endpoint = ray.GetPoint (1000);
		}
		gameObject.GetComponent<PhotonView>().RPC("shoot",PhotonTargets.All, currentGun.getJustTheTip().transform.position,endpoint, player.getUserId());
		currentGun.ammoShot ();
		gui.setAmmoCounter (currentGun.getAmmo(), currentGun.getMagCapacity());
		if (currentGun.getIsScoped() && isAds) {
			stillScoped = true;
		}
	}

	private void swapGuns(GameObject newGun) {
		stillScoped = false;
		currentGun.gameObject.SetActive (false);
		newGun.SetActive (true);
		currentGun = newGun.GetComponent<Gun>();
		gui.setAmmoCounter (currentGun.getAmmo (), currentGun.getMagCapacity ());
		if (isAds) {
			adsToHip (true);
		}

	}
	private void adsToHip(bool direct) {
		if (!direct) {
			StartCoroutine (lerpGunPosition (gunContainer.transform.localPosition, currentGun.hip, 0.07f));
		} else {
			StartCoroutine (lerpGunPosition (gunContainer.transform.localPosition, currentGun.hip, 0f));
		}
		playerController.changeAdsState (false);
		gui.setCrosshairEnabled(true);
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
    	StartCoroutine(lerpGunPosition(gunContainer.transform.localPosition, currentGun.ads, 0.07f));
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
