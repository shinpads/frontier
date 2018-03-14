using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	[SerializeField] private GameObject armPivot;
	[SerializeField] private GameObject gunContainer;
	[SerializeField] private GameObject revolver;
	[SerializeField] private GameObject sniper;
	[SerializeField] private GameObject semiAuto;
	[SerializeField] private AudioSource audioSource;
	private Animator armPivotAnimator;
	private Gun currentGun;
	private PhotonView photonView;
	LayerMask ignoreRayCastLayer;
	Character player;
	PlayerController playerController;

	IEnumerator reloadwait()  {
	    canShoot = false;
	    yield return new WaitForSeconds(currentGun.getReloadTime());
	    canShoot = true;
	    currentGun.reload ();
	    gui.setAmmoCounter (currentGun.getAmmo(), currentGun.getMagCapacity());

	}

	void Start () {
		currentGun = revolver.GetComponent<Gun>();
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
	}

	void Update () {
		if (!photonView.isMine) { return; }

		if (Input.GetKeyDown (KeyCode.R) && currentGun.getAmmo() != currentGun.getMagCapacity()) {
		     StartCoroutine(reloadwait());
		}

		if (Input.GetKeyDown (KeyCode.Alpha1) && currentGun != revolver) {
			swapGuns (revolver);
		}

		else if (Input.GetKeyDown (KeyCode.Alpha2) && currentGun != sniper) {
			swapGuns(sniper);
		}

		else if ((Input.GetKeyDown (KeyCode.Alpha3) && currentGun != semiAuto)) {
			swapGuns (semiAuto);
		}

		if (currentGun.getAmmo() == 0) {
			canShoot = false;
		}

		if (Input.GetButtonDown ("Fire1") && canShoot) {
      		//Get Point where bullet will hit
      		StartCoroutine(delayedShooting());
     		armPivotAnimator.SetTrigger("shooting");
			ray = new Ray(playerCamera.transform.position,playerCamera.transform.forward*100);
			if (Physics.Raycast(ray ,out hit, Mathf.Infinity, ignoreRayCastLayer)) {
				endpoint = ray.GetPoint(hit.distance);
			} else {
				endpoint = ray.GetPoint(1000);
			}

			gameObject.GetComponent<PhotonView>().RPC("shoot",PhotonTargets.All, currentGun.getJustTheTip().transform.position,endpoint, player.getUserId());
			currentGun.ammoShot ();
			gui.setAmmoCounter (currentGun.getAmmo(), currentGun.getMagCapacity());
		}
		if (Input.GetButtonDown("Fire2")) {
			gunContainer.transform.localPosition = currentGun.ads;
			playerCamera.fieldOfView = currentGun.adsFov;
			gui.setCrosshairEnabled(false);
			if (currentGun.getIsScoped()) {
				gui.setScopeEnabled(true);
				gunCamera.SetActive(false);
				playerController.setSensitivity(2);
			} else {
				playerController.setSensitivity(1);
			}
		} else if (Input.GetButtonUp("Fire2")) {
			gunContainer.transform.localPosition = currentGun.hip;
			playerCamera.fieldOfView = 60;
			gui.setCrosshairEnabled(true);
			gui.setScopeEnabled(false);
			gunCamera.SetActive(true);
			playerController.setSensitivity(0);
		}
	}

	[PunRPC]
	private void shoot(Vector3 start, Vector3 end, int userId) {
		audioSource.PlayOneShot (currentGun.getGunShotSound());
		if(!PhotonNetwork.isMasterClient) { return; }
		//create the bullet at tip of gun
		PhotonNetwork.Instantiate ("Bullet", start ,Quaternion.LookRotation(Vector3.Normalize(end-start)), 0, new object[] {userId, Vector3.Normalize(end-start)*currentGun.getBulletSpeed(), photonView.viewID, currentGun.getBulletDamage()});
		//shot.GetComponent<Rigidbody>().velocity = Vector3.Normalize(end-start)*300;
		//shot.GetComponent<Bullet> ().setUserId (userId);
	}

    private IEnumerator delayedShooting(){
        canShoot = false;
		yield return new WaitForSeconds(currentGun.getShotDelay());
        canShoot = true;
    }

	private void swapGuns(GameObject newGun) {
		currentGun.gameObject.SetActive (false);
		newGun.SetActive (true);
		currentGun = newGun.GetComponent<Gun>();
		gui.setAmmoCounter (currentGun.getAmmo (), currentGun.getMagCapacity ());
	}
}
